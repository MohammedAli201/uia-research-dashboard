using System.Collections.Generic;
using Newtonsoft.Json;

namespace PBDASHBOARD.Models
{
    public class Faculty
    {
        public Faculty()
        {
        }

        public int Id { get; set; }

        [JsonProperty("cristin_unit_id")] public string CristinUnitId { get; set; }

        [JsonProperty("unit_name")] public Name UnitName { get; set; }

        public int NameId { get; set; }

        [JsonProperty("url")] public string Url { get; set; }
        
        /// <summary>
        /// Navigation property between Institution and Faculty
        /// </summary>
        public Institution Institution { get; set; }
        
        //List of institutions in the faculty
        public List<Institute> Institutes { get; set; } = new List<Institute>();
                
        // List of Researchers in the faculty
        public List<Contributor> Researchers { get; set; } = new List<Contributor>();
    }
}