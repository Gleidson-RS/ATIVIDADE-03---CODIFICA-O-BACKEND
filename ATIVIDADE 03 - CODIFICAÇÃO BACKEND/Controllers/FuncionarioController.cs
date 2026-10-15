using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;

namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public FuncionarioController(BdAulaBackendContext context)
        {
            _context = context;
        }

        // ---------- CONSULTA BASE (funcionário + pessoa + endereços) ----------
        private IQueryable<Funcionario> ConsultaCompleta() =>
            _context.Funcionarios
                .Include(f => f.IdpessoaNavigation)
                    .ThenInclude(p => p.Enderecos);

        // ---------- MAPEAMENTO MODEL -> DTO ----------
        private static FuncionarioResponseDTO ToResponse(Funcionario f) => new()
        {
            Idpessoa = f.Idpessoa,
            Cargo = f.Cargo,
            DataAdmissao = f.DataAdmissao,
            Pessoa = new PessoaDTO
            {
                Idpessoa = f.IdpessoaNavigation.Idpessoa,
                Nome = f.IdpessoaNavigation.Nome,
                Cpf = f.IdpessoaNavigation.Cpf,
                DataNascimento = f.IdpessoaNavigation.DataNascimento,
                Sexo = f.IdpessoaNavigation.Sexo,
                Email = f.IdpessoaNavigation.Email,
                Telefone = f.IdpessoaNavigation.Telefone,
                AtivoPessoa = f.IdpessoaNavigation.AtivoPessoa,
                Enderecos = f.IdpessoaNavigation.Enderecos.Select(e => new EnderecoDTO
                {
                    Idendereco = e.Idendereco,
                    Idpessoa = e.Idpessoa,
                    CEP = e.Cep,
                    Numero = e.Numero,
                    Complemento = e.Complemento
                }).ToList()
            }
        };

        // ---------- VALIDAÇÕES (retornam a mensagem de erro, ou null se estiver ok) ----------
        private static string? ValidarFuncionario(FuncionarioRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Cargo))
                return "O cargo é obrigatório.";
            if (dto.Cargo.Length > 20)
                return "O cargo deve ter no máximo 20 caracteres.";
            if (dto.DataAdmissao > DateOnly.FromDateTime(DateTime.Today))
                return "A data de admissão não pode ser futura.";
            return null;
        }

        private static string? ValidarPessoa(PessoaDTO p)
        {
            if (string.IsNullOrWhiteSpace(p.Nome) || p.Nome.Length > 100)
                return "O nome é obrigatório e deve ter até 100 caracteres.";
            if (p.Cpf <= 0)
                return "O CPF é obrigatório.";
            if (string.IsNullOrWhiteSpace(p.Sexo) || p.Sexo.Length != 1)
                return "O sexo deve ter exatamente 1 caractere.";
            if (string.IsNullOrWhiteSpace(p.Email) || p.Email.Length > 100)
                return "O e-mail é obrigatório e deve ter até 100 caracteres.";
            if (p.Telefone <= 0)
                return "O telefone é obrigatório.";
            if (string.IsNullOrWhiteSpace(p.AtivoPessoa) || p.AtivoPessoa.Length != 1)
                return "O campo AtivoPessoa deve ter exatamente 1 caractere.";

            foreach (var e in p.Enderecos)
            {
                var erro = ValidarEndereco(e);
                if (erro != null) return erro;
            }
            return null;
        }

        private static string? ValidarEndereco(EnderecoDTO e)
        {
            if (string.IsNullOrWhiteSpace(e.CEP) || e.CEP.Length != 8 || !e.CEP.All(char.IsDigit))
                return "O CEP deve ter exatamente 8 dígitos.";
            if (string.IsNullOrWhiteSpace(e.Numero) || e.Numero.Length > 10)
                return "O número do endereço é obrigatório e deve ter até 10 caracteres.";
            if ((e.Complemento?.Length ?? 0) > 150)
                return "O complemento deve ter no máximo 150 caracteres.";
            return null;
        }

        // ---------- GET ALL ----------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FuncionarioResponseDTO>>> GetAll()
        {
            var lista = await ConsultaCompleta().AsNoTracking().ToListAsync();
            return Ok(lista.Select(ToResponse));
        }

        // ---------- GET POR ID ----------
        [HttpGet("{id}")]
        public async Task<ActionResult<FuncionarioResponseDTO>> GetById(int id)
        {
            var funcionario = await ConsultaCompleta()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Idpessoa == id);

            if (funcionario == null)
                return NotFound($"Funcionário {id} não encontrado.");

            return Ok(ToResponse(funcionario));
        }

        // ---------- POST ----------
        [HttpPost]
        public async Task<ActionResult<FuncionarioResponseDTO>> Post(FuncionarioRequestDTO dto)
        {
            var erro = ValidarFuncionario(dto);
            if (erro != null) return BadRequest(erro);

            Pessoa pessoa;

            if (dto.Pessoa.Idpessoa > 0)
            {
                // VINCULA A UMA PESSOA JÁ EXISTENTE
                var pessoaExistente = await _context.Pessoas
                    .Include(p => p.Enderecos)
                    .FirstOrDefaultAsync(p => p.Idpessoa == dto.Pessoa.Idpessoa);

                if (pessoaExistente == null)
                    return NotFound($"Pessoa {dto.Pessoa.Idpessoa} não encontrada.");

                // A PESSOA NÃO PODE SER FUNCIONÁRIA DUAS VEZES (chave compartilhada)
                if (await _context.Funcionarios.AnyAsync(f => f.Idpessoa == pessoaExistente.Idpessoa))
                    return Conflict("Essa pessoa já está cadastrada como funcionário.");

                pessoa = pessoaExistente;
            }
            else
            {
                // CRIA A PESSOA COM OS ENDEREÇOS
                erro = ValidarPessoa(dto.Pessoa);
                if (erro != null) return BadRequest(erro);

                if (await _context.Pessoas.AnyAsync(p => p.Cpf == dto.Pessoa.Cpf))
                    return Conflict("Já existe uma pessoa com esse CPF.");

                pessoa = new Pessoa
                {
                    Nome = dto.Pessoa.Nome!,
                    Cpf = dto.Pessoa.Cpf,
                    DataNascimento = dto.Pessoa.DataNascimento,
                    Sexo = dto.Pessoa.Sexo!,
                    Email = dto.Pessoa.Email!,
                    Telefone = dto.Pessoa.Telefone,
                    AtivoPessoa = dto.Pessoa.AtivoPessoa!,
                    Enderecos = dto.Pessoa.Enderecos.Select(e => new Endereco
                    {
                        Cep = e.CEP!,
                        Numero = e.Numero!,
                        Complemento = e.Complemento ?? ""
                    }).ToList()
                };
                _context.Pessoas.Add(pessoa);
            }

            var funcionario = new Funcionario
            {
                IdpessoaNavigation = pessoa,
                Cargo = dto.Cargo!,
                DataAdmissao = dto.DataAdmissao
            };

            _context.Funcionarios.Add(funcionario);

            // UM ÚNICO SaveChanges = TUDO OU NADA (pessoa + endereços + funcionário)
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = funcionario.Idpessoa },
                ToResponse(funcionario));
        }

        // ---------- PUT ----------
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, FuncionarioRequestDTO dto)
        {
            var erro = ValidarFuncionario(dto) ?? ValidarPessoa(dto.Pessoa);
            if (erro != null) return BadRequest(erro);

            if (dto.Pessoa.Idpessoa > 0 && dto.Pessoa.Idpessoa != id)
                return BadRequest("O Idpessoa do corpo não confere com o id da URL.");

            var funcionario = await ConsultaCompleta().FirstOrDefaultAsync(f => f.Idpessoa == id);
            if (funcionario == null)
                return NotFound($"Funcionário {id} não encontrado.");

            if (await _context.Pessoas.AnyAsync(p => p.Cpf == dto.Pessoa.Cpf && p.Idpessoa != id))
                return Conflict("Esse CPF pertence a outra pessoa.");

            // DADOS DO FUNCIONÁRIO
            funcionario.Cargo = dto.Cargo!;
            funcionario.DataAdmissao = dto.DataAdmissao;

            // DADOS DA PESSOA
            var pessoa = funcionario.IdpessoaNavigation;
            pessoa.Nome = dto.Pessoa.Nome!;
            pessoa.Cpf = dto.Pessoa.Cpf;
            pessoa.DataNascimento = dto.Pessoa.DataNascimento;
            pessoa.Sexo = dto.Pessoa.Sexo!;
            pessoa.Email = dto.Pessoa.Email!;
            pessoa.Telefone = dto.Pessoa.Telefone;
            pessoa.AtivoPessoa = dto.Pessoa.AtivoPessoa!;

            // ENDEREÇOS: Idendereco > 0 atualiza, Idendereco == 0 cria.
            // (endereços que não vierem no DTO são mantidos)
            foreach (var e in dto.Pessoa.Enderecos)
            {
                if (e.Idendereco > 0)
                {
                    var endereco = pessoa.Enderecos.FirstOrDefault(x => x.Idendereco == e.Idendereco);
                    if (endereco == null)
                        return BadRequest($"O endereço {e.Idendereco} não pertence a essa pessoa.");

                    endereco.Cep = e.CEP!;
                    endereco.Numero = e.Numero!;
                    endereco.Complemento = e.Complemento ?? "";
                }
                else
                {
                    pessoa.Enderecos.Add(new Endereco
                    {
                        Cep = e.CEP!,
                        Numero = e.Numero!,
                        Complemento = e.Complemento ?? ""
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
            var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(f => f.Idpessoa == id);
            if (funcionario == null)
                return NotFound($"Funcionário {id} não encontrado.");

            // NÃO EXCLUI SE O FUNCIONÁRIO JÁ ATENDEU ALGUÉM
            if (await _context.Atendimentoitems.AnyAsync(a => a.Idfuncionario == id))
                return Conflict("Não é possível excluir: o funcionário está vinculado a atendimentos.");

            // REMOVE SÓ O FUNCIONÁRIO; A PESSOA CONTINUA NO BANCO
            _context.Funcionarios.Remove(funcionario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
