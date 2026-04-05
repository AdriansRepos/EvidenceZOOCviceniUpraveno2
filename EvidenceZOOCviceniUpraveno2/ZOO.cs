
namespace EvidenceZOOCviceniUpraveno2
{
    class ZOO
    {
        public List<Zvire> Zvirata { get; private set; }

        public List<Zamestnanec> Zamestnanci { get; private set; }

        public ZOO()
        {
            Zvirata = [];
            Zamestnanci = [];
        }

        public void MenuStatistiky()
        {
            char volba;
            do
            {
                Console.WriteLine("\n=== STATISTIKY ===");
                Console.WriteLine("\t1. Počet zvířat");
                Console.WriteLine("\t2. Počet zaměstnanců");
                Console.WriteLine("\t3. Součet mezd zaměstnanců");
                Console.WriteLine("\t4. Návrat do hlavního menu");
                Console.Write("Vyber možnost: ");

                volba = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (volba)
                {
                    case '1':
                        Console.WriteLine($"Počet zvířat: {PocetZvirat()}");
                        break;

                    case '2':
                        Console.WriteLine($"Počet zaměstnanců: {PocetZamestnancu()}");
                        break;

                    case '3':
                        Console.WriteLine($"Součet mezd: {SoucetMezd()} Kč");
                        break;

                    case '4':
                        Console.WriteLine();
                        break;

                    default:
                        Console.WriteLine("Neplatná volba, opakujte zadání:");
                        break;
                }
                                
            } 
            while (volba != '4');
        }

        public int PocetZvirat()
        {
            return Zvirata.Count;
        }

        public int PocetZamestnancu()
        {
            return Zamestnanci.Count;
        }

        public int SoucetMezd()
        {
            return Zamestnanci.Sum(z => z.Mzda);
        }
    }
}
