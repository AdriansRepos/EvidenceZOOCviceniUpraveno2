using EvidenceZOOCviceniUpraveno2.Enumy;
using System.Text.Json.Serialization;

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

        public void VypisZaznam()
        {
            Console.ForegroundColor = Uroven 
            switch
            {
                UrovenLogu.Chyba => ConsoleColor.Red,
                UrovenLogu.Varovani => ConsoleColor.Yellow,
                _ => ConsoleColor.Gray
            };

            Console.WriteLine($"{Cas,-20:g} [{Uroven}] {Zprava}");
            Console.ResetColor();
        }
    }
}