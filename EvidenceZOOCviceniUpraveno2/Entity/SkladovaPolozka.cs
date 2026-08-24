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
        private string _nazev = string.Empty;
        [JsonPropertyName("nazev")]
        public string Nazev
        {
            get => _nazev;
            internal set => _nazev = TitleCase.ToTitleCase(value);
        }

        [JsonPropertyName("kategorie")]
        public KategoriePolozky Kategorie { get; internal set; }

        [JsonPropertyName("mnozstvi")]
        public double Mnozstvi { get; internal set; }

        [JsonPropertyName("jednotka")]
        public string Jednotka { get; internal set; } = string.Empty;

        [JsonPropertyName("minimalniStav")]
        public double MinimalniStav { get; internal set; }

        [JsonIgnore]
        public bool JeDochazejici => Mnozstvi <= MinimalniStav;

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

        public override string ToString()
        {
            return $"{Nazev,-20} {Kategorie,-25} {Mnozstvi,8:0.##} {Jednotka,-8}";
        }
        
        public string VypisPolozku()
        {
            return ToString();
        }
    }
}