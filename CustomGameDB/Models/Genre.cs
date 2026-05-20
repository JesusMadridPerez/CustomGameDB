using System;
using System.Collections.Generic;

namespace CustomGameDB.Models;

public partial class Genre
{
    public int IdGenres { get; set; }

    public string? ValueGenres { get; set; }

    public virtual ICollection<Game> IdGames { get; set; } = new List<Game>();
}
