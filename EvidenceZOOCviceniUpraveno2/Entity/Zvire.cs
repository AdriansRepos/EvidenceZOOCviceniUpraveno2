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
        [JsonPropertyName("nazev")]
        public string Nazev
        {
            get => _nazev;
            internal set => _nazev = TitleCase.ToTitleCase(value);
        }

        [JsonPropertyName("datumNarozeni")]
        public DateOnly DatumNarozeni { get; internal set; }

        [JsonPropertyName("vaha")]
        public double Vaha { get; internal set; }

        [JsonIgnore]
        public string Vek => VypocetVeku.VypocitejVekTextove(DatumNarozeni);

        [JsonPropertyName("datumPrijetiZJineZoo")]
        public DateOnly? DatumPrijetiZJineZoo { get; internal set; }

        [JsonPropertyName("zdravotniZaznamy")]
        public List<ZdravotniZaznam> ZdravotniZaznamy { get; internal set; } = [];

        [JsonPropertyName("id")]
        public string Id { get; internal set; } = string.Empty;

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