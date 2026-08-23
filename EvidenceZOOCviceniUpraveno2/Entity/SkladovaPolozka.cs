using System.Text.Json.Serialization;
using EvidenceZOOCviceniUpraveno2.Enumy;
using InputHelper;

namespace EvidenceZOOCviceniUpraveno2.Entity
{   
    /// <summary>
    /// Reprezentuje jednu skladovou položku (krmivo, pomůcku,
    /// lék nebo veterinární materiál). Sleduje aktuální množství
    /// a minimální stav pro upozornění na docházející zásoby.
    /// </summary>
    class SkladovaPolozka
    {
        /// <summary>
        /// Název položky. Automaticky převáděn do formátu TitleCase.
        /// </summary>
        private string _nazev = string.Empty;
        [JsonPropertyName("nazev")]
        public string Nazev
        {
            get => _nazev;
            internal set => _nazev = TitleCase.ToTitleCase(value);
        }

        /// <summary>
        /// Kategorie položky (krmivo, pomůcka, lék/veterinární materiál).
        /// </summary>
        [JsonPropertyName("kategorie")]
        public KategoriePolozky Kategorie { get; internal set; }

        /// <summary>
        /// Aktuální množství skladem.
        /// </summary>
        [JsonPropertyName("mnozstvi")]
        public double Mnozstvi { get; internal set; }

        /// <summary>
        /// Měrná jednotka (např. kg, l, ks, balení).
        /// </summary>
        [JsonPropertyName("jednotka")]
        public string Jednotka { get; internal set; } = string.Empty;

        /// <summary>
        /// Minimální stav, při jehož dosažení nebo poklesu pod něj
        /// je položka označena jako docházející.
        /// </summary>
        [JsonPropertyName("minimalniStav")]
        public double MinimalniStav { get; internal set; }

        /// <summary>
        /// Určuje, zda množství kleslo na nebo pod minimální stav.
        /// </summary>
        [JsonIgnore]
        public bool JeDochazejici => Mnozstvi <= MinimalniStav;

        /// <summary>
        /// Vytvoří novou skladovou položku.
        /// </summary>
        [JsonConstructor]
        public SkladovaPolozka(string nazev, KategoriePolozky kategorie,
            double mnozstvi, string jednotka, double minimalniStav)
        {
            Nazev = nazev;
            Kategorie = kategorie;
            Mnozstvi = mnozstvi;
            Jednotka = jednotka;
            MinimalniStav = minimalniStav;
        }

        /// <summary>
        /// Vypíše informace o položce do konzole. Docházející položky
        /// jsou zvýrazněny červeně.
        /// </summary>
        public void VypisPolozku()
        {
            if (JeDochazejici)
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(
                $"{Nazev,-20} {Kategorie,-25} {Mnozstvi,8:0.##} {Jednotka,-8} " +
                $"(min. {MinimalniStav:0.##})"
            );

            Console.ResetColor();
        }
    }
}