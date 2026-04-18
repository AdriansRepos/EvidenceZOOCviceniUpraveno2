
namespace EvidenceZOOCviceniUpraveno2
{

    class Zamestnanec
    {
        // Křestní jméno zaměstnance – nikdy nesmí být null
        public string Jmeno { get; private set; } = "";

        // Příjmení zaměstnance
        public string Prijmeni { get; private set; } = "";

        // Datum narození zaměstnance
        public DateOnly DatumNarozeni { get; private set; }

        // Mzda zaměstnance v Kč
        public int Mzda { get; private set; }

        // Pracovní pozice zaměstnance
        public string PracovniPozice { get; private set; } = "";

        // Konstruktor – nastaví všechny vlastnosti pomocí interních metod
        public Zamestnanec(string jmeno, string prijmeni, DateOnly datumNarozeni,
                           int mzda, string pracovniPozice)
        {
            NastavJmeno(jmeno);
            NastavPrijmeni(prijmeni);
            NastavDatumNarozeni(datumNarozeni);
            NastavMzdu(mzda);
            NastavPracovniPozici(pracovniPozice);
        }

        // Nastaví jméno a převede ho do TitleCase
        internal void NastavJmeno(string noveJmeno)
        {
            Jmeno = Vstupy.ToTitleCase(noveJmeno);
        }

        // Nastaví příjmení a převede ho do TitleCase
        internal void NastavPrijmeni(string novePrijmeni)
        {
            Prijmeni = Vstupy.ToTitleCase(novePrijmeni);
        }

        // Nastaví datum narození
        internal void NastavDatumNarozeni(DateOnly noveDatumNarozeni)
        {
            DatumNarozeni = noveDatumNarozeni;
        }

        // Nastaví mzdu
        internal void NastavMzdu(int novaMzda)
        {
            Mzda = novaMzda;
        }

        // Nastaví pracovní pozici a převede ji do TitleCase
        internal void NastavPracovniPozici(string novaPracovniPozice)
        {
            PracovniPozice = Vstupy.ToTitleCase(novaPracovniPozice);
        }

        // Vypíše všechny informace o zaměstnanci
        public void VypisZamestnance()
        {
            Console.WriteLine("Jméno zaměstnance: {0}", Jmeno);
            Console.WriteLine("\tPříjmení zaměstnance: {0}", Prijmeni);
            Console.WriteLine("\tDatum narození zaměstnance: {0}", DatumNarozeni);
            Console.WriteLine("\tMzda zaměstnance: {0}", Mzda);
            Console.WriteLine("\tPracovní pozice zaměstnance: {0}", PracovniPozice);
        }

        // Převede objekt na řetězec pro uložení do souboru
        public string ToFileString()
        {
            return $"{Jmeno}|{Prijmeni}|{DatumNarozeni}|{Mzda}|{PracovniPozice}";
        }

        // Vytvoří objekt Zamestnanec z jednoho řádku textu v souboru
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
