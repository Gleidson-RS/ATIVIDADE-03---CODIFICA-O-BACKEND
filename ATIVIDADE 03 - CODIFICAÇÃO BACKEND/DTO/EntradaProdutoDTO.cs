namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO
{
    // ENTRADA (POST/PUT)
    public class EntradaProdutoRequestDTO
    {
        public int Iditem { get; set; }                 // item/produto que está entrando no estoque
        public DateOnly DataEntradaProduto { get; set; }
        public decimal QuantidadeEntrada { get; set; }
        public string? Unidade { get; set; }
        public decimal ValorUnitarioEntrada { get; set; }
    }

    // SAÍDA (GET/POST/PUT)
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

        // Lido do banco DEPOIS do SaveChanges: mostra o efeito da trigger
        public int QuantidadeEstoqueAtual { get; set; }
    }
}
