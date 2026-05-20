using System;
using System.Collections.Generic;

namespace CustomGameDB.Models;

public partial class Developer
{
    public int Iddeveloper { get; set; }

    public string? Valuedeveloper { get; set; }

    public virtual ICollection<Game> Idgames { get; set; } = new List<Game>();
}
