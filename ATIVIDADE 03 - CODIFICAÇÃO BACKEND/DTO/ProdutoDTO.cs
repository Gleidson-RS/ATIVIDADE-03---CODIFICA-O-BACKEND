namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO
{
    public class ProdutoDTO
    {
        //DADOS TABELA ITEM
        public int IdCategoria { get; set; }
        public int Idcategoria { get; internal set; }
        public string? Item { get; set; }
        public string? DescricaoItem { get; set; }
        public decimal Preco { get; set; }
        public string? TipoItem { get; set; }
        public string? AtivoProdutoServico { get; set; }


        //DADOS TABELA PRODUTO
        public int CodigoPreco { get; set; }
        public decimal PrecoVenda { get; set; }
        public string? Unidade { get; set; }

    }
}
