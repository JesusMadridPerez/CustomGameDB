using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;


namespace CustomGameDB.trailers
{
    public class Trailers
    {
        [JsonPropertyName("480")]
        public String? _480 { get; set; }
        public String? max { get; set; }


    }
}
