
namespace EvidenceZOOCviceniUpraveno2
{
    class ZOO
    {
        /// <summary>
        /// Vytvoření kolekce pro evidenci zvířat 
        /// </summary>
        public List<Zvire> Zvirata { get; private set; }

        /// <summary>
        /// Vytvoření kolekce pro evidenci zvířat 
        /// </summary>
        public List<Zamestnanec> Zamestnanci { get; private set; }

        /// <summary>
        /// Vytvoření instance ZOO
        /// </summary>
        public ZOO()
        {
            Zvirata = [];
            Zamestnanci = [];
        }

        /// <summary>
        /// Podmenu pro statistiky
        /// </summary>
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

        /// <summary>
        /// Statistika celkového počtu zvířat v ZOO
        /// </summary>
        /// <returns>celkový počet zvířat</returns>
        public int PocetZvirat()
        {
            return Zvirata.Count;
        }

        /// <summary>
        /// Statistika celkového počtu zaměstnanců v ZOO
        /// </summary>
        /// <returns></returns>
        public int PocetZamestnancu()
        {
            return Zamestnanci.Count;
        }

        /// <summary>
        /// Statistika celkového mzdového nákladu na všechny zaměstnance v ZOO
        /// </summary>
        /// <returns>součet všech mezd</returns>
        public int SoucetMezd()
        {
            return Zamestnanci.Sum(z => z.Mzda);
        }
    }
}
