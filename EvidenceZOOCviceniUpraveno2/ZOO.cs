
namespace EvidenceZOOCviceniUpraveno2
{
    class ZOO
    {
        // Seznam všech zvířat načtených ze souboru
        public List<Zvire> Zvirata { get; private set; }

        // Seznam všech zaměstnanců načtených ze souboru
        public List<Zamestnanec> Zamestnanci { get; private set; }

        // Cesta k souboru se zaměstnanci
        public string SouborZamestnanci { get; private set; } = "";

        // Cesta k souboru se zvířaty
        public string SouborZvirata { get; private set; } = "";

        // Konstruktor – uloží cesty k souborům, zajistí jejich existenci
        // a načte data do seznamů
        public ZOO(string souborZam, string souborZvir)
        {
            SouborZamestnanci = souborZam;
            SouborZvirata = souborZvir;

            // Pokud soubory neexistují, vytvoří je
            KontrolaExistenceSouboru(SouborZamestnanci);
            KontrolaExistenceSouboru(SouborZvirata);

            // Načtení dat ze souborů do paměti
            Zamestnanci = NactiZamestnanceZeSouboru();
            Zvirata = NactiZvirataZeSouboru();
        }

        // Zkontroluje existenci souboru a pokud chybí, vytvoří ho
        public static void KontrolaExistenceSouboru(string cesta)
        {
            try
            {
                if (!File.Exists(cesta))
                {
                    Console.WriteLine($"Vytvářím soubor: {cesta}");
                    using FileStream fs = File.Create(cesta);
                }
                else
                {
                    Console.WriteLine($"Soubor už existuje: {cesta}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při práci se souborem: {ex.Message}");
            }
        }

        // Načte zaměstnance ze souboru a převede je pomocí Parse
        private List<Zamestnanec> NactiZamestnanceZeSouboru()
        {
            try
            {
                return [.. File.ReadAllLines(SouborZamestnanci)
                   .Where(r => !string.IsNullOrWhiteSpace(r))   // ignoruje prázdné řádky
                   .Select(Zamestnanec.Parse)];                 // převede řádek na objekt
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Soubor se zaměstnanci nebyl nalezen. Pokračuji prázdným seznamem.");
                return [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání zaměstnanců: {ex.Message}");
                return [];
            }
        }

        // Načte zvířata ze souboru a převede je pomocí Parse
        private List<Zvire> NactiZvirataZeSouboru()
        {
            try
            {
                return [.. File.ReadAllLines(SouborZvirata)
                   .Where(l => !string.IsNullOrWhiteSpace(l))   // ignoruje prázdné řádky
                   .Select(Zvire.Parse)];                       // převede řádek na objekt
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Soubor se zvířaty nebyl nalezen. Pokračuji prázdným seznamem.");
                return [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání zvířat: {ex.Message}");
                return [];
            }
        }

        // Uloží všechny zaměstnance zpět do souboru
        public void UlozZamestnance()
        {
            try
            {
                File.WriteAllLines(SouborZamestnanci,
                    Zamestnanci.Select(z => z.ToFileString()));  // každý objekt převede na řádek
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání zaměstnanců: {ex.Message}");
            }
        }

        // Uloží všechna zvířata zpět do souboru
        public void UlozZvirata()
        {
            try
            {
                File.WriteAllLines(SouborZvirata,
                    Zvirata.Select(z => z.ToFileString()));       // každý objekt převede na řádek
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání zvířat: {ex.Message}");
            }
        }

        // Hlavní menu statistik
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
                        break;

                    default:
                        Console.WriteLine("Neplatná volba, opakujte zadání:");
                        break;
                }

            }
            while (volba != '4');
        }

        // Vrátí počet zvířat v seznamu
        public int PocetZvirat()
        {
            return Zvirata.Count;
        }

        // Vrátí počet zaměstnanců v seznamu
        public int PocetZamestnancu()
        {
            return Zamestnanci.Count;
        }

        // Vrátí součet mezd všech zaměstnanců
        public int SoucetMezd()
        {
            return Zamestnanci.Sum(z => z.Mzda);
        }
    }

}
