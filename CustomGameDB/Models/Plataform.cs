using System;
using System.Collections.Generic;

namespace CustomGameDB.Models;

public partial class Plataform
{
    public int Id { get; set; }

    public string? Slug { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Plataform1> Plataform1s { get; set; } = new List<Plataform1>();
}
