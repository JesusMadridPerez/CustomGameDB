using System;
using System.Collections.Generic;

namespace CustomGameDB.Models;

public partial class Game
{
    public int Id { get; set; }

    public string? Slug { get; set; }

    public string? Name { get; set; }

    public DateOnly? Released { get; set; }

    public bool? Tba { get; set; }

    public string? BackgroundImage { get; set; }

    public double? Raiting { get; set; }

    public int? RatingTop { get; set; }

    public int? RatingsCount { get; set; }

    public string? ReviewsTextCount { get; set; }

    public int? Metacritic { get; set; }

    public int? Playtime { get; set; }

    public string? Reviewpersonal { get; set; }

    public double? Notapersonal { get; set; }

    public virtual ICollection<Plataform1> Plataform1s { get; set; } = new List<Plataform1>();

    public virtual ICollection<ReviewsUsuario> ReviewsUsuarios { get; set; } = new List<ReviewsUsuario>();

    public virtual ICollection<Genre> IdGenres { get; set; } = new List<Genre>();

    public virtual ICollection<Developer> Iddevelopers { get; set; } = new List<Developer>();

    public virtual ICollection<Store> Idstores { get; set; } = new List<Store>();
}
