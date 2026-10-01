using Newtonsoft.Json;

namespace PBDASHBOARD.Models.AssociationModels
{
    public class Affiliation
    {
        // an affiliation a contributor have when contributing to this publication. thus A countributor can have 
        // different affiliations in different times.
        
        [JsonProperty("institution")] 
        public Institution Institution { get; set; }
        public int InstitutionId { get; set; }
       
        public Contributor Contributor { get; set; }
        public int ContributorId { get; set; }
        
    }
}