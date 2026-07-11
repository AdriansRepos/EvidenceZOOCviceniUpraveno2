using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using InputHelper;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje jednoho zaměstnance zoologické zahrady.
    /// Uchovává jeho jméno, příjmení, datum narození, mzdu, pracovní
    /// pozici a kontaktní/adresní údaje.
    /// </summary>
    partial class Zamestnanec
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
        /// Město trvalého bydliště. Automaticky převáděno do formátu
        /// TitleCase s ohledem na české předložky v názvu (např. "Ústí nad Labem").
        /// </summary>
        private string _mesto = string.Empty;
        [JsonPropertyName("mesto")]
        public string Mesto
        {
            get => _mesto;
            internal set => _mesto = MestoTitleCase.ZpracujNazevMesta(value);
        }

        /// <summary>
        /// Ulice a popisné/orientační číslo (např. "Hlavní 123", "Nová 45/2").
        /// Slouží pouze jako informační údaj, nijak dál nezpracovávaný.
        /// </summary>
        [JsonPropertyName("ulice")]
        public string Ulice { get; internal set; } = string.Empty;

        /// <summary>
        /// Poštovní směrovací číslo ve formátu "12345" nebo "123 45".
        /// Slouží pouze jako informační údaj.
        /// </summary>
        private string _psc = string.Empty;
        [JsonPropertyName("psc")]
        public string PSC
        {
            get => _psc;
            internal set => _psc = NormalizujPsc(value);
        }

        /// <summary>
        /// Telefonní číslo zaměstnance. Slouží pouze jako informační
        /// údaj, nijak dál nezpracovávaný.
        /// </summary>
        [JsonPropertyName("telefon")]
        public string Telefon { get; internal set; } = string.Empty;

        /// <summary>
        /// E-mailová adresa zaměstnance.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; internal set; } = string.Empty;

        /// <summary>
        /// Vytvoří nového zaměstnance a nastaví všechny jeho vlastnosti.
        /// </summary>
        [JsonConstructor]
        public Zamestnanec(string jmeno, string prijmeni, DateOnly datumNarozeni,
            int mzda, string pracovniPozice, string mesto, string ulice,
            string psc, string telefon, string email)
        {
            Jmeno = jmeno;
            Prijmeni = prijmeni;
            DatumNarozeni = datumNarozeni;
            Mzda = mzda;
            PracovniPozice = pracovniPozice;
            Mesto = mesto;
            Ulice = ulice;
            PSC = psc;
            Telefon = telefon;
            Email = email;
        }

        /// <summary>
        /// Převede PSČ na jednotný formát "123 45" (mezera po třetí číslici),
        /// pokud vstup obsahuje přesně 5 číslic. Jinak vrátí ořezaný
        /// vstup beze změny.
        /// </summary>
        private static string NormalizujPsc(string vstup)
        {
            string cisliceJen = new([.. vstup.Where(char.IsDigit)]);

            if (cisliceJen.Length == 5)
                return $"{cisliceJen[..3]} {cisliceJen[3..]}";

            return vstup.Trim();
        }

        /// <summary>
        /// Ověří, zda text vypadá jako platná e-mailová adresa
        /// (základní formátová kontrola, ne ověření existence).
        /// </summary>
        public static bool JePlatnyEmail(string email)
        {
            return EmailRegex().IsMatch(email);
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

        /// <summary>
        /// Vypíše kompletní kontaktní a adresní údaje zaměstnance.
        /// </summary>
        public void VypisKontaktniUdaje()
        {
            Console.WriteLine($"Adresa:  {Ulice}, {PSC} {Mesto}");
            Console.WriteLine($"Telefon: {Telefon}");
            Console.WriteLine($"E-mail:  {Email}");
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
    }
}