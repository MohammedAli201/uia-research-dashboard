using System.Collections.Generic;
using Newtonsoft.Json;

namespace PBDASHBOARD.Models
{
    public class Category
    {
        public Category()
        {
        }

        public Category(string code)
        {
            Code = code;
        }

        public int id { get; set; }

        [JsonProperty("code")] public string Code { get; set; }
        
        [JsonProperty("name")] public Name Name { get; set; }

        public int NameId { get; set; }
    }
    
}