using System;
using System.Collections.Generic;

namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;

public partial class Entradaproduto
{
    public int Identradaproduto { get; set; }

    public int Iditem { get; set; }

    public DateOnly DataEntradaProduto { get; set; }

    public decimal QuantidadeEntrada { get; set; }

    public string Unidade { get; set; } = null!;

    public decimal ValorUnitarioEntrada { get; set; }

    public virtual Estoque IditemNavigation { get; set; } = null!;
}
