using Newtonsoft.Json;

namespace PBDASHBOARD.Models
{
    public class Institution
    {
        [JsonProperty("cristin_institution_id")]
        public string CristinUnitId { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

        public Name InstitutionName { get; set; }
        public string Acronym { get; set; }
        public string Country { get; set; }
        public int Id { get; set; }
        
        
    }
}