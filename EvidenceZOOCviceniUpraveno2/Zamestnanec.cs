
namespace EvidenceZOOCviceniUpraveno2
{
    
    class Zamestnanec
    {        
        public string Jmeno { get; private set; } = "";
                
        public string Prijmeni { get; private set; } = "";

        public DateOnly DatumNarozeni { get; private set; }
                
        public int Mzda { get; private set; }

        public string PracovniPozice { get; private set; } = "";

        public Zamestnanec(string jmeno, string prijmeni, DateOnly datumNarozeni,
                       int mzda, string pracovniPozice)
        {
            NastavJmeno(jmeno);
            NastavPrijmeni(prijmeni);
            NastavDatumNarozeni(datumNarozeni);
            NastavMzdu(mzda);
            NastavPracovniPozici(pracovniPozice);
        }


        internal void NastavJmeno(string noveJmeno)
        {
            Jmeno = Vstupy.ToTitleCase(noveJmeno);
        }

        internal void NastavPrijmeni(string novePrijmeni)
        {
            Prijmeni = Vstupy.ToTitleCase(novePrijmeni);
        }

        internal void NastavDatumNarozeni(DateOnly noveDatumNarozeni)
        {
            DatumNarozeni = noveDatumNarozeni;
        }

        internal void NastavMzdu(int novaMzda)
        {
            Mzda = novaMzda;
        }

        internal void NastavPracovniPozici(string novaPracovniPozice)
        {
            PracovniPozice = Vstupy.ToTitleCase(novaPracovniPozice);
        }

        public void VypisZamestnance()
        {
            Console.WriteLine("Jméno zaměstnance: {0}", Jmeno);

            Console.WriteLine("\tPříjmení zaměstnance: {0}", Prijmeni);

            Console.WriteLine("\tDatum narození zaměstnance: {0}", DatumNarozeni);

            Console.WriteLine("\tMzda zaměstnance: {0}", Mzda);

            Console.WriteLine("\tPracovní pozice zaměstnance: {0}", PracovniPozice);
        }

        public string ToFileString()
        {
            return $"{Jmeno};{Prijmeni};{Mzda}";
        }

        public static Zamestnanec Parse(string line)
        {
            string[] parts = line.Split(';');

            if (parts.Length != 5)
                throw new FormatException("Řádek nemá správný formát pro Zamestnance.");

            if (string.IsNullOrWhiteSpace(parts[0]))
                throw new FormatException("Jméno nesmí být prázdné.");
            string jmeno = parts[0];

            if (string.IsNullOrWhiteSpace(parts[1]))
                throw new FormatException("Příjmení nesmí být prázdné.");
            string prijmeni = parts[1];

            if (!DateOnly.TryParse(parts[2], out DateOnly datumNarozeni))
                throw new FormatException("Datum narození není platné.");

            if (!int.TryParse(parts[3], out int mzda))
                throw new FormatException("Mzda není platné číslo.");

            if (string.IsNullOrWhiteSpace(parts[4]))
                throw new FormatException("Pracovní pozice nesmí být prázdná.");
            string pracovniPozice = parts[4];


            return new Zamestnanec(jmeno, prijmeni, datumNarozeni, mzda, pracovniPozice);
        }

    }
}
