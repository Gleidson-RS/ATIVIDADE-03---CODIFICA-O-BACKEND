namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.DTO;

public class EnderecoDTO
{
    public int Idendereco { get; set; }   
    public int Idpessoa { get; set; }
    public string? CEP { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
}
