using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Jeden zdravotní záznam zvířete (nemoc, léčba, podaný lék, kontrola).
    /// </summary>
    class ZdravotniZaznam
    {
        [JsonPropertyName("datum")]
        public DateOnly Datum { get; internal set; }

        [JsonPropertyName("popis")]
        public string Popis { get; internal set; } = string.Empty;

        [JsonConstructor]
        public ZdravotniZaznam(DateOnly datum, string popis)
        {
            Datum = datum;
            Popis = popis;
        }
    }
}