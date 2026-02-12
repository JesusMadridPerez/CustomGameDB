using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameDB
{
    public class GameResults
    {
        public int count {  get; set; }
        public string? next {  get; set; }
        public string? previous {  get; set; }
        public ObservableCollection<Game>? results {  get; set; }


    }
}
