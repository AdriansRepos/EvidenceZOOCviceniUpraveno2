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
        /// <summary> Úroveň závažnosti logu (Info, Warning, Error). </summary>
        [JsonPropertyName("uroven")]
        public UrovenLogu Uroven { get; internal set; } = uroven;

        /// <summary> Detailní systémová nebo chybová zpráva. </summary>
        [JsonPropertyName("zprava")]
        public string Zprava { get; internal set; } = zprava;

        /// <summary> Přesné datum a čas vzniku systémové události. </summary>
        [JsonPropertyName("cas")]
        public DateTime Cas { get; internal set; } = cas;
    }
}