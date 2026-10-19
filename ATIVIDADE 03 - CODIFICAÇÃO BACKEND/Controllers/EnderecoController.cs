using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacaoBackAula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnderecosController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public EnderecosController(BdAulaBackendContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Endereco>>> GetEnderecos()
        {
            return await _context.Enderecos.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Endereco>> GetEndereco(int id)
        {
            var Endereco = await _context.Enderecos
                .FirstOrDefaultAsync(c => c.Idendereco == id);

            if (Endereco == null)
            {
                return NotFound();
            }

            return Endereco;
        }


        [HttpPost]
        public async Task<ActionResult<Endereco>> PostEndereco(Endereco Endereco)
        {
            _context.Enderecos.Add(Endereco);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEndereco),
                new { id = Endereco.Idendereco },
                Endereco
            );
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndereco(
            int id,
            Endereco Endereco)
        {
            if (id != Endereco.Idendereco)
            {
                return BadRequest();
            }

            _context.Entry(Endereco).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnderecoExiste(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEndereco(int id)
        {
            var Endereco = await _context.Enderecos
                .FirstOrDefaultAsync(c => c.Idendereco == id);

            if (Endereco == null)
            {
                return NotFound();
            }

            _context.Enderecos.Remove(Endereco);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool EnderecoExiste(int id)
        {
            return _context.Enderecos
                .Any(e => e.Idendereco == id);
        }
    }
}