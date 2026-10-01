using System.Collections.Generic;
using Newtonsoft.Json;
using PBDASHBOARD.Models.AssociationModels;

namespace PBDASHBOARD.Models
{
    public class Institute
    {
        public int Id { get; set; }

        [JsonProperty("cristin_unit_id")] public string CristinUnitId { get; set; }
        [JsonProperty("url")] public string url { get; set; }
        [JsonProperty("unit_name")] public Name UnitName { get; set; }
        public int NameId { get; set; }
        
        public Faculty Faculty { get; set; }
        public int FacultyId { get; set; }
        public int TotalCount { get; set; }

        public int getTotalCount()
        {
            return this.TotalCount;
            
        }

        public void incrementTotalCount()
        {
            TotalCount++;
        }

        /// <summary>
        ///     This list maps to the institute the projects it commissioned or participated
        /// </summary>
        public List<PublicationInstitute> PublicationInstitutes { get; set; } = new List<PublicationInstitute>();

        /// <summary>
        ///     association table between Institute and research project
        /// </summary>
        public List<InstituteResearchProject> InstituteResearchProjects { get; set; } =
            new List<InstituteResearchProject>();
    }
}