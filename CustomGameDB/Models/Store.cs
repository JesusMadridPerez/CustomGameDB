using System;
using System.Collections.Generic;

namespace CustomGameDB.Models;

public partial class Store
{
    public int Idstore { get; set; }
    public string? Valuestore { get; set; }
    public virtual ICollection<Game> Idgames { get; set; } = new List<Game>();
}
