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
        /// <summary> Cena dětské vstupenky v Kč. </summary>
        [JsonPropertyName("cenaDetska")]
        public decimal CenaDetska { get; set; } = 80;

        /// <summary> Cena dospělé vstupenky v Kč. </summary>
        [JsonPropertyName("cenaDospela")]
        public decimal CenaDospela { get; set; } = 150;

        /// <summary> Cena vstupenky pro držitele ZTP v Kč. </summary>
        [JsonPropertyName("cenaZTP")]
        public decimal CenaZTP { get; set; } = 50;

        /// <summary> Cena důchodcovské vstupenky v Kč. </summary>
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