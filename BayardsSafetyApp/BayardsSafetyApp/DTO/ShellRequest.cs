using BayardsSafetyApp.Entities;
using System.Collections.Generic;

namespace BayardsSafetyApp.DTO
{
    public class ShellRequest
    {
        /// <summary>
        /// Data list -- is a list of specified type, SafetyObject/SectionRisk/etc
        /// </summary>
        public List<SectionAPI> Sections { get; set; }
        public List<Location> Locations { get; set; }
    }
}
