using Newtonsoft.Json;

namespace PBDASHBOARD.Models
{
    public class UnitName
    {
        [JsonProperty("en")] 
        public string en { get; set; }

        [JsonProperty("nb")]
        public string nb { get; set; }
        
       
    }
}