using System.Text.Json.Serialization;
using PohybHelper;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje jeden pokladní pohyb – prodej nebo storno vstupenky.
    /// U jednoduchých kategorií se používá PocetKusu, u rodinných
    /// a skupinových vstupenek PocetDospelych a PocetDeti.
    /// </summary>
    [method: JsonConstructor]
    class PokladniPohyb(PokladniTypPohybu typPohybu, TypVstupenky typVstupenky,
        int pocetKusu, int pocetDospelych, int pocetDeti,
        decimal castka, DateTime datumCas) : IPohyb
    {
        [JsonPropertyName("typPohybu")]
        public PokladniTypPohybu TypPohybu { get; internal set; } = typPohybu;

        [JsonPropertyName("typVstupenky")]
        public TypVstupenky TypVstupenky { get; internal set; } = typVstupenky;

        /// <summary>
        /// Počet kusů u jednoduchých kategorií (dětská, dospělá, ZTP, důchodce).
        /// U rodinných a skupinových vstupenek je vždy 1 (jedna transakce = jedna rodina/skupina).
        /// </summary>
        [JsonPropertyName("pocetKusu")]
        public int PocetKusu { get; internal set; } = pocetKusu;

        [JsonPropertyName("pocetDospelych")]
        public int PocetDospelych { get; internal set; } = pocetDospelych;

        [JsonPropertyName("pocetDeti")]
        public int PocetDeti { get; internal set; } = pocetDeti;

        /// <summary>
        /// Celková částka pohybu, spočtená a uložená v okamžiku prodeje.
        /// Zůstává historicky přesná i při pozdější změně ceníku.
        /// </summary>
        [JsonPropertyName("castka")]
        public decimal Castka { get; internal set; } = castka;

        [JsonPropertyName("datumCas")]
        public DateTime DatumCas { get; internal set; } = datumCas;

        public string Popis()
        {
            string typText = PopiskyHelper.ZiskejPopisek(TypPohybu);
            string vstupenkaText = PopiskyHelper.ZiskejPopisek(TypVstupenky);
            string mnozstviText = TypVstupenky is TypVstupenky.RodinaUplna or TypVstupenky.RodinaNeuplna or TypVstupenky.Skupina
                ? $"{PocetDospelych} dosp. + {PocetDeti} dětí"
                : $"{PocetKusu}×";

            return $"{typText}: {vstupenkaText} ({mnozstviText}) – {Castka:0.##} Kč";
        }

        public void VypisPohyb()
        {
            Console.ForegroundColor = TypPohybu == PokladniTypPohybu.Prodej
                ? ConsoleColor.Green
                : ConsoleColor.DarkYellow;

            Console.WriteLine($"{DatumCas,-20:g} {Popis()}");
            Console.ResetColor();
        }
    }
}