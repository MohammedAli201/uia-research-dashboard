using Newtonsoft.Json;

namespace PBDASHBOARD.Models
{
    public class Publisher
    {
        [JsonProperty("cristin_publisher_id")] public string cristin_publisher_id { get; set; }

        [JsonProperty("name")] public string name { get; set; }

        [JsonProperty("url")] public string url { get; set; }
    }
}