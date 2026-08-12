using System.Text.Json.Serialization;
using InputHelper;

namespace EvidenceZOOCviceniUpraveno2.Data
{
    public class Dite
    {
        private string _jmeno = string.Empty;
        [JsonPropertyName("jmeno")]
        public string Jmeno
        {
            get => _jmeno;
            internal set => _jmeno = TitleCase.ToTitleCase(value);
        }

        private string _prijmeni = string.Empty;
        [JsonPropertyName("prijmeni")]
        public string Prijmeni
        {
            get => _prijmeni;
            internal set => _prijmeni = TitleCase.ToTitleCase(value);
        }

        [JsonPropertyName("datumNarozeni")]
        public DateOnly DatumNarozeni { get; internal set; }

        [JsonPropertyName("adresa")]
        public string Adresa { get; internal set; } = string.Empty;

        [JsonPropertyName("skola")]
        public string Skola { get; internal set; } = string.Empty;

        [JsonPropertyName("invalidita")]
        public bool Invalidita { get; internal set; }

        [JsonPropertyName("uplatnenBonus")]
        public bool UplatnenBonus { get; internal set; }

        [JsonConstructor]
        public Dite(string jmeno, string prijmeni, DateOnly datumNarozeni,
            string adresa, string skola, bool invalidita, bool uplatnenBonus)
        {
            Jmeno = jmeno;
            Prijmeni = prijmeni;
            DatumNarozeni = datumNarozeni;
            Adresa = adresa;
            Skola = skola;
            Invalidita = invalidita;
            UplatnenBonus = uplatnenBonus;
        }
    }
}