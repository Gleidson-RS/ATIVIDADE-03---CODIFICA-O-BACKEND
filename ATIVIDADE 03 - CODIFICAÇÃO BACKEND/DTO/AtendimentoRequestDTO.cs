namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO
{
    public class AtendimentoRequestDTO
    {
        public int Idpessoa { get; set; }
        public DateOnly DataAtendimento { get; set; }
        public string? StatusAtendimento { get; set; }
        public List<AtendimentoItemRequestDTO> Itens { get; set; } = new();
    }
}
