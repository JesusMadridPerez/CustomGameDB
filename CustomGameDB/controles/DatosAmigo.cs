using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameDB.controles
{
    public class DatosAmigo
    {
        public int IdAmigo { get; set; }
        public string NombreAmigo { get; set; } = string.Empty;
        public DateTime? FechaAmistad { get; set; }
    }
}
