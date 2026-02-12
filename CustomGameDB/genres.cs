using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameDB
{
    public class genres
    {
        public int id { get; set; }
        public string name { get; set; }
        public String slug { get; set; }
        public int games_count { get; set; }
        public String image_background { get; set; }
        public ObservableCollection<stores> stores { get; set; }

    }
}
