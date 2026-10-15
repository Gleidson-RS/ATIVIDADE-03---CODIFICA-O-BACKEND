using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplicacaoBackAula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PessoasController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public PessoasController(BdAulaBackendContext context)
        {
            _context = context;
        }

        // GET: api/Pessoas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pessoa>>> GetPessoas()
        {
            return await _context.Pessoas.ToListAsync();
        }


        // GET: api/Pessoas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pessoa>> GetPessoa(int id)
        {
            var Pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(c => c.Idpessoa == id);

            if (Pessoa == null)
            {
                return NotFound();
            }

            return Pessoa;
        }


        // POST: api/Pessoas
        [HttpPost]
        public async Task<ActionResult<Pessoa>> PostPessoa(Pessoa Pessoa)
        {
            _context.Pessoas.Add(Pessoa);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPessoa),
                new { id = Pessoa.Idpessoa },
                Pessoa
            );
        }


        // PUT: api/Pessoas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPessoa(
            int id,
            Pessoa Pessoa)
        {
            if (id != Pessoa.Idpessoa)
            {
                return BadRequest();
            }

            _context.Entry(Pessoa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PessoaExiste(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }


        // DELETE: api/Pessoas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePessoa(int id)
        {
            var Pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(c => c.Idpessoa == id);

            if (Pessoa == null)
            {
                return NotFound();
            }

            _context.Pessoas.Remove(Pessoa);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool PessoaExiste(int id)
        {
            return _context.Pessoas
                .Any(e => e.Idpessoa == id);
        }
    }
}