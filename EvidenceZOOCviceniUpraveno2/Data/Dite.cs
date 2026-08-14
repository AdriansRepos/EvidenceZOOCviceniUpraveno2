using System.Text.Json.Serialization;
using InputHelper;

namespace EvidenceZOOCviceniUpraveno2.Data
{
    /// <summary>
    /// Reprezentuje dítě zaměstnance – evidováno kvůli uplatnění
    /// daňového bonusu a evidenci ZTP/P pro mzdové účely.
    /// </summary>
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

        private string _rodneCislo = string.Empty;
        [JsonPropertyName("rodneCislo")]
        public string RodneCislo
        {
            get => _rodneCislo;
            internal set => _rodneCislo = InputHelper.RodneCislo.Zkontroluj(value);
        }

        /// <summary>
        /// Držitel průkazu ZTP/P – jediná zdravotní informace relevantní
        /// pro mzdové účely u dítěte (automaticky implikuje invaliditu).
        /// </summary>
        [JsonPropertyName("jeDrzitelZtpP")]
        public bool JeDrzitelZtpP { get; internal set; }

        [JsonPropertyName("uplatnenBonus")]
        public bool UplatnenBonus { get; internal set; }

        [JsonConstructor]
        public Dite(string jmeno, string prijmeni, DateOnly datumNarozeni,
            string rodneCislo, bool jeDrzitelZtpP, bool uplatnenBonus)
        {
            Jmeno = jmeno;
            Prijmeni = prijmeni;
            DatumNarozeni = datumNarozeni;
            RodneCislo = rodneCislo;
            JeDrzitelZtpP = jeDrzitelZtpP;
            UplatnenBonus = uplatnenBonus;
        }
    }
}