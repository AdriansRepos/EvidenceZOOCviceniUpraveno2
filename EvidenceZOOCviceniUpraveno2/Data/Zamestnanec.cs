using EvidenceZOOCviceniUpraveno2.Enumy;
using InputHelper;
using PohybHelper;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace EvidenceZOOCviceniUpraveno2.Data
{
    public partial class Zamestnanec
    {
        [JsonPropertyName("osobniCislo")]
        public string OsobniCislo { get; internal set; } = string.Empty;

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

        [JsonPropertyName("datumNarozeni")]
        public DateOnly DatumNarozeni { get; internal set; }

        [JsonPropertyName("mzda")]
        public int Mzda { get; internal set; }

        private string _pracovniPozice = string.Empty;
        [JsonPropertyName("pracovniPozice")]
        public string PracovniPozice
        {
            get => _pracovniPozice;
            internal set => _pracovniPozice = TitleCase.ToTitleCase(value);
        }

        private string _mesto = string.Empty;
        [JsonPropertyName("mesto")]
        public string Mesto
        {
            get => _mesto;
            internal set => _mesto = MestoTitleCase.ZpracujNazevMesta(value);
        }

        [JsonPropertyName("ulice")]
        public string Ulice { get; internal set; } = string.Empty;

        private string _psc = string.Empty;
        [JsonPropertyName("psc")]
        public string PSC
        {
            get => _psc;
            internal set => _psc = NormalizujPsc(value);
        }

        [JsonPropertyName("telefon")]
        public string Telefon { get; internal set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; internal set; } = string.Empty;

        [JsonPropertyName("rodinnyStav")]
        public RodinnyStav RodinnyStav { get; internal set; }

        [JsonPropertyName("zdravotniStav")]
        public ZdravotniStav ZdravotniStav { get; internal set; }

        [JsonPropertyName("typDokladu")]
        public string TypDokladu { get; internal set; } = string.Empty;

        [JsonPropertyName("cisloDokladu")]
        public string CisloDokladu { get; internal set; } = string.Empty;

        private string _rodneCislo = string.Empty;
        [JsonPropertyName("rodneCislo")]
        public string RodneCislo
        {
            get => _rodneCislo;
            internal set => _rodneCislo = InputHelper.RodneCislo.Zkontroluj(value);
        }

        [JsonPropertyName("deti")]
        public List<Dite> Deti { get; internal set; } = [];

        [JsonConstructor]
        public Zamestnanec(string osobniCislo, string jmeno, string prijmeni, DateOnly datumNarozeni,
            string rodneCislo, int mzda, string pracovniPozice, string mesto, string ulice, string psc,
            string telefon, string email, RodinnyStav rodinnyStav, ZdravotniStav zdravotniStav,
            string typDokladu, string cisloDokladu, List<Dite> deti)
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
        }

        private static string NormalizujPsc(string vstup)
        {
            string cisliceJen = new([.. vstup.Where(char.IsDigit)]);

            if (cisliceJen.Length == 5)
                return $"{cisliceJen[..3]} {cisliceJen[3..]}";

            return vstup.Trim();
        }

        public static bool JePlatnyEmail(string email)
        {
            return EmailRegex().IsMatch(email);
        }

        public void VypisZamestnance()
        {
            Console.WriteLine(
                $"{OsobniCislo,-12} {Jmeno,-15} {Prijmeni,-15} {DatumNarozeni,-15} {Mzda,-10} {PracovniPozice,-20}"
            );
        }

        public void VypisKontaktniUdaje()
        {
            Console.WriteLine($"Adresa:            {Ulice}, {PSC} {Mesto}");
            Console.WriteLine($"Telefon:           {Telefon}");
            Console.WriteLine($"E-mail:            {Email}");
            Console.WriteLine($"Rodinný stav:      {PopiskyHelper.ZiskejPopisek(RodinnyStav)}");
            Console.WriteLine($"Zdravotní stav:    {PopiskyHelper.ZiskejPopisek(ZdravotniStav)}");
            Console.WriteLine($"Doklad totožnosti: {TypDokladu} {CisloDokladu}");

            if (Deti.Count > 0)
            {
                Console.WriteLine("Děti:");
                foreach (Dite dite in Deti)
                {
                    string bonus = dite.UplatnenBonus ? " (uplatněn bonus)" : "";
                    string ztpP = dite.JeDrzitelZtpP ? " [ZTP/P]" : "";
                    Console.WriteLine($"  - {dite.Jmeno} {dite.Prijmeni}, nar. {dite.DatumNarozeni:d}, RČ: {dite.RodneCislo}{ztpP}{bonus}");
                }
            }
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
    }
}