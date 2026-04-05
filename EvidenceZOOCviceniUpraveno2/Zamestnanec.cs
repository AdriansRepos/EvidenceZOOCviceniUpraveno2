
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
    }
}
