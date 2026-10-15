using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;

namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtendimentoController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public AtendimentoController(BdAulaBackendContext context)
        {
            _context = context;
        }

        // ---------- PROJEÇÃO: atendimento + pessoa + itens ----------
        private static IQueryable<AtendimentoResponseDTO> Projetar(IQueryable<Atendimento> consulta) =>
            consulta.Select(a => new AtendimentoResponseDTO
            {
                Idatendimento = a.Idatendimento,
                DataAtendimento = a.DataAtendimento,
                StatusAtendimento = a.StatusAtendimento,
                Pessoa = new AtendimentoPessoaDTO
                {
                    Idpessoa = a.IdpessoaNavigation.Idpessoa,
                    Nome = a.IdpessoaNavigation.Nome,
                    Cpf = a.IdpessoaNavigation.Cpf,
                    Email = a.IdpessoaNavigation.Email,
                    Telefone = a.IdpessoaNavigation.Telefone
                },
                Itens = a.Atendimentoitems.Select(i => new AtendimentoItemResponseDTO
                {
                    Iditem = i.Iditem,
                    NomeItem = i.IditemNavigation.Item1,
                    Idfuncionario = i.Idfuncionario,
                    NomeFuncionario = i.IdfuncionarioNavigation.IdpessoaNavigation.Nome,
                    QuantidadeItens = i.QuantidadeItens,
                    ValorUnitario = i.ValorunitarioEntrada
                }).ToList()
            });

        // ---------- VALIDAÇÕES (retornam a mensagem de erro, ou null se estiver ok) ----------
        private async Task<string?> ValidarAsync(AtendimentoRequestDTO dto)
        {
            if (!await _context.Pessoas.AnyAsync(p => p.Idpessoa == dto.Idpessoa))
                return $"Pessoa {dto.Idpessoa} não encontrada.";

            if (string.IsNullOrWhiteSpace(dto.StatusAtendimento) || dto.StatusAtendimento.Length != 1)
                return "O status do atendimento deve ter exatamente 1 caractere.";

            if (dto.Itens.Count == 0)
                return "O atendimento precisa ter pelo menos um item.";

            // (Iditem, Idatendimento) é a chave primária: o mesmo item não pode repetir
            var repetido = dto.Itens.GroupBy(i => i.Iditem).FirstOrDefault(g => g.Count() > 1);
            if (repetido != null)
                return $"O item {repetido.Key} está repetido. Some as quantidades em uma única linha.";

            foreach (var i in dto.Itens)
            {
                if (i.QuantidadeItens <= 0)
                    return $"A quantidade do item {i.Iditem} deve ser maior que zero.";
                if (i.ValorUnitario < 0)
                    return $"O valor unitário do item {i.Iditem} não pode ser negativo.";
            }

            // RELACIONAMENTOS: itens e funcionários precisam existir
            var idsItens = dto.Itens.Select(i => i.Iditem).Distinct().ToList();
            var itensExistentes = await _context.Items
                .Where(i => idsItens.Contains(i.Iditem))
                .Select(i => i.Iditem)
                .ToListAsync();
            var itemInexistente = idsItens.Except(itensExistentes).FirstOrDefault();
            if (itemInexistente != 0)
                return $"Item {itemInexistente} não encontrado.";

            var idsFuncionarios = dto.Itens.Select(i => i.Idfuncionario).Distinct().ToList();
            var funcionariosExistentes = await _context.Funcionarios
                .Where(f => idsFuncionarios.Contains(f.Idpessoa))
                .Select(f => f.Idpessoa)
                .ToListAsync();
            var funcInexistente = idsFuncionarios.Except(funcionariosExistentes).FirstOrDefault();
            if (funcInexistente != 0)
                return $"Funcionário {funcInexistente} não encontrado.";

            return null;
        }

        private Task<AtendimentoResponseDTO> BuscarRespostaAsync(int id) =>
            Projetar(_context.Atendimentos.AsNoTracking().Where(a => a.Idatendimento == id)).FirstAsync();

        // ---------- GET ALL (filtro opcional ?idpessoa=) ----------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtendimentoResponseDTO>>> GetAll([FromQuery] int? idpessoa)
        {
            var consulta = _context.Atendimentos.AsNoTracking();

            if (idpessoa.HasValue)
                consulta = consulta.Where(a => a.Idpessoa == idpessoa.Value);

            var lista = await Projetar(consulta.OrderByDescending(a => a.DataAtendimento)).ToListAsync();
            return Ok(lista);
        }

        // ---------- GET POR ID ----------
        [HttpGet("{id}")]
        public async Task<ActionResult<AtendimentoResponseDTO>> GetById(int id)
        {
            var atendimento = await Projetar(
                _context.Atendimentos.AsNoTracking().Where(a => a.Idatendimento == id))
                .FirstOrDefaultAsync();

            if (atendimento == null)
                return NotFound($"Atendimento {id} não encontrado.");

            return Ok(atendimento);
        }

        // ---------- POST ----------
        [HttpPost]
        public async Task<ActionResult<AtendimentoResponseDTO>> Post(AtendimentoRequestDTO dto)
        {
            var erro = await ValidarAsync(dto);
            if (erro != null) return BadRequest(erro);

            var atendimento = new Atendimento
            {
                Idpessoa = dto.Idpessoa,
                DataAtendimento = dto.DataAtendimento,
                StatusAtendimento = dto.StatusAtendimento!,
                Atendimentoitems = dto.Itens.Select(i => new Atendimentoitem
                {
                    Iditem = i.Iditem,
                    Idfuncionario = i.Idfuncionario,
                    QuantidadeItens = i.QuantidadeItens,
                    ValorunitarioEntrada = i.ValorUnitario
                }).ToList()
            };

            _context.Atendimentos.Add(atendimento);
            await _context.SaveChangesAsync();   // atendimento + itens, tudo ou nada

            var resposta = await BuscarRespostaAsync(atendimento.Idatendimento);
            return CreatedAtAction(nameof(GetById), new { id = atendimento.Idatendimento }, resposta);
        }

        // ---------- PUT ----------
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, AtendimentoRequestDTO dto)
        {
            var atendimento = await _context.Atendimentos
                .Include(a => a.Atendimentoitems)
                .FirstOrDefaultAsync(a => a.Idatendimento == id);

            if (atendimento == null)
                return NotFound($"Atendimento {id} não encontrado.");

            var erro = await ValidarAsync(dto);
            if (erro != null) return BadRequest(erro);

            atendimento.Idpessoa = dto.Idpessoa;
            atendimento.DataAtendimento = dto.DataAtendimento;
            atendimento.StatusAtendimento = dto.StatusAtendimento!;

            // SINCRONIZA OS ITENS: remove os que sumiram, atualiza os existentes, adiciona os novos
            var idsRecebidos = dto.Itens.Select(i => i.Iditem).ToHashSet();

            var removidos = atendimento.Atendimentoitems
                .Where(i => !idsRecebidos.Contains(i.Iditem))
                .ToList();
            _context.Atendimentoitems.RemoveRange(removidos);

            foreach (var i in dto.Itens)
            {
                var existente = atendimento.Atendimentoitems.FirstOrDefault(x => x.Iditem == i.Iditem);

                if (existente != null)
                {
                    existente.Idfuncionario = i.Idfuncionario;
                    existente.QuantidadeItens = i.QuantidadeItens;
                    existente.ValorunitarioEntrada = i.ValorUnitario;
                }
                else
                {
                    atendimento.Atendimentoitems.Add(new Atendimentoitem
                    {
                        Iditem = i.Iditem,
                        Idfuncionario = i.Idfuncionario,
                        QuantidadeItens = i.QuantidadeItens,
                        ValorunitarioEntrada = i.ValorUnitario
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ---------- DELETE ----------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var atendimento = await _context.Atendimentos
                .Include(a => a.Atendimentoitems)
                .FirstOrDefaultAsync(a => a.Idatendimento == id);

            if (atendimento == null)
                return NotFound($"Atendimento {id} não encontrado.");

            try
            {
                // PRIMEIRO OS ITENS (filhos), DEPOIS O ATENDIMENTO
                _context.Atendimentoitems.RemoveRange(atendimento.Atendimentoitems);
                _context.Atendimentos.Remove(atendimento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict("Não foi possível excluir o atendimento por causa de vínculos no banco.");
            }

            return NoContent();
        }
    }
}
