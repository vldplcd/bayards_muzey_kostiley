using BayardsSafetyApp.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayardsSafetyApp.DTO
{
    //The class is created for deserializing json coming from the server
    public class SectionAPI
    {
        [JsonProperty("id_s")]
        public string Id_s { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("risks")]
        public Risk[] Risks { get; set; }

        [JsonProperty("subsections")]
        public SectionAPI[] Subsections { get; set; }
    }
}
