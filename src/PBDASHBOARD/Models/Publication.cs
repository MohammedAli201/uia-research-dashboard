using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PBDASHBOARD.Models.AssociationModels;

namespace PBDASHBOARD.Models
{
    /// <summary>
    /// Represents the result of a research project
    /// </summary>
    public class Publication
    {
        public int Id { get; set; }

        [JsonProperty("category")] public Category Category { get; set; }

        [JsonProperty("cristin_result_id")] public string CristinId { get; set; }

        [JsonProperty("year_published")] public string YearPublished { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("date_published")] public DateTime? DatePublished { get; set; }
        
        /** List of contributors **/
        public List<PublicationContributor> Contributors { get; set; } = new List<PublicationContributor>();
        public List<PublicationInstitute> Institutes { get; set; } = new List<PublicationInstitute>();
    }
}
