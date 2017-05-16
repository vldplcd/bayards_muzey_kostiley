using Newtonsoft.Json;
using SQLite;

namespace BayardsSafetyApp.Entities
{
    [Table("Media")]
    public class Media
    {
        
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        [JsonProperty("link_m")]
        [Column("url"), Unique]
        public string Url { get; set; }
        [JsonProperty("type_media")]
        [Column("type")]
        public string Type { get; set; }
        [Column("lang")]
        public string Lang { get; set; }
        [Column("id_r")]
        public string Id_r { get; set; }
    }
}
