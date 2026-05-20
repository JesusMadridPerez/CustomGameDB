using System;
using System.Collections.Generic;

namespace CustomGameDB.Models;

public partial class Plataform1
{
    public int Idgame { get; set; }
    public int Idplataform { get; set; }
    public string? ReleasedAt { get; set; }
    public string? RequirementMinimun { get; set; }

    public string? RequirementMaximun { get; set; }

    public virtual Game IdgameNavigation { get; set; } = null!;

    public virtual Plataform IdplataformNavigation { get; set; } = null!;
}
