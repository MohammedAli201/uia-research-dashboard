using System.Collections.Generic;
using Newtonsoft.Json;
using PBDASHBOARD.Models;
using PBDASHBOARD.Models.AssociationModels;

/** Represents a researcher**/
public class Contributor
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("cristin_person_id")]
    public string CristinPersonId { get; set; }

    public int Id { get; set; }


    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("surname")] 
    public string SurName { get; set; }
    
    /// <summary>
    /// Holds the affiliations of the contributor
    /// </summary>
    public List<Affiliation> Affiliations { get; set; } = new List<Affiliation>();
    
    /// <summary>
    /// associated groups 
    /// </summary>
    public List<ContributorReGroup> Groups { get; set; } = new List<ContributorReGroup>();
     
    /**
         * List of Publications Contributor participated
         */
    public List<PublicationContributor> ContributorPublications { get; set; } = new List<PublicationContributor>();
}