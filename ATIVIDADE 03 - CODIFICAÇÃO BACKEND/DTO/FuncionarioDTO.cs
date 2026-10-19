namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO
{
    public class FuncionarioRequestDTO
    {
        public string? Cargo { get; set; }
        public DateOnly DataAdmissao { get; set; }


        public PessoaDTO Pessoa { get; set; } = new();
    }

    public class FuncionarioResponseDTO
    {
        public int Idpessoa { get; set; }
        public string? Cargo { get; set; }
        public DateOnly DataAdmissao { get; set; }

        public PessoaDTO Pessoa { get; set; } = new();
    }
}
