using System.Text.Json.Serialization;
using PohybHelper;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    /// <summary>
    /// Reprezentuje jeden záznam o pohybu na skladě (naskladnění
    /// nebo vyskladnění položky) včetně data a množství.
    /// </summary>
    [method: JsonConstructor]
    class SkladovyPohyb(string nazevPolozky, SkladovyTypPohybu typPohybu,
        double mnozstvi, DateTime datumCas) : IPohyb
    {
        /// <summary> Název položky, které se pohyb týká. </summary>
        [JsonPropertyName("nazevPolozky")]
        public string NazevPolozky { get; internal set; } = nazevPolozky;

        /// <summary> Typ operace (Naskladnění / Vyskladnění). </summary>
        [JsonPropertyName("typPohybu")]
        public SkladovyTypPohybu TypPohybu { get; internal set; } = typPohybu;

        /// <summary> Množství naskladněného nebo vyskladněného materiálu. </summary>
        [JsonPropertyName("mnozstvi")]
        public double Mnozstvi { get; internal set; } = mnozstvi;

        /// <summary> Časové razítko vykonání operace. </summary>
        [JsonPropertyName("datumCas")]
        public DateTime DatumCas { get; internal set; } = datumCas;

        /// <summary> Vrátí textový popis transakce s určením směru pohybu. </summary>
        public string Popis()
        {
            // Převod enum typu pohybu na uživatelsky přívětivý text[cite: 16]
            string typText = TypPohybu == SkladovyTypPohybu.Naskladneni ? "Naskladnění" : "Vyskladnění";
            return $"{typText}: {NazevPolozky} ({Mnozstvi:0.##})";
        }

        /// <summary> Vrátí časově orazítkovaný záznam pohybu. </summary>
        public override string ToString()
        {
            return $"{DatumCas,-20:g} {TypPohybu} – {NazevPolozky}: {Mnozstvi:0.##}";
        }
        
        /// <summary> Pomocná metoda pro získání textového výstupu pohybu. </summary>
        public string VypisPohyb()
        {
            return ToString();
        }
    }
}