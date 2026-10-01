using Newtonsoft.Json;

namespace PBDASHBOARD.Models
{
    public class Name
    {
        public int id { get; set; }
        
        public Name(){}
        /** its english name **/
        [JsonProperty("en")]
        public string En { get; set; }

        /** norsk bokmål name**/
        [JsonProperty("nb")]
        public string Nb { get; set; }
    }
}