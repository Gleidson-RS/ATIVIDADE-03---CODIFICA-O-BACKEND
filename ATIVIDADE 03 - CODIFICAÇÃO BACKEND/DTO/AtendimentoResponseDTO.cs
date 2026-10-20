namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO
{
    public class AtendimentoResponseDTO
    {
        public int Idatendimento { get; set; }
        public DateOnly DataAtendimento { get; set; }
        public string? StatusAtendimento { get; set; }
        public AtendimentoPessoaDTO Pessoa { get; set; } = new();
        public List<AtendimentoItemResponseDTO> Itens { get; set; } = new();
        public decimal ValorTotal => Itens.Sum(i => i.Subtotal);
    }
}
