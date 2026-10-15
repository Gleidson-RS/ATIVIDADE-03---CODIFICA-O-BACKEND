using System;
using System.Collections.Generic;

namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;

public partial class Estoque
{
    public int Iditem { get; set; }

    public int QuantidadeEstoque { get; set; }

    public virtual ICollection<Entradaproduto> Entradaprodutos { get; set; } = new List<Entradaproduto>();

    public virtual Produto IditemNavigation { get; set; } = null!;
}
