using System.Text.Json.Serialization;
using EvidenceZOOCviceniUpraveno2.Enumy;
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
        [JsonPropertyName("nazevPolozky")]
        public string NazevPolozky { get; internal set; } = nazevPolozky;

        [JsonPropertyName("typPohybu")]
        public SkladovyTypPohybu TypPohybu { get; internal set; } = typPohybu;

        [JsonPropertyName("mnozstvi")]
        public double Mnozstvi { get; internal set; } = mnozstvi;

        [JsonPropertyName("datumCas")]
        public DateTime DatumCas { get; internal set; } = datumCas;

        public string Popis()
        {
            string typText = TypPohybu == SkladovyTypPohybu.Naskladneni ? "Naskladnění" : "Vyskladnění";
            return $"{typText}: {NazevPolozky} ({Mnozstvi:0.##})";
        }

        public void VypisPohyb()
        {
            Console.ForegroundColor = TypPohybu == SkladovyTypPohybu.Naskladneni
                ? ConsoleColor.Green
                : ConsoleColor.Yellow;

            Console.WriteLine($"{DatumCas,-20:g} {Popis(),-45}");
            Console.ResetColor();
        }
    }
}