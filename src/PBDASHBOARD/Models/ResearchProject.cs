using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PBDASHBOARD.Models.AssociationModels;

namespace PBDASHBOARD.Models
{
    public class ResearchProject
    {
        public List<InstituteResearchProject> InstituteResearchProjects = new List<InstituteResearchProject>();

        public int Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("cristin_project_id")]
        public string CristinProjectId { get; set; }

        [JsonProperty("title")] public Name Title { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("start_date")]
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("end_date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}