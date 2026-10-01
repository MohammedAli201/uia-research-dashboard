using Newtonsoft.Json;

namespace PBDASHBOARD.Models.ModelMapping
{
    public class ResearchGroupMapper
    {
        [JsonProperty("searchResults")] public string searchResults { get; set; }
    }
}