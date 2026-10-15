using System;
using System.Collections.Generic;

namespace ATIVIDADE_03___CODIFICAÇÃO_BACKEND.Models;

public partial class Funcionario
{
    public int Idpessoa { get; set; }

    public string Cargo { get; set; } = null!;

    public DateOnly DataAdmissao { get; set; }

    public virtual ICollection<Atendimentoitem> Atendimentoitems { get; set; } = new List<Atendimentoitem>();

    public virtual Pessoa IdpessoaNavigation { get; set; } = null!;
}
