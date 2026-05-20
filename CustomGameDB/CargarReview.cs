using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameDB
{
    public class CargarReview
    {
        public string NombreUsuario { get; set; } = "Usuario Desconocido";
        public string NombreJuego { get; set; } = "Juego Desconocido";
        public int HorasJugadas { get; set; }
        public decimal? NotaPersonal { get; set; } 
        public string EstadoJuego { get; set; }
        public string? ReviewTexto { get; set; } 
        public string? RutaJuego { get; set; }

        public Boolean? esFavorito { get; set; }

    }
}
