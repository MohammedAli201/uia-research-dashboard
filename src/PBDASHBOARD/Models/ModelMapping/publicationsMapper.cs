using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PBDASHBOARD.Models.ModelMapping
{
    public class Contributors
    {
        [JsonProperty("url")] public string url { get; set; }

        [JsonProperty("count")] public int count { get; set; }

        [JsonProperty("preview")] public IList<person> preview { get; set; }
    }

   

    public class Link
    {
        [JsonProperty("url_type")] public string url_type { get; set; }

        [JsonProperty("url")] public string url { get; set; }
    }

    public class Title
    {
        [JsonProperty("en")] public string en { get; set; }

        [JsonProperty("nb")] public string nb { get; set; }

        [JsonProperty("no")] public string no { get; set; }

        [JsonProperty("fr")] public string fr { get; set; }

        [JsonProperty("de")] public string de { get; set; }

        [JsonProperty("mu")] public string mu { get; set; }

        [JsonProperty("sv")] public string sv { get; set; }

        [JsonProperty("nn")] public string nn { get; set; }
    }

    public class Journal
    {
        [JsonProperty("cristin_journal_id")] public string cristin_journal_id { get; set; }

        [JsonProperty("name")] public string name { get; set; }

        [JsonProperty("publisher")] public Publisher publisher { get; set; }
    }


    public class ArrangedBy
    {
        [JsonProperty("name")] public string name { get; set; }
    }

    public class Event
    {
        [JsonProperty("name")] public string name { get; set; }

        [JsonProperty("location")] public string location { get; set; }

        [JsonProperty("date_from")] public DateTime date_from { get; set; }

        [JsonProperty("date_to")] public DateTime date_to { get; set; }

        [JsonProperty("arranged_by")] public ArrangedBy arranged_by { get; set; }
    }

    public class PartOf
    {
        [JsonProperty("url")] public string url { get; set; }
    }

    public class pbMapper
    {
        [JsonProperty("category")] public Category category { get; set; }

        [JsonProperty("contributors")] public Contributors contributors { get; set; }

        [JsonProperty("cristin_result_id")] public string cristin_result_id { get; set; }

        [JsonProperty("links")] public IList<Link> links { get; set; }

        [JsonProperty("original_language")] public string original_language { get; set; }

        [JsonProperty("year_published")] public string year_published { get; set; }

        [JsonProperty("url")] public string url { get; set; }

        [JsonProperty("date_published")] public DateTime? date_published { get; set; }
    }
}