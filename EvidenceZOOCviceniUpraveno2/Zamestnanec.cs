
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
        public string Jmeno { get; private set; } = "";

        /// <summary>
        /// Příjmení zaměstnance. Automaticky převáděno do TitleCase.
        /// </summary>
        public string Prijmeni { get; private set; } = "";

        /// <summary>
        /// Datum narození zaměstnance.
        /// </summary>
        public DateOnly DatumNarozeni { get; private set; }

        /// <summary>
        /// Mzda zaměstnance v českých korunách.
        /// </summary>
        public int Mzda { get; private set; }

        /// <summary>
        /// Pracovní pozice zaměstnance. Automaticky převáděna do TitleCase.
        /// </summary>
        public string PracovniPozice { get; private set; } = "";

        /// <summary>
        /// Vytvoří nového zaměstnance a nastaví všechny jeho vlastnosti.
        /// </summary>
        /// <param name="jmeno">Křestní jméno zaměstnance.</param>
        /// <param name="prijmeni">Příjmení zaměstnance.</param>
        /// <param name="datumNarozeni">Datum narození.</param>
        /// <param name="mzda">Mzda v Kč.</param>
        /// <param name="pracovniPozice">Pracovní pozice.</param>
        public Zamestnanec(string jmeno, string prijmeni, DateOnly datumNarozeni,
                           int mzda, string pracovniPozice)
        {
            NastavJmeno(jmeno);
            NastavPrijmeni(prijmeni);
            NastavDatumNarozeni(datumNarozeni);
            NastavMzdu(mzda);
            NastavPracovniPozici(pracovniPozice);
        }

        /// <summary>
        /// Nastaví jméno zaměstnance a převede jej do TitleCase.
        /// </summary>
        /// <param name="noveJmeno">Nové jméno.</param>
        internal void NastavJmeno(string noveJmeno)
        {
            Jmeno = Vstupy.ToTitleCase(noveJmeno);
        }

        /// <summary>
        /// Nastaví příjmení zaměstnance a převede jej do TitleCase.
        /// </summary>
        /// <param name="novePrijmeni">Nové příjmení.</param>
        internal void NastavPrijmeni(string novePrijmeni)
        {
            Prijmeni = Vstupy.ToTitleCase(novePrijmeni);
        }

        /// <summary>
        /// Nastaví datum narození zaměstnance.
        /// </summary>
        /// <param name="noveDatumNarozeni">Nové datum narození.</param>
        internal void NastavDatumNarozeni(DateOnly noveDatumNarozeni)
        {
            DatumNarozeni = noveDatumNarozeni;
        }

        /// <summary>
        /// Nastaví mzdu zaměstnance.
        /// </summary>
        /// <param name="novaMzda">Nová mzda v Kč.</param>
        internal void NastavMzdu(int novaMzda)
        {
            Mzda = novaMzda;
        }

        /// <summary>
        /// Nastaví pracovní pozici zaměstnance a převede ji do TitleCase.
        /// </summary>
        /// <param name="novaPracovniPozice">Nová pracovní pozice.</param>
        internal void NastavPracovniPozici(string novaPracovniPozice)
        {
            PracovniPozice = Vstupy.ToTitleCase(novaPracovniPozice);
        }

        /// <summary>
        /// Vypíše všechny informace o zaměstnanci do konzole.
        /// </summary>
        public void VypisZamestnance()
        {
            Console.WriteLine("Jméno zaměstnance: {0}", Jmeno);
            Console.WriteLine("\tPříjmení zaměstnance: {0}", Prijmeni);
            Console.WriteLine("\tDatum narození zaměstnance: {0}", DatumNarozeni);
            Console.WriteLine("\tMzda zaměstnance: {0}", Mzda);
            Console.WriteLine("\tPracovní pozice zaměstnance: {0}", PracovniPozice);
        }

        /// <summary>
        /// Převede zaměstnance na řetězec vhodný pro uložení do souboru.
        /// </summary>
        /// <returns>Řetězec ve formátu "Jmeno|Prijmeni|DatumNarozeni|Mzda|PracovniPozice".</returns>
        public string ToFileString()
        {
            return $"{Jmeno}|{Prijmeni}|{DatumNarozeni}|{Mzda}|{PracovniPozice}";
        }

        /// <summary>
        /// Vytvoří objekt <see cref="Zamestnanec"/> z jednoho řádku textu v souboru.
        /// </summary>
        /// <param name="line">Řádek textu obsahující údaje o zaměstnanci.</param>
        /// <returns>Nově vytvořený objekt zaměstnance.</returns>
        /// <exception cref="FormatException">
        /// Vyvolána, pokud řádek nemá správný formát nebo obsahuje neplatná data.
        /// </exception>
        public static Zamestnanec Parse(string line)
        {
            // Rozdělení řádku podle svislé čáry
            string[] parts = line.Split('|');

            // Kontrola správného počtu položek
            if (parts.Length != 5)
                throw new FormatException("Řádek nemá správný formát pro Zamestnance.");

            // Kontrola jména
            if (string.IsNullOrWhiteSpace(parts[0]))
                throw new FormatException("Jméno nesmí být prázdné.");
            string jmeno = parts[0];

            // Kontrola příjmení
            if (string.IsNullOrWhiteSpace(parts[1]))
                throw new FormatException("Příjmení nesmí být prázdné.");
            string prijmeni = parts[1];

            // Kontrola a převod data narození
            if (!DateOnly.TryParse(parts[2], out DateOnly datumNarozeni))
                throw new FormatException("Datum narození není platné.");

            // Kontrola a převod mzdy
            if (!int.TryParse(parts[3], out int mzda))
                throw new FormatException("Mzda není platné číslo.");

            // Kontrola pracovní pozice
            if (string.IsNullOrWhiteSpace(parts[4]))
                throw new FormatException("Pracovní pozice nesmí být prázdná.");
            string pracovniPozice = parts[4];

            // Vytvoření nového objektu
            return new Zamestnanec(jmeno, prijmeni, datumNarozeni, mzda, pracovniPozice);
        }
    }
}
