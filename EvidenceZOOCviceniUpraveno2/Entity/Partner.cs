using InputHelper;
using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    /// <summary>
    /// Reprezentuje partnera/manžela zaměstnance pro účely uplatnění daňových slev.
    /// </summary>
    public class Partner
    {
        private string _jmeno = string.Empty;

        /// <summary> Jméno partnera automaticky převedené na TitleCase. </summary>
        [JsonPropertyName("jmeno")]
        public string Jmeno
        {
            get => _jmeno;
            internal set => _jmeno = TitleCase.ToTitleCase(value); // Automatická úprava velikosti písmen
        }

        private string _prijmeni = string.Empty;

        /// <summary> Příjmení partnera automaticky převedené na TitleCase. </summary>
        [JsonPropertyName("prijmeni")]
        public string Prijmeni
        {
            get => _prijmeni;
            internal set => _prijmeni = TitleCase.ToTitleCase(value); // Automatická úprava velikosti písmen
        }

        private string _rodneCislo = string.Empty;

        /// <summary> Rodné číslo partnera podléhající validaci. </summary>
        [JsonPropertyName("rodneCislo")]
        public string RodneCislo
        {
            get => _rodneCislo;
            internal set => _rodneCislo = InputHelper.RodneCislo.Zkontroluj(value); // Validace formátu rodného čísla
        }

        /// <summary> Příznak držitelství průkazu ZTP/P partnera. </summary>
        [JsonPropertyName("jeDrzitelZtpP")]
        public bool JeDrzitelZtpP { get; internal set; }

        /// <summary> Zda partner pečuje o dítě do 3 let věku. </summary>
        [JsonPropertyName("pecujeODiteDo3Let")]
        public bool PecujeODiteDo3Let { get; internal set; }

        /// <summary> Splnění podmínky vlastního příjmu partnera do 68 000 Kč/rok pro daňovou slevu. </summary>
        [JsonPropertyName("prijemDo68Tisic")]
        public bool PrijemDo68Tisic { get; internal set; }

        /// <summary> Indikuje, zda zaměstnanec uplatňuje slevu na manželku/manžela. </summary>
        [JsonPropertyName("uplatnitSlevu")]
        public bool UplatnitSlevu { get; internal set; }

        /// <summary>
        /// Vytvoří novou instanci partnera se všemi daňově relevantními vlastnostmi.
        /// </summary>
        [JsonConstructor]
        public Partner(string jmeno, string prijmeni, string rodneCislo, 
                       bool jeDrzitelZtpP, bool pecujeODiteDo3Let, bool prijemDo68Tisic, bool uplatnitSlevu)
        {
            Jmeno = jmeno;
            Prijmeni = prijmeni;
            RodneCislo = rodneCislo;
            JeDrzitelZtpP = jeDrzitelZtpP;
            PecujeODiteDo3Let = pecujeODiteDo3Let;
            PrijemDo68Tisic = prijemDo68Tisic;
            UplatnitSlevu = uplatnitSlevu;
        }
    }
}