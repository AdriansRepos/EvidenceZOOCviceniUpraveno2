using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Enumy;
using InputHelper;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace EvidenceZOOCviceniUpraveno2.Data
{
    /// <summary>
    /// Reprezentuje zaměstnance ZOO včetně osobních, kontaktních a mzdových údajů,
    /// jakož i vazeb na rodinné příslušníky pro účely daňových slev.
    /// </summary>
    public partial class Zamestnanec
    {
        /// <summary> Unikátní identifikátor zaměstnance. </summary>
        [JsonPropertyName("osobniCislo")]
        public string OsobniCislo { get; internal set; } = string.Empty;

        private string _jmeno = string.Empty;

        /// <summary> Jméno zaměstnance s automatickou úpravou velkých písmen. </summary>
        [JsonPropertyName("jmeno")]
        public string Jmeno
        {
            get => _jmeno;
            internal set => _jmeno = TitleCase.ToTitleCase(value); // Převedení na velké počáteční písmeno
        }

        private string _prijmeni = string.Empty;

        /// <summary> Příjmení zaměstnance s automatickou úpravou velkých písmen. </summary>
        [JsonPropertyName("prijmeni")]
        public string Prijmeni
        {
            get => _prijmeni;
            internal set => _prijmeni = TitleCase.ToTitleCase(value); // Převedení na velké počáteční písmeno
        }

        /// <summary> Datum narození zaměstnance. </summary>
        [JsonPropertyName("datumNarozeni")]
        public DateOnly DatumNarozeni { get; internal set; }

        /// <summary> Hrubá měsíční mzda v Kč. </summary>
        [JsonPropertyName("mzda")]
        public int Mzda { get; internal set; }

        private string _pracovniPozice = string.Empty;

        /// <summary> Název pracovní pozice v ZOO. </summary>
        [JsonPropertyName("pracovniPozice")]
        public string PracovniPozice
        {
            get => _pracovniPozice;
            internal set => _pracovniPozice = TitleCase.ToTitleCase(value);
        }

        private string _mesto = string.Empty;

        /// <summary> Město trvalého bydliště s ošetřením speciálních násobnostních názvů. </summary>
        [JsonPropertyName("mesto")]
        public string Mesto
        {
            get => _mesto;
            internal set => _mesto = MestoTitleCase.ZpracujNazevMesta(value); // Pokročilá úprava složených názvů měst
        }

        /// <summary> Ulice a číslo popisné trvalého bydliště. </summary>
        [JsonPropertyName("ulice")]
        public string Ulice { get; internal set; } = string.Empty;

        private string _psc = string.Empty;

        /// <summary> Poštovní směrovací číslo normalizované do formátu XXX XX. </summary>
        [JsonPropertyName("psc")]
        public string PSC
        {
            get => _psc;
            internal set => _psc = NormalizujPsc(value);
        }

        /// <summary> Kontaktní telefonní číslo. </summary>
        [JsonPropertyName("telefon")]
        public string Telefon { get; internal set; } = string.Empty;

        /// <summary> Kontaktní emailová adresa. </summary>
        [JsonPropertyName("email")]
        public string Email { get; internal set; } = string.Empty;

        /// <summary> Rodinný stav zaměstnance. </summary>
        [JsonPropertyName("rodinnyStav")]
        public RodinnyStav RodinnyStav { get; internal set; }

        /// <summary> Zdravotní stav pro posouzení nároku na slevy na invaliditu. </summary>
        [JsonPropertyName("zdravotniStav")]
        public ZdravotniStav ZdravotniStav { get; internal set; }

        /// <summary> Typ předloženého dokladu totožnosti (např. OP, Pas). </summary>
        [JsonPropertyName("typDokladu")]
        public string TypDokladu { get; internal set; } = string.Empty;

        /// <summary> Číslo dokladu totožnosti. </summary>
        [JsonPropertyName("cisloDokladu")]
        public string CisloDokladu { get; internal set; } = string.Empty;

        private string _rodneCislo = string.Empty;

        /// <summary> Rodné číslo ověřované vůči standardním pravidlům validace. </summary>
        [JsonPropertyName("rodneCislo")]
        public string RodneCislo
        {
            get => _rodneCislo;
            internal set => _rodneCislo = InputHelper.RodneCislo.Zkontroluj(value);
        }

        /// <summary> Seznam nezaopatřených dětí pro uplatnění daňového zvýhodnění. </summary>
        [JsonPropertyName("deti")]
        public List<Dite> Deti { get; internal set; } = [];

        /// <summary> Partner/manžel(ka) zaměstnance pro uplatnění daňové slevy. </summary>
        [JsonPropertyName("partner")]
        public Partner? Partner { get; internal set; }

        /// <summary>
        /// Konstruktor kompletního záznamu zaměstnance pro serializaci i vytváření nového subjektu.
        /// </summary>
        [JsonConstructor]
        public Zamestnanec(string osobniCislo, string jmeno, string prijmeni, DateOnly datumNarozeni,
            string rodneCislo, int mzda, string pracovniPozice, string mesto, string ulice, string psc,
            string telefon, string email, RodinnyStav rodinnyStav, ZdravotniStav zdravotniStav,
            string typDokladu, string cisloDokladu, List<Dite> deti, Partner? partner)
        {
            OsobniCislo = osobniCislo;
            Jmeno = jmeno;
            Prijmeni = prijmeni;
            DatumNarozeni = datumNarozeni;
            RodneCislo = rodneCislo;
            Mzda = mzda;
            PracovniPozice = pracovniPozice;
            Mesto = mesto;
            Ulice = ulice;
            PSC = psc;
            Telefon = telefon;
            Email = email;
            RodinnyStav = rodinnyStav;
            ZdravotniStav = zdravotniStav;
            TypDokladu = typDokladu;
            CisloDokladu = cisloDokladu;
            Deti = deti;
            Partner = partner;
        }

        /// <summary> Úprava PSČ na jednotný tvar 5 číslic s mezerou uprostřed. </summary>
        private static string NormalizujPsc(string vstup)
        {
            // Extrakce pouze číselných znaků ze vstupu[cite: 18]
            string cisliceJen = new([.. vstup.Where(char.IsDigit)]);

            // Vloží mezeru za 3. číslici, pokud řetězec obsahuje standardních 5 čísel
            if (cisliceJen.Length == 5)
                return $"{cisliceJen[..3]} {cisliceJen[3..]}";

            return vstup.Trim();
        }

        /// <summary> Ověří, zda má předaný řetězec validní syntaktický tvar emailu. </summary>
        public static bool JePlatnyEmail(string email)
        {
            return EmailRegex().IsMatch(email);
        }

        /// <summary> Kompilovaný regulární výraz pro ověření e-mailu. </summary>
        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
    }
}