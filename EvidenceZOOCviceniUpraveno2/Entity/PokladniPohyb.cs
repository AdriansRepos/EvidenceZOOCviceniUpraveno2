using System.Text.Json.Serialization;
using EvidenceZOOCviceniUpraveno2.Enumy;
using PohybHelper;

namespace EvidenceZOOCviceniUpraveno2.Entity
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
        /// <summary> Typ operace (příjem za prodej / výdaj za storno). </summary>
        [JsonPropertyName("typPohybu")]
        public PokladniTypPohybu TypPohybu { get; internal set; } = typPohybu;

        /// <summary> Kategórie zakoupené vstupenky. </summary>
        [JsonPropertyName("typVstupenky")]
        public TypVstupenky TypVstupenky { get; internal set; } = typVstupenky;

        /// <summary>
        /// Počet kusů u jednoduchých kategorií (dětská, dospělá, ZTP, důchodce).
        /// U rodinných a skupinových vstupenek je vždy 1 (jedna transakce = jedna rodina/skupina).
        /// </summary>
        [JsonPropertyName("pocetKusu")]
        public int PocetKusu { get; internal set; } = pocetKusu;

        /// <summary> Počet dospělých osob v rámci kombinované vstupenky. </summary>
        [JsonPropertyName("pocetDospelych")]
        public int PocetDospelych { get; internal set; } = pocetDospelych;

        /// <summary> Počet dětí v rámci kombinované vstupenky. </summary>
        [JsonPropertyName("pocetDeti")]
        public int PocetDeti { get; internal set; } = pocetDeti;

        /// <summary>
        /// Celková částka pohybu, spočtená a uložená v okamžiku prodeje.
        /// Zůstává historicky přesná i při pozdější změně ceníku.
        /// </summary>
        [JsonPropertyName("castka")]
        public decimal Castka { get; internal set; } = castka;

        /// <summary> Čas razítka prodeje/storna. </summary>
        [JsonPropertyName("datumCas")]
        public DateTime DatumCas { get; internal set; } = datumCas;

        /// <summary>
        /// Vrátí formátovaný přehlední řetězec pohybu včetně popisků a výpočtu kusů.
        /// </summary>
        public string Popis()
        {
            string typText = PopiskyHelper.ZiskejPopisek(TypPohybu);
            string vstupenkaText = PopiskyHelper.ZiskejPopisek(TypVstupenky);
            
            // Diferenciace zobrazení počtu podle typu vstupenky
            string mnozstviText = TypVstupenky is TypVstupenky.RodinaUplna or TypVstupenky.RodinaNeuplna or TypVstupenky.Skupina
                ? $"{PocetDospelych} dosp. + {PocetDeti} dětí"
                : $"{PocetKusu}×";

            return $"{typText}: {vstupenkaText} ({mnozstviText}) – {Castka:0.##} Kč";
        }

        /// <summary> Vrátí datum a čas s popisem transakce zarovnaným do bloku. </summary>
        public override string ToString()
        {
            return $"{DatumCas,-20:g} {Popis()}";
        }
    }
}