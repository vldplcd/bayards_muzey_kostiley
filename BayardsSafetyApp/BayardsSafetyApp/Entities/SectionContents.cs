using System.Collections.Generic;

namespace BayardsSafetyApp.Entities
{
 
    public class SectionContents
    {
   
        public int Id { get; set; }

       
        public Section Section { get; set; }
       
        public List<Risk> Risks { get; set; }
       
        public List<Section> Subsections { get; set; }
    }
}
