
namespace EvidenceZOOCviceniUpraveno2
{
    class ZOO
    {
        public List<Zvire> Zvirata { get; private set; }
        public List<Zamestnanec> Zamestnanci { get; private set; }
        public string SouborZamestnanci { get; private set; } = "";
        public string SouborZvirata { get; private set; } = "";
        public ZOO(string souborZam, string souborZvir)
        {
            SouborZamestnanci = souborZam;
            SouborZvirata = souborZvir;

            KontrolaExistenceSouboru(SouborZamestnanci);
            KontrolaExistenceSouboru(SouborZvirata);

            Zamestnanci = NactiZamestnanceZeSouboru();
            Zvirata = NactiZvirataZeSouboru();
        }

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

        private List<Zamestnanec> NactiZamestnanceZeSouboru()
        {
            try
            {
                return [.. File.ReadAllLines(SouborZamestnanci)
                       .Where(l => !string.IsNullOrWhiteSpace(l))
                       .Select(Zamestnanec.Parse)];
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

        private List<Zvire> NactiZvirataZeSouboru()
        {
            try
            {
                return [.. File.ReadAllLines(SouborZvirata)
                       .Where(l => !string.IsNullOrWhiteSpace(l))
                       .Select(Zvire.Parse)];
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

        public void UlozZamestnance()
        {
            try
            {
                File.WriteAllLines(SouborZamestnanci,
                    Zamestnanci.Select(z => z.ToFileString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání zaměstnanců: {ex.Message}");
            }
        }

        public void UlozZvirata()
        {
            try
            {
                File.WriteAllLines(SouborZvirata,
                    Zvirata.Select(z => z.ToFileString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání zvířat: {ex.Message}");
            }
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
