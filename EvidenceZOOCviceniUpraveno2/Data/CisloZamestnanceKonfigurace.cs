using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2.Data
{
    /// <summary>
    /// Konfigurace generování unikátních čísel zaměstnanců. Umožňuje
    /// upravit formát (prefix, počet číslic) i pokračovat v číslování
    /// od posledního vydaného čísla.
    /// </summary>
    class CisloZamestnanceKonfigurace
    {
        /// <summary>Volitelný prefix před číslem (např. "" nebo "Z").</summary>
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; } = "";

        /// <summary>Počet číslic čísla, doplněných nulami zleva.</summary>
        [JsonPropertyName("pocetCislic")]
        public int PocetCislic { get; set; } = 8;

        /// <summary>Poslední vydané pořadové číslo (bez prefixu/nul).</summary>
        [JsonPropertyName("posledniCislo")]
        public long PosledniCislo { get; set; } = 85016545; // další vydané bude 85016546

        /// <summary>
        /// Vygeneruje další unikátní číslo a zvýší interní počítadlo.
        /// </summary>
        public string DalsiCislo()
        {
            PosledniCislo++;
            return $"{Prefix}{PosledniCislo.ToString().PadLeft(PocetCislic, '0')}";
        }
    }
}