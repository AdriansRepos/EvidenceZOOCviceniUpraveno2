using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje jeden záznam o pohybu na skladě (naskladnění
    /// nebo vyskladnění položky) včetně data a množství.
    /// </summary>
    [method: JsonConstructor] 
    class SkladovyPohyb(string nazevPolozky, TypPohybu typPohybu,
        double mnozstvi, DateTime datumCas)
    {
        [JsonPropertyName("nazevPolozky")]
        public string NazevPolozky { get; internal set; } = nazevPolozky;

        [JsonPropertyName("typPohybu")]
        public TypPohybu TypPohybu { get; internal set; } = typPohybu;

        [JsonPropertyName("mnozstvi")]
        public double Mnozstvi { get; internal set; } = mnozstvi;

        [JsonPropertyName("datumCas")]
        public DateTime DatumCas { get; internal set; } = datumCas;

        public void VypisPohyb()
        {
            string typText = TypPohybu == TypPohybu.Naskladneni ? "Naskladnění" : "Vyskladnění";
            Console.ForegroundColor = TypPohybu == TypPohybu.Naskladneni
                ? ConsoleColor.Green
                : ConsoleColor.Yellow;

            Console.WriteLine($"{DatumCas,-20:g} {typText,-15} {NazevPolozky,-20} {Mnozstvi,8:0.##}");
            Console.ResetColor();
        }
    }
}