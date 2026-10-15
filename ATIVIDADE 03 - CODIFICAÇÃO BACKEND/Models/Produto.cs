using System;
using System.Collections.Generic;

namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;

public partial class Produto
{
    public int Iditem { get; set; }

    public int CodigoPreco { get; set; }

    public decimal PrecoVenda { get; set; }

    public string Unidade { get; set; } = null!;

    public virtual Estoque? Estoque { get; set; }

    public virtual Item IditemNavigation { get; set; } = null!;
}
