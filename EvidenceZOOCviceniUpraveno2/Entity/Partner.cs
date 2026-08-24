using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    public class Partner
    {
        [JsonPropertyName("jmeno")]
        public string? Jmeno { get; internal set; }
    
        [JsonPropertyName("prijmeni")]
        public string? Prijmeni { get; internal set; }
    
        [JsonPropertyName("rodneCislo")]
        public string? RodneCislo { get; internal set; }
    
        [JsonPropertyName("jeDrzitelZtpP")]
        public bool JeDrzitelZtpP { get; internal set; }
    
        [JsonPropertyName("pecujeODiteDo3Let")]
        public bool PecujeODiteDo3Let { get; internal set; }
    
        [JsonPropertyName("prijemDo68Tisic")]
        public bool PrijemDo68Tisic { get; internal set; }
    
        [JsonPropertyName("uplatnitSlevu")]
        public bool UplatnitSlevu { get; internal set; }
    }
}
