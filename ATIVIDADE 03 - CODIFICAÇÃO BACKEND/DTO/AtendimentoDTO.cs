namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO
{
    public class AtendimentoItemRequestDTO
    {
        public int Iditem { get; set; }
        public int Idfuncionario { get; set; }      
        public int QuantidadeItens { get; set; }
        public decimal ValorUnitario { get; set; }
    }

    public class AtendimentoRequestDTO
    {
        public int Idpessoa { get; set; }           
        public DateOnly DataAtendimento { get; set; }
        public string? StatusAtendimento { get; set; }
        public List<AtendimentoItemRequestDTO> Itens { get; set; } = new();
    }

    
    public class AtendimentoPessoaDTO
    {
        public int Idpessoa { get; set; }
        public string? Nome { get; set; }
        public long Cpf { get; set; }
        public string? Email { get; set; }
        public long Telefone { get; set; }
    }

    public class AtendimentoItemResponseDTO
    {
        public int Iditem { get; set; }
        public string? NomeItem { get; set; }
        public int Idfuncionario { get; set; }
        public string? NomeFuncionario { get; set; }
        public int QuantidadeItens { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Subtotal => QuantidadeItens * ValorUnitario;
    }

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
