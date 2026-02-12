using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameDB
{
    public class Store

    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string domain { get; set; }
        public int games_count { get; set; }
        public string image_background { get; set; }
    }
}
