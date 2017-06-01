using System.Collections.Generic;

namespace BayardsSafetyApp.Entities
{
    public class Risk
    {
 
        public int _id { get; set; }
 
        public string Id_r { get; set; }

        public string Content { get; set; }
        
        public List<Media> Media { get; set; }
        
        public string Parent_s { get; set; }
        
        public string Name { get; set; }
        
        public string Lang { get; set; }
        
        public int? Order { get; set; }
    }
}
