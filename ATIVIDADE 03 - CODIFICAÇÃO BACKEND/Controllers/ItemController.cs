using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;

namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public ItemController(BdAulaBackendContext context)
        {
            _context = context;
        }

        private static IQueryable<ItemResponseDTO> Projetar(IQueryable<Item> consulta) =>
            consulta.Select(i => new ItemResponseDTO
            {
                Iditem = i.Iditem,
                Idcategoria = i.Idcategoria,
                Categoria = i.IdcategoriaNavigation.Categoria,
                Item = i.Item1,
                DescricaoItem = i.DescricaoItem,
                Preco = i.Preco,
                TipoItem = i.TipoItem,
                AtivoProdutoServico = i.AtivoProdutoServico
            });

        private async Task<string?> ValidarAsync(ItemRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Item) || dto.Item.Length > 40)
                return "O nome do item é obrigatório e deve ter até 40 caracteres.";
            if (string.IsNullOrWhiteSpace(dto.DescricaoItem) || dto.DescricaoItem.Length > 200)
                return "A descrição é obrigatória e deve ter até 200 caracteres.";
            if (dto.Preco < 0)
                return "O preço não pode ser negativo.";
            if (string.IsNullOrWhiteSpace(dto.TipoItem) || dto.TipoItem.Length != 1)
                return "O tipo do item deve ter exatamente 1 caractere.";
            if (string.IsNullOrWhiteSpace(dto.AtivoProdutoServico) || dto.AtivoProdutoServico.Length != 1)
                return "O campo AtivoProdutoServico deve ter exatamente 1 caractere.";

            if (!await _context.Categoria.AnyAsync(c => c.Idcategoria == dto.Idcategoria))
                return $"Categoria {dto.Idcategoria} não encontrada.";

            return null;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemResponseDTO>>> GetAll()
        {
            var lista = await Projetar(_context.Items.AsNoTracking()).ToListAsync();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemResponseDTO>> GetById(int id)
        {
            var item = await Projetar(_context.Items.AsNoTracking().Where(i => i.Iditem == id))
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound($"Item {id} não encontrado.");

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ItemResponseDTO>> Post(ItemRequestDTO dto)
        {
            var erro = await ValidarAsync(dto);
            if (erro != null) return BadRequest(erro);

            var item = new Item
            {
                Idcategoria = dto.Idcategoria,
                Item1 = dto.Item!,
                DescricaoItem = dto.DescricaoItem!,
                Preco = dto.Preco,
                TipoItem = dto.TipoItem!,
                AtivoProdutoServico = dto.AtivoProdutoServico!
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var resposta = await Projetar(_context.Items.AsNoTracking().Where(i => i.Iditem == item.Iditem))
                .FirstAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.Iditem }, resposta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ItemRequestDTO dto)
        {
            var erro = await ValidarAsync(dto);
            if (erro != null) return BadRequest(erro);

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Iditem == id);
            if (item == null)
                return NotFound($"Item {id} não encontrado.");

            item.Idcategoria = dto.Idcategoria;
            item.Item1 = dto.Item!;
            item.DescricaoItem = dto.DescricaoItem!;
            item.Preco = dto.Preco;
            item.TipoItem = dto.TipoItem!;
            item.AtivoProdutoServico = dto.AtivoProdutoServico!;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Iditem == id);
            if (item == null)
                return NotFound($"Item {id} não encontrado.");

            if (await _context.Produtos.AnyAsync(p => p.Iditem == id))
                return Conflict("Não é possível excluir: o item é um produto. Exclua pelo ProdutoItemController.");

            if (await _context.Atendimentoitems.AnyAsync(a => a.Iditem == id))
                return Conflict("Não é possível excluir: o item está vinculado a atendimentos.");

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
