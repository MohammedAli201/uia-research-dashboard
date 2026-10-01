using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PBDASHBOARD.Models.ModelMapping
{
    public class ProjectsMapper
    {
        public class Title
        {
            [JsonProperty("no")] public string no { get; set; }

            [JsonProperty("en")] public string en { get; set; }
        }

        public class Created
        {
            [JsonProperty("date")] public DateTime date { get; set; }
        }

        public class LastModified
        {
            [JsonProperty("date")] public DateTime date { get; set; }
        }

        public class InstitutionName
        {
            [JsonProperty("en")] public string en { get; set; }
        }

        public class Institution
        {
            [JsonProperty("cristin_institution_id")]
            public string cristin_institution_id { get; set; }

            [JsonProperty("institution_name")] public InstitutionName institution_name { get; set; }

            [JsonProperty("url")] public string url { get; set; }
        }


        public class Unit
        {
            [JsonProperty("cristin_unit_id")] public string cristin_unit_id { get; set; }

            [JsonProperty("unit_name")] public UnitName unit_name { get; set; }

            [JsonProperty("url")] public string url { get; set; }
        }

        public class CoordinatingInstitution
        {
            [JsonProperty("institution")] public Institution institution { get; set; }

            [JsonProperty("unit")] public Unit unit { get; set; }
        }


        public class FundingSourceName
        {
            [JsonProperty("en")] public string en { get; set; }
        }

        public class ProjectFundingSource
        {
            [JsonProperty("funding_source_code")] public string funding_source_code { get; set; }

            [JsonProperty("funding_source_name")] public FundingSourceName funding_source_name { get; set; }
        }

        public class ContactInfo
        {
            [JsonProperty("email")] public string email { get; set; }
        }

        public class Role
        {
            [JsonProperty("role_code")] public string role_code { get; set; }

            [JsonProperty("institution")] public Institution institution { get; set; }

            [JsonProperty("unit")] public Unit unit { get; set; }
        }

        public class Participant
        {
            [JsonProperty("cristin_person_id")] public string cristin_person_id { get; set; }

            [JsonProperty("first_name")] public string first_name { get; set; }

            [JsonProperty("surname")] public string surname { get; set; }

            [JsonProperty("url")] public string url { get; set; }

            [JsonProperty("roles")] public IList<Role> roles { get; set; }
        }

        public class AcademicSummary
        {
            [JsonProperty("no")] public string no { get; set; }

            [JsonProperty("en")] public string en { get; set; }
        }

        public class Method
        {
            [JsonProperty("no")] public string no { get; set; }
        }

        public class Keyword
        {
            [JsonProperty("code")] public string code { get; set; }

            [JsonProperty("name")] public Name name { get; set; }
        }

        public class Project
        {
            [JsonProperty("cristin_project_id")] public string cristin_project_id { get; set; }

            [JsonProperty("published")] public bool published { get; set; }

            [JsonProperty("title")] public Name title { get; set; }

            [JsonProperty("main_language")] public string main_language { get; set; }

            [JsonProperty("start_date")] public DateTime start_date { get; set; }

            [JsonProperty("end_date")] public DateTime end_date { get; set; }

            [JsonProperty("status")] public string status { get; set; }

            [JsonProperty("created")] public Created created { get; set; }

            [JsonProperty("last_modified")] public LastModified last_modified { get; set; }

            [JsonProperty("coordinating_institution")]
            public CoordinatingInstitution coordinating_institution { get; set; }

            [JsonProperty("participants_url")] public string participants_url { get; set; }

            [JsonProperty("academic_summary")] public AcademicSummary academic_summary { get; set; }
        }
    }
}