using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameDB.trailers
{
    public class gameTrailers
    {
        public int count { get; set; }
        public object next { get; set; }
        public object previous { get; set; }
        public List<GameTrailerResult> results { get; set; }

    }
}
