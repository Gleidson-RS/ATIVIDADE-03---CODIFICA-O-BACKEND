
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO;

namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoItemController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public ProdutoItemController(BdAulaBackendContext context)
        {
            _context = context;
        }

        // LISTAR TODOS OS PRODUTOS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            var produtos = await _context.Produtos
                .Include(p => p.IditemNavigation)
                .ToListAsync();

            return Ok(produtos);
        }

        // BUSCAR PRODUTO POR ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.IditemNavigation)
                .FirstOrDefaultAsync(p => p.Iditem == id);

            if (produto == null)
            {
                return NotFound(new
                {
                    mensagem = "Produto não localizado"
                });
            }

            return Ok(produto);
        }

        // ADICIONAR PRODUTO
        [HttpPost]
        public async Task<ActionResult> PostProduto(ProdutoDTO dados)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var item = new Item
                {
                    Idcategoria = dados.IdCategoria,
                    Item1 = dados.Item,
                    DescricaoItem = dados.DescricaoItem,
                    Preco = dados.Preco,
                    TipoItem = dados.TipoItem,
                    AtivoProdutoServico = dados.AtivoProdutoServico
                };

                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                int idItemGerado = item.Iditem;

                var produto = new Produto
                {
                    Iditem = idItemGerado,
                    CodigoPreco = dados.CodigoPreco,
                    PrecoVenda = dados.PrecoVenda,
                    Unidade = dados.Unidade
                };

                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(
                    nameof(GetProduto),
                    new { id = produto.Iditem },
                    produto
                );
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, new
                {
                    mensagem = "Erro ao cadastrar produto."
                });
            }
        }

        // ATUALIZAR PRODUTO
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(
            int id,
            ProdutoDTO dados)
        {
            var produtoExistente = await _context.Produtos
                .Include(p => p.IditemNavigation)
                .FirstOrDefaultAsync(p => p.Iditem == id);

            if (produtoExistente == null)
            {
                return NotFound(new
                {
                    mensagem = "Produto não encontrado."
                });
            }

            produtoExistente.CodigoPreco = dados.CodigoPreco;
            produtoExistente.PrecoVenda = dados.PrecoVenda;
            produtoExistente.Unidade = dados.Unidade;

            produtoExistente.IditemNavigation.Idcategoria = dados.IdCategoria;
            produtoExistente.IditemNavigation.Item1 = dados.Item;
            produtoExistente.IditemNavigation.DescricaoItem = dados.DescricaoItem;
            produtoExistente.IditemNavigation.Preco = dados.Preco;
            produtoExistente.IditemNavigation.TipoItem = dados.TipoItem;
            produtoExistente.IditemNavigation.AtivoProdutoServico = dados.AtivoProdutoServico;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id))
                {
                    return NotFound(new
                    {
                        mensagem = "Produto não encontrado."
                    });
                }

                throw;
            }

            return NoContent();
        }

        // EXCLUIR PRODUTO
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var produto = await _context.Produtos
                    .FirstOrDefaultAsync(p => p.Iditem == id);

                if (produto == null)
                {
                    return NotFound(new
                    {
                        mensagem = "Produto não encontrado."
                    });
                }

                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();

                var item = await _context.Items
                    .FirstOrDefaultAsync(i => i.Iditem == id);

                if (item != null)
                {
                    _context.Items.Remove(item);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, new
                {
                    mensagem = "Erro ao excluir produto.",
                    erro = ex.Message
                });
            }
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(p => p.Iditem == id);
        }
    }
}
