using System.Text.Json.Serialization;
using InputHelper;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    /// <summary>
    /// Reprezentuje dítě zaměstnance – evidováno kvůli uplatnění
    /// daňového bonusu a evidenci ZTP/P pro mzdové účely.
    /// </summary>
    public class Dite
    {
        private string _jmeno = string.Empty;

        /// <summary> Jméno dítěte automaticky převedené na TitleCase. </summary>
        [JsonPropertyName("jmeno")]
        public string Jmeno
        {
            get => _jmeno;
            internal set => _jmeno = TitleCase.ToTitleCase(value); // Automatická úprava velikosti písmen
        }

        private string _prijmeni = string.Empty;

        /// <summary> Příjmení dítěte automaticky převedené na TitleCase. </summary>
        [JsonPropertyName("prijmeni")]
        public string Prijmeni
        {
            get => _prijmeni;
            internal set => _prijmeni = TitleCase.ToTitleCase(value); // Automatická úprava velikosti písmen
        }

        /// <summary> Datum narození dítěte. </summary>
        [JsonPropertyName("datumNarozeni")]
        public DateOnly DatumNarozeni { get; internal set; }

        private string _rodneCislo = string.Empty;

        /// <summary> Rodné číslo dítěte podléhající automatické validaci. </summary>
        [JsonPropertyName("rodneCislo")]
        public string RodneCislo
        {
            get => _rodneCislo;
            internal set => _rodneCislo = InputHelper.RodneCislo.Zkontroluj(value); // Validace správnosti formátu rodného čísla
        }

        /// <summary>
        /// Držitel průkazu ZTP/P – jediná zdravotní informace relevantní
        /// pro mzdové účely u dítěte (automaticky implikuje invaliditu).
        /// </summary>
        [JsonPropertyName("jeDrzitelZtpP")]
        public bool JeDrzitelZtpP { get; internal set; }

        /// <summary> Příznak, zda je na toto dítě uplatňován daňový bonus na děti. </summary>
        [JsonPropertyName("uplatnenBonus")]
        public bool UplatnenBonus { get; internal set; }

        /// <summary>
        /// Vytvoří novou instanci dítěte se všemi mzdově relevantními parametry.
        /// </summary>
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