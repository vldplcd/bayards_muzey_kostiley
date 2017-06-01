using Newtonsoft.Json;

namespace BayardsSafetyApp.Entities
{
    public class Media
    {
        public int Id { get; set; }
        [JsonProperty("link_m")]
        public string Url { get; set; }
        [JsonProperty("type_media")]
        public string Type { get; set; }
        public string Lang { get; set; }
        public string Id_r { get; set; }
        public double Width { get; set; }
    }
}
