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

        /// <summary>
        /// Název položky s automatickým převodem na TitleCase.
        /// </summary>
        [JsonPropertyName("nazev")]
        public string Nazev
        {
            get => _nazev;
            internal set => _nazev = TitleCase.ToTitleCase(value); // Převedení prvních písmen na velká[cite: 15]
        }

        /// <summary> Katedorie skladové položky (např. Krmivo, Léky). </summary>
        [JsonPropertyName("kategorie")]
        public KategoriePolozky Kategorie { get; internal set; }

        /// <summary> Aktuální stav zásoby na skladě. </summary>
        [JsonPropertyName("mnozstvi")]
        public double Mnozstvi { get; internal set; }

        /// <summary> Měrná jednotka (např. kg, l, ks). </summary>
        [JsonPropertyName("jednotka")]
        public string Jednotka { get; internal set; } = string.Empty;

        /// <summary> Hranice minimálního množství pro vyvolání varování o nedostatku. </summary>
        [JsonPropertyName("minimalniStav")]
        public double MinimalniStav { get; internal set; }

        /// <summary>
        /// Vypočítaná vlastnost určující, zda stav zásoby klesl na nebo pod stanovené minimum.
        /// Ukládání do JSONu je ignorováno.
        /// </summary>
        [JsonIgnore]
        public bool JeDochazejici => Mnozstvi <= MinimalniStav;

        /// <summary>
        /// Vytvoří novou instanci skladové položky.
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

        /// <summary> Formátovaný výstup položky zarovnaný do sloupců. </summary>
        public override string ToString()
        {
            return $"{Nazev,-20} {Kategorie,-25} {Mnozstvi,8:0.##} {Jednotka,-8}";
        }
        
        /// <summary> Vrací formátovaný řetězec položky pro konzolový výpis. </summary>
        public string VypisPolozku()
        {
            return ToString();
        }
    }
}