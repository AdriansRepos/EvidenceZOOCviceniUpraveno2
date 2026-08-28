using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    /// <summary>
    /// Jeden zdravotní záznam zvířete (nemoc, léčba, podaný lék, kontrola).
    /// </summary>
    [method: JsonConstructor]   
    class ZdravotniZaznam(DateOnly datum, string popis)
    {
        /// <summary> Datum vyhotovení zdravotního záznamu / zákroku. </summary>
        [JsonPropertyName("datum")]
        public DateOnly Datum { get; internal set; } = datum;

        /// <summary> Detailní popis zdravotního stavu, diagnozy nebo léčby. </summary>
        [JsonPropertyName("popis")]
        public string Popis { get; internal set; } = popis;
    }
}