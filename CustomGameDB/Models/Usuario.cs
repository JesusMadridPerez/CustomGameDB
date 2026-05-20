using System;
using System.Collections.Generic;

namespace CustomGameDB.Models;

public partial class Usuario
{
    public int Idusuario { get; set; }

    public string? Username { get; set; }

    public string? UserPassword { get; set; }

    public string? Email { get; set; }

    public DateOnly? Anyonacimiento { get; set; }

    public virtual ICollection<Amistade> AmistadeIdUsuario1Navigations { get; set; } = new List<Amistade>();

    public virtual ICollection<Amistade> AmistadeIdUsuario2Navigations { get; set; } = new List<Amistade>();

    public virtual ICollection<ReviewsUsuario> ReviewsUsuarios { get; set; } = new List<ReviewsUsuario>();
}
