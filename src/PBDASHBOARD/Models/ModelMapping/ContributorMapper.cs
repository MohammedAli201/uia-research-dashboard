using System.Collections.Generic;
using Newtonsoft.Json;
using PBDASHBOARD.Models.AssociationModels;

namespace PBDASHBOARD.Models.ModelMapping
{
   

    public class person
    {
           

        [JsonProperty("cristin_person_id")] public string CristinPersonId { get; set; }

        [JsonProperty("first_name")] public string FirstName { get; set; }

        [JsonProperty("surname")] public string SurName { get; set; }

        [JsonProperty("affiliations")] 
        public IList<Affiliation> Affiliations { get; set; }
    
        [JsonProperty("url")] public string Url { get; set; }
    }
}