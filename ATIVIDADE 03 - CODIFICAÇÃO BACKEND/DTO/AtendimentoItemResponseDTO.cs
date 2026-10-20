namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO
{
    public class AtendimentoItemResponseDTO // O AtendimentoItemListDTO do professor
    {
        public int Iditem { get; set; }
        public string? NomeItem { get; set; }
        public int Idfuncionario { get; set; }
        public string? NomeFuncionario { get; set; }
        public int QuantidadeItens { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Subtotal => QuantidadeItens * ValorUnitario;
    }
}
