using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;

namespace PBDASHBOARD.Models.ModelMapping
{
    public class CorrespondingUnit
    {
        public string cristin_unit_id { get; set; }
        public string url { get; set; }
    }
    public class institutionMap
    {
        [JsonProperty("cristin_institution_id")]
        public string CristinUnitId { get; set; }
        
        [JsonProperty("institution_name")]
        public Name InstitutionName { get; set; }
        
        [JsonProperty("acronym")]
        public string Acronym { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
        
        [JsonProperty("corresponding_unit")]
        public CorrespondingUnit corresponding_unit { get; set; }
    }

    public class ParentUnit    {
        [JsonProperty("cristin_institution_id")]
        public string CristinUnitId { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

    }

    public class InstitutionUnit    {
        public string cristin_unit_id { get; set; } 
        public UnitName unit_name { get; set; } 
        public string url { get; set; } 
    }

    /** Json Mapping object for institutes request**/
    public class FacultyUnitsRoot    {
        public string cristin_unit_id { get; set; } 
        public UnitName unit_name { get; set; } 
        public Institution institution { get; set; } 
        public ParentUnit parent_unit { get; set; } 
        public List<ParentUnit> parent_units { get; set; } 
        public List<Institute> subunits { get; set; } 
    }

    /** Json root object (Mapping objects)
 * for a Institution request*
     */
    public class units
    {
        public string cristin_unit_id { get; set; }
        public Name unit_name { get; set; }
        public Institution institution { get; set; }
        public List<Faculty> subunits { get; set; }
    }

    public class pubInst
    {
        public Category category { get; set; }
        public Contributor contributors { get; set; }
        public string cristin_result_id { get; set; }

        public string title { get; set; }
        public string year_published { get; set; }
        public string url { get; set; }
        public DateTime? date_published { get; set; }
        public MediaType media_type { get; set; }
        public string place { get; set; }
    }
}