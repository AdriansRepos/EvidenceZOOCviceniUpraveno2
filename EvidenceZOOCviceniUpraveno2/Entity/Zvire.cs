using System.Text.Json.Serialization;
using InputHelper;
using VekZviratHelper;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    /// <summary>
    /// Reprezentuje jedno zvíře v zoologické zahradě.
    /// Uchovává jeho název, datum narození a váhu. Věk se počítá
    /// dynamicky z data narození, není ukládán jako pevná hodnota.
    /// </summary>
    class Zvire
    {
        private string _nazev = string.Empty;

        /// <summary> Název zvířete (druhu nebo jména) v TitleCase formátu. </summary>
        [JsonPropertyName("nazev")]
        public string Nazev
        {
            get => _nazev;
            internal set => _nazev = TitleCase.ToTitleCase(value); // Převedení počátečních písmen na velká
        }

        /// <summary> Datum narození zvířete. </summary>
        [JsonPropertyName("datumNarozeni")]
        public DateOnly DatumNarozeni { get; internal set; }

        /// <summary> Hmotnost zvířete v kilogramech. </summary>
        [JsonPropertyName("vaha")]
        public double Vaha { get; internal set; }

        /// <summary>
        /// Počítaný věk zvířete v textovém formátu na základě aktuálního data.
        /// Neukládá se do JSONu.
        /// </summary>
        [JsonIgnore]
        public string Vek => VypocetVeku.VypocitejVekTextove(DatumNarozeni);

        /// <summary> Datum přijetí z jiné ZOO, pokud se nejedná o narození v místě. </summary>
        [JsonPropertyName("datumPrijetiZJineZoo")]
        public DateOnly? DatumPrijetiZJineZoo { get; internal set; }

        /// <summary> Kompletní historie zdravotních záznamů zvířete. </summary>
        [JsonPropertyName("zdravotniZaznamy")]
        public List<ZdravotniZaznam> ZdravotniZaznamy { get; internal set; } = [];

        /// <summary> Unikátní identifikátor zvířete. </summary>
        [JsonPropertyName("id")]
        public string Id { get; internal set; } = string.Empty;

        /// <summary>
        /// Konstruktor kompletní instance zvířete.
        /// </summary>
        [JsonConstructor]
        public Zvire(string nazev, DateOnly datumNarozeni, double vaha, DateOnly? datumPrijetiZJineZoo,
            List<ZdravotniZaznam> zdravotniZaznamy, string id)
        {
            Nazev = nazev;
            DatumNarozeni = datumNarozeni;
            Vaha = vaha;
            DatumPrijetiZJineZoo = datumPrijetiZJineZoo;
            ZdravotniZaznamy = zdravotniZaznamy;
            Id = id;
        }
    }
}