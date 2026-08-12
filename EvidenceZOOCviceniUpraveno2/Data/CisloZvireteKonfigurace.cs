using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2.Data
{
    /// <summary>
    /// Konfigurace generování unikátních identifikátorů zvířat.
    /// </summary>
    class CisloZvireteKonfigurace
    {
        [JsonPropertyName("posledniCislo")]
        public long PosledniCislo { get; set; } = 0;

        public string DalsiId()
        {
            PosledniCislo++;
            return PosledniCislo.ToString().PadLeft(3, '0');
        }
    }
}