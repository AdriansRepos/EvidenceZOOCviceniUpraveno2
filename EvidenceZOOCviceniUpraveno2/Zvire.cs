using System.Text.Json.Serialization;
using InputHelper;
using DateConverterForJson;
using VekZviratHelper;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje jedno zvíře v zoologické zahradě.
    /// Uchovává jeho název, datum narození a váhu. Věk se počítá
    /// dynamicky z data narození, není ukládán jako pevná hodnota.
    /// </summary>
    class Zvire
    {
        /// <summary>
        /// Název zvířete. Nikdy nesmí být null.
        /// Je automaticky převáděn do formátu TitleCase.
        /// </summary>
        private string _nazev = string.Empty;
        [JsonPropertyName("nazev")]
        public string Nazev
        {
            get => _nazev;
            internal set => _nazev = TitleCase.ToTitleCase(value);
        }

        /// <summary>
        /// Datum narození zvířete. Slouží jako zdroj pro dynamický
        /// výpočet aktuálního věku.
        /// </summary>
        [JsonPropertyName("datumNarozeni")]
        public DateOnly DatumNarozeni { get; internal set; }

        /// <summary>
        /// Váha zvířete v kilogramech.
        /// </summary>
        [JsonPropertyName("vaha")]
        public double Vaha { get; internal set; }

        /// <summary>
        /// Aktuální věk zvířete, vypočtený dynamicky z data narození
        /// při každém přístupu. Nikdy tedy není zastaralý.
        /// </summary>
        [JsonIgnore]
        public string Vek => VypocetVeku.VypocitejVekTextove(DatumNarozeni);

        /// <summary>
        /// Vytvoří nové zvíře a nastaví jeho název, datum narození a váhu.
        /// </summary>
        /// <param name="nazev">Název zvířete.</param>
        /// <param name="datumNarozeni">Datum narození.</param>
        /// <param name="vaha">Váha v kilogramech.</param>
        [JsonConstructor]
        public Zvire(string nazev, DateOnly datumNarozeni, double vaha)
        {
            Nazev = nazev;
            DatumNarozeni = datumNarozeni;
            Vaha = vaha;
        }

        /// <summary>
        /// Vypíše informace o zvířeti do konzole.
        /// </summary>
        public void VypisZvire()
        {
            Console.WriteLine(
                $"{Nazev,-20} {Vek,-15} {Vaha + " kg",-10}"
            );
        }
    }
}