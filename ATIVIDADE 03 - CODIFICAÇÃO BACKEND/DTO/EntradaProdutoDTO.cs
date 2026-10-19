namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO
{
    public class EntradaProdutoRequestDTO
    {
        public int Iditem { get; set; }                 
        public DateOnly DataEntradaProduto { get; set; }
        public decimal QuantidadeEntrada { get; set; }
        public string? Unidade { get; set; }
        public decimal ValorUnitarioEntrada { get; set; }
    }

    public class EntradaProdutoResponseDTO
    {
        public int Identradaproduto { get; set; }
        public int Iditem { get; set; }
        public string? NomeItem { get; set; }
        public DateOnly DataEntradaProduto { get; set; }
        public decimal QuantidadeEntrada { get; set; }
        public string? Unidade { get; set; }
        public decimal ValorUnitarioEntrada { get; set; }
        public decimal ValorTotal { get; set; }

        public int QuantidadeEstoqueAtual { get; set; }
    }
}
