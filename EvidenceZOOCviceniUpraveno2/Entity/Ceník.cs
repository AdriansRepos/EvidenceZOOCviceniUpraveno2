using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    /// <summary>
    /// Ceník vstupného. Obsahuje základní ceny pro jednotlivé kategorie
    /// návštěvníků a procentuální slevy pro rodinné a skupinové vstupenky.
    /// Ukládá se do souboru a je upravitelný za běhu aplikace.
    /// </summary>
    class Cenik
    {
        [JsonPropertyName("cenaDetska")]
        public decimal CenaDetska { get; set; } = 80;

        [JsonPropertyName("cenaDospela")]
        public decimal CenaDospela { get; set; } = 150;

        [JsonPropertyName("cenaZTP")]
        public decimal CenaZTP { get; set; } = 50;

        [JsonPropertyName("cenaDuchodce")]
        public decimal CenaDuchodce { get; set; } = 100;
        
        /// <summary>
        /// Sleva na rodinnou vstupenku v procentech (0-100), uplatněná
        /// na součet cen jednotlivých členů rodiny.
        /// </summary>
        [JsonPropertyName("slevaRodinaProcenta")]
        public decimal SlevaRodinaProcenta { get; set; } = 10;

        /// <summary>
        /// Sleva na skupinovou vstupenku v procentech (0-100), uplatněná
        /// na součet cen jednotlivých členů skupiny.
        /// </summary>
        [JsonPropertyName("slevaSkupinaProcenta")]
        public decimal SlevaSkupinaProcenta { get; set; } = 15;
    }
}