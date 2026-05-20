using System;
using System.Collections.Generic;

namespace CustomGameDB.Models;

public partial class ReviewsUsuario
{
    public int IdUsuario { get; set; }

    public int IdGame { get; set; }

    public decimal? NotaPersonal { get; set; }

    public string? ReviewTexto { get; set; }

    public DateTime? FechaUltimaModificacion { get; set; }

    public int? HorasJugadas { get; set; }

    public string? Estadojuego { get; set; }

    public bool? Esfavorito { get; set; }

    public string? rutaJuego { get; set; }

    public virtual Game IdGameNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; }
}
