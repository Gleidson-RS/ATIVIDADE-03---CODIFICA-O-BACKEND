namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO
{
    // ENTRADA (POST/PUT)
    public class ItemRequestDTO
    {
        public int Idcategoria { get; set; }
        public string? Item { get; set; }
        public string? DescricaoItem { get; set; }
        public decimal Preco { get; set; }
        public string? TipoItem { get; set; }
        public string? AtivoProdutoServico { get; set; }
    }

    // SAÍDA (GET/POST)
    public class ItemResponseDTO
    {
        public int Iditem { get; set; }
        public int Idcategoria { get; set; }
        public string? Categoria { get; set; }
        public string? Item { get; set; }
        public string? DescricaoItem { get; set; }
        public decimal Preco { get; set; }
        public string? TipoItem { get; set; }
        public string? AtivoProdutoServico { get; set; }
    }
}
