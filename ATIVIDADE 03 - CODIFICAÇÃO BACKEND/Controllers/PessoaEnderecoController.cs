using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO;
using ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;

namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class PessoaEnderecoController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public PessoaEnderecoController(BdAulaBackendContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaDTO>>> GetALL()
        {
            var pessoas = await _context.Pessoas.
                Select(p => new PessoaDTO
                {
                    Idpessoa = p.Idpessoa,
                    Nome = p.Nome,
                    Cpf = p.Cpf,
                    DataNascimento = p.DataNascimento,
                    Sexo = p.Sexo,
                    Email = p.Email,
                    Telefone = p.Telefone,
                    AtivoPessoa = p.AtivoPessoa,

                    Enderecos = p.Enderecos.Select(e => new EnderecoDTO
                    {

                        Idpessoa = e.Idpessoa,
                        CEP = e.Cep,
                        Numero = e.Numero,
                        Complemento = e.Complemento

                    }).ToList()

                }).ToListAsync();

            return Ok(pessoas);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaDTO>> GetPessoa(int id)
        {
            var pessoa = await _context.Pessoas
                .Where(p => p.Idpessoa == id)
                .Select(p => new PessoaDTO
                {
                    Idpessoa = p.Idpessoa,
                    Nome = p.Nome,
                    Cpf = p.Cpf,
                    DataNascimento = p.DataNascimento,
                    Sexo = p.Sexo,
                    Email = p.Email,
                    Telefone = p.Telefone,
                    AtivoPessoa = p.AtivoPessoa,

                    Enderecos = p.Enderecos.Select(e => new EnderecoDTO
                    {

                        Idpessoa = e.Idpessoa,
                        CEP = e.Cep,
                        Numero = e.Numero,
                        Complemento = e.Complemento

                    }).ToList()

                }).ToListAsync();

            return Ok(pessoa);

        }

        [HttpPost]
        public async Task<ActionResult<PessoaDTO>> Post(PessoaDTO dto)
        {
            var ObjPessoa = new Pessoa
            {
                Nome = dto.Nome,
                Cpf = dto.Cpf,
                DataNascimento = dto.DataNascimento,
                Sexo = dto.Sexo,
                Email = dto.Email,
                Telefone = dto.Telefone,
                AtivoPessoa = dto.AtivoPessoa,

                Enderecos = dto.Enderecos.Select(e => new Endereco
                {
                    Idpessoa = e.Idpessoa,
                    Numero = e.Numero,
                    Cep = e.CEP,
                    Complemento = e.Complemento

                }).ToList()
            };

            _context.Pessoas.Add(ObjPessoa);

            await _context.SaveChangesAsync();

            var respostaBd = new PessoaDTO
            {
                Idpessoa = ObjPessoa.Idpessoa,
                Nome = ObjPessoa.Nome,
                Cpf = ObjPessoa.Cpf,
                DataNascimento = ObjPessoa.DataNascimento,
                Sexo = ObjPessoa.Sexo,
                Telefone = ObjPessoa.Telefone,
                AtivoPessoa = ObjPessoa.AtivoPessoa,
                Enderecos = ObjPessoa.Enderecos.Select(e => new EnderecoDTO
                {
                    Idpessoa = e.Idpessoa,
                    Numero = e.Numero,
                    CEP = e.Cep,
                    Complemento = e.Complemento
                }).ToList()
            };
            return Ok(respostaBd);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ActionResult<Pessoa>>> Put(int id, PessoaDTO dto)
        {
            var pessoa = await _context.Pessoas
                .Include(p => p.Enderecos)
                .FirstOrDefaultAsync(p => p.Idpessoa == id);

            if (pessoa == null)
            {
                return NotFound();
            }

            pessoa.Nome = dto.Nome;
            pessoa.Cpf = dto.Cpf;
            pessoa.DataNascimento = dto.DataNascimento;
            pessoa.Sexo = dto.Sexo;
            pessoa.Email = dto.Email;
            pessoa.Telefone = dto.Telefone;
            pessoa.AtivoPessoa = dto.AtivoPessoa;

            foreach (var endPessoa in dto.Enderecos)
            {
                if (endPessoa.Idpessoa > 0)
                {

                    //
                    var endereco = pessoa.Enderecos
                    .FirstOrDefault(e => e.Idpessoa == endPessoa.Idpessoa);

                    if (endereco == null)
                    {
                        return BadRequest($"O endereço não pertence a essa pessoa");
                    }


                    endereco.Numero = endPessoa.Numero;
                    endereco.Cep = endPessoa.CEP;
                    endereco.Complemento = endPessoa.Complemento;
                }
                else
                {


                    pessoa.Enderecos.Add(new Endereco
                    {
                        Numero = endPessoa.Numero,
                        Cep = endPessoa.CEP,
                        Complemento = endPessoa.Complemento,
                    });

                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Pessoa>> Delete(int id)
        {

            var pessoa = await _context.Pessoas
                .Include(p => p.Enderecos)
                .FirstOrDefaultAsync(p => p.Idpessoa == id);


            if (pessoa == null)
            {
                return NotFound();
            }


            _context.Pessoas.Remove(pessoa);

            await _context.SaveChangesAsync();

            return NoContent();

        }

    }
    }
