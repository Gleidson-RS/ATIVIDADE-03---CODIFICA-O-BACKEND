using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;

namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntradaProdutoController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public EntradaProdutoController(BdAulaBackendContext context)
        {
            _context = context;
        }


        private static IQueryable<EntradaProdutoResponseDTO> Projetar(IQueryable<Entradaproduto> consulta) =>
            consulta.Select(e => new EntradaProdutoResponseDTO
            {
                Identradaproduto = e.Identradaproduto,
                Iditem = e.Iditem,
                NomeItem = e.IditemNavigation.IditemNavigation.IditemNavigation.Item1,
                DataEntradaProduto = e.DataEntradaProduto,
                QuantidadeEntrada = e.QuantidadeEntrada,
                Unidade = e.Unidade,
                ValorUnitarioEntrada = e.ValorUnitarioEntrada,
                ValorTotal = e.QuantidadeEntrada * e.ValorUnitarioEntrada,
                QuantidadeEstoqueAtual = e.IditemNavigation.QuantidadeEstoque
            });

        private static string? Validar(EntradaProdutoRequestDTO dto)
        {
            if (dto.Iditem <= 0)
                return "O Iditem é obrigatório.";
            if (dto.QuantidadeEntrada <= 0)
                return "A quantidade de entrada deve ser maior que zero.";
            if (dto.ValorUnitarioEntrada < 0)
                return "O valor unitário não pode ser negativo.";
            if (string.IsNullOrWhiteSpace(dto.Unidade) || dto.Unidade.Length > 10)
                return "A unidade é obrigatória e deve ter até 10 caracteres.";
            return null;
        }

        private async Task<int?> ObterEstoqueAsync(int iditem) =>
            await _context.Estoques.AsNoTracking()
                .Where(e => e.Iditem == iditem)
                .Select(e => (int?)e.QuantidadeEstoque)
                .FirstOrDefaultAsync();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntradaProdutoResponseDTO>>> GetAll([FromQuery] int? iditem)
        {
            var consulta = _context.Entradaprodutos.AsNoTracking();

            if (iditem.HasValue)
                consulta = consulta.Where(e => e.Iditem == iditem.Value);

            var lista = await Projetar(consulta.OrderByDescending(e => e.DataEntradaProduto)).ToListAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntradaProdutoResponseDTO>> GetById(int id)
        {
            var entrada = await Projetar(
                _context.Entradaprodutos.AsNoTracking().Where(e => e.Identradaproduto == id))
                .FirstOrDefaultAsync();

            if (entrada == null)
                return NotFound($"Entrada {id} não encontrada.");

            return Ok(entrada);
        }

        [HttpPost]
        public async Task<ActionResult<EntradaProdutoResponseDTO>> Post(EntradaProdutoRequestDTO dto)
        {
            var erro = Validar(dto);
            if (erro != null) return BadRequest(erro);

            if (await ObterEstoqueAsync(dto.Iditem) == null)
                return NotFound($"O item {dto.Iditem} não é um produto com registro em estoque.");

            var entrada = new Entradaproduto
            {
                Iditem = dto.Iditem,
                DataEntradaProduto = dto.DataEntradaProduto,
                QuantidadeEntrada = dto.QuantidadeEntrada,
                Unidade = dto.Unidade!,
                ValorUnitarioEntrada = dto.ValorUnitarioEntrada
            };

            _context.Entradaprodutos.Add(entrada);


            await _context.SaveChangesAsync();

            var resposta = await Projetar(_context.Entradaprodutos.AsNoTracking()
                .Where(e => e.Identradaproduto == entrada.Identradaproduto && e.Iditem == entrada.Iditem))
                .FirstAsync();

            return CreatedAtAction(nameof(GetById), new { id = entrada.Identradaproduto }, resposta);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EntradaProdutoResponseDTO>> Put(int id, EntradaProdutoRequestDTO dto)
        {
            var erro = Validar(dto);
            if (erro != null) return BadRequest(erro);

            var entrada = await _context.Entradaprodutos.FirstOrDefaultAsync(e => e.Identradaproduto == id);
            if (entrada == null)
                return NotFound($"Entrada {id} não encontrada.");

            if (dto.Iditem != entrada.Iditem)
                return BadRequest("Não é permitido trocar o item de uma entrada. Exclua e cadastre outra.");

            var estoque = await ObterEstoqueAsync(entrada.Iditem);
            var diferenca = dto.QuantidadeEntrada - entrada.QuantidadeEntrada;
            if (estoque != null && estoque.Value + diferenca < 0)
                return Conflict("Não é possível reduzir a entrada: o estoque ficaria negativo.");

            entrada.DataEntradaProduto = dto.DataEntradaProduto;
            entrada.QuantidadeEntrada = dto.QuantidadeEntrada;
            entrada.Unidade = dto.Unidade!;
            entrada.ValorUnitarioEntrada = dto.ValorUnitarioEntrada;

            await _context.SaveChangesAsync();

            var resposta = await Projetar(_context.Entradaprodutos.AsNoTracking()
                .Where(e => e.Identradaproduto == id && e.Iditem == entrada.Iditem))
                .FirstAsync();

            return Ok(resposta);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entrada = await _context.Entradaprodutos.FirstOrDefaultAsync(e => e.Identradaproduto == id);
            if (entrada == null)
                return NotFound($"Entrada {id} não encontrada.");

            var estoque = await ObterEstoqueAsync(entrada.Iditem);
            if (estoque != null && estoque.Value < entrada.QuantidadeEntrada)
                return Conflict("Não é possível excluir: parte dessa entrada já foi consumida e o estoque ficaria negativo.");

            _context.Entradaprodutos.Remove(entrada);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
