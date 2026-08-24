using System.Text.Json.Serialization;
using EvidenceZOOCviceniUpraveno2.Enumy;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    /// <summary>
    /// Jeden záznam technického logu – událost na úrovni aplikace/systému
    /// (start, chyby zápisu, obnova ze zálohy apod.), určený pro IT podporu.
    /// </summary>
    [method: JsonConstructor]
    class TechnickyZaznam(UrovenLogu uroven, string zprava, DateTime cas)
    {
        [JsonPropertyName("uroven")]
        public UrovenLogu Uroven { get; internal set; } = uroven;

        [JsonPropertyName("zprava")]
        public string Zprava { get; internal set; } = zprava;

        [JsonPropertyName("cas")]
        public DateTime Cas { get; internal set; } = cas;
    }
}