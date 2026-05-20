using System;
using System.Collections.Generic;

namespace CustomGameDB.Models;

public partial class Amistade
{
    public int IdUsuario1 { get; set; }

    public int IdUsuario2 { get; set; }

    public DateTime? FechaAmistad { get; set; }

    public string? Estado { get; set; }

    public virtual Usuario IdUsuario1Navigation { get; set; } = null!;

    public virtual Usuario IdUsuario2Navigation { get; set; } = null!;
}
