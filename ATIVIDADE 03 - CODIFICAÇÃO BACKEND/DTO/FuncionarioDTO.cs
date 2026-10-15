namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO
{
    // ENTRADA (POST/PUT)
    public class FuncionarioRequestDTO
    {
        public string? Cargo { get; set; }
        public DateOnly DataAdmissao { get; set; }

        // Pessoa.Idpessoa > 0  -> vincula a uma pessoa já cadastrada
        // Pessoa.Idpessoa == 0 -> cria a pessoa (e os endereços) junto com o funcionário
        public PessoaDTO Pessoa { get; set; } = new();
    }

    // SAÍDA (GET/POST)
    public class FuncionarioResponseDTO
    {
        // O id do funcionário é o mesmo id da pessoa (chave compartilhada)
        public int Idpessoa { get; set; }
        public string? Cargo { get; set; }
        public DateOnly DataAdmissao { get; set; }

        public PessoaDTO Pessoa { get; set; } = new();
    }
}
