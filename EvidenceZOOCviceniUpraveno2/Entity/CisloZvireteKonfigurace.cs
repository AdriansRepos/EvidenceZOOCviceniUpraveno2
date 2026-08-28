using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    /// <summary>
    /// Konfigurace generování unikátních identifikátorů zvířat.
    /// </summary>
    class CisloZvireteKonfigurace
    {
        /// <summary> Poslední přiřazený unikátní identifikátor zvířete. </summary>
        [JsonPropertyName("posledniCislo")]
        public long PosledniCislo { get; set; } = 0;

        /// <summary>
        /// Zvýší interní čítač a vrátí nový identifikátor zarovnaný na 3 číslice.
        /// </summary>
        public string DalsiId()
        {
            // Inkrementace číselníku zvířat
            PosledniCislo++;
            // Formátování identifikátoru zarovnaného nulami na minimálně 3 místa
            return PosledniCislo.ToString().PadLeft(3, '0');
        }
    }
}