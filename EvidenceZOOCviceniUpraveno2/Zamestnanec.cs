using System.Text.Json.Serialization;
using InputHelper;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje jednoho zaměstnance zoologické zahrady.
    /// Uchovává jeho jméno, příjmení, datum narození, mzdu a pracovní pozici.
    /// </summary>
    class Zamestnanec
    {
        /// <summary>
        /// Křestní jméno zaměstnance. Nikdy nesmí být null.
        /// Automaticky převáděno do formátu TitleCase.
        /// </summary>
        private string _jmeno = string.Empty;
        [JsonPropertyName("jmeno")]
        public string Jmeno
        {
            get => _jmeno;
            internal set => _jmeno = TitleCase.ToTitleCase(value);
        }

        /// <summary>
        /// Příjmení zaměstnance. Automaticky převáděno do TitleCase.
        /// </summary>
        private string _prijmeni = string.Empty;
        [JsonPropertyName("prijmeni")]
        public string Prijmeni
        {
            get => _prijmeni;
            internal set => _prijmeni = TitleCase.ToTitleCase(value);
        }

        /// <summary>
        /// Datum narození zaměstnance.
        /// </summary>
        [JsonPropertyName("datumNarozeni")]
        public DateOnly DatumNarozeni { get; internal set; }

        /// <summary>
        /// Mzda zaměstnance v českých korunách.
        /// </summary>
        [JsonPropertyName("mzda")]
        public int Mzda { get; internal set; }

        /// <summary>
        /// Pracovní pozice zaměstnance. Automaticky převáděna do TitleCase.
        /// </summary>
        private string _pracovniPozice = string.Empty;
        [JsonPropertyName("pracovniPozice")]
        public string PracovniPozice
        {
            get => _pracovniPozice;
            internal set => _pracovniPozice = TitleCase.ToTitleCase(value);
        }

        /// <summary>
        /// Vytvoří nového zaměstnance a nastaví všechny jeho vlastnosti.
        /// </summary>
        /// <param name="jmeno">Křestní jméno zaměstnance.</param>
        /// <param name="prijmeni">Příjmení zaměstnance.</param>
        /// <param name="datumNarozeni">Datum narození.</param>
        /// <param name="mzda">Mzda v Kč.</param>
        /// <param name="pracovniPozice">Pracovní pozice.</param>
        [JsonConstructor]
        public Zamestnanec(string jmeno, string prijmeni, DateOnly datumNarozeni,
                       int mzda, string pracovniPozice)
        {
            Jmeno = jmeno;
            Prijmeni = prijmeni;
            DatumNarozeni = datumNarozeni;
            Mzda = mzda;
            PracovniPozice = pracovniPozice;
        }

        /// <summary>
        /// Vypíše všechny informace o zaměstnanci do konzole.
        /// </summary>
        public void VypisZamestnance()
        {
            Console.WriteLine(
                $"{Jmeno,-15} {Prijmeni,-15} {DatumNarozeni,-15} {Mzda,-10} {PracovniPozice,-20}"
            );
        }        
    }
}