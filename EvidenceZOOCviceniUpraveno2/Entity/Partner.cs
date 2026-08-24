using InputHelper;
using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    public class Partner
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
    
        private string _rodneCislo = string.Empty;
        [JsonPropertyName("rodneCislo")]
        public string RodneCislo
        {
            get => _rodneCislo;
            internal set => _rodneCislo = InputHelper.RodneCislo.Zkontroluj(value);
        }
    
        [JsonPropertyName("jeDrzitelZtpP")]
        public bool JeDrzitelZtpP { get; internal set; }
    
        [JsonPropertyName("pecujeODiteDo3Let")]
        public bool PecujeODiteDo3Let { get; internal set; }
    
        [JsonPropertyName("prijemDo68Tisic")]
        public bool PrijemDo68Tisic { get; internal set; }
    
        [JsonPropertyName("uplatnitSlevu")]
        public bool UplatnitSlevu { get; internal set; }
    
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
