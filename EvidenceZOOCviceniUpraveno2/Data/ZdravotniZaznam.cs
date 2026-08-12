using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2.Data
{
    /// <summary>
    /// Jeden zdravotní záznam zvířete (nemoc, léčba, podaný lék, kontrola).
    /// </summary>
    [method: JsonConstructor]   
    class ZdravotniZaznam(DateOnly datum, string popis)
    {
        [JsonPropertyName("datum")]
        public DateOnly Datum { get; internal set; } = datum;

        [JsonPropertyName("popis")]
        public string Popis { get; internal set; } = popis;
    }
}