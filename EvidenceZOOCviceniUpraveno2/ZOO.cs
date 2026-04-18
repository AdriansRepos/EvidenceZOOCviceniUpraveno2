
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

        // Cesta k logovacímu souboru pro chybné řádky
        private readonly string LogSoubor = "";

        /* Konstruktor – uloží cesty k souborům, zajistí jejich existenci
         * a načte data do seznamů */
        public ZOO(string souborZam, string souborZvir, string logSoubor)
        {
            SouborZamestnanci = souborZam;
            SouborZvirata = souborZvir;
            LogSoubor = logSoubor;

            Zamestnanci = NactiZamestnanceZeSouboru();
            Zvirata = NactiZvirataZeSouboru();
        }

        // Načte zaměstnance ze souboru, nevalidní řádky zapíše do logu
        private List<Zamestnanec> NactiZamestnanceZeSouboru()
        {   // Vytvoření prázdného seznamu pro načtené zaměstnance
            var list = new List<Zamestnanec>();
            // Otevření logovacího souboru pro zápis chyb (v režimu přidávání)
            using StreamWriter log = new(LogSoubor, append: true);
            // Kontrola existence souboru se zaměstnanci, pokud neexistuje, vrátí prázdný seznam
            if (!File.Exists(SouborZamestnanci))
                return list;
            // Procházení každého řádku v souboru se zaměstnanci
            foreach (var radek in File.ReadAllLines(SouborZamestnanci))
            {
                try
                {   // Kontrola, zda řádek není prázdný nebo pouze bílý, pokud ano, přeskočí ho
                    if (string.IsNullOrWhiteSpace(radek))
                        continue;
                    // Pokus o parsování řádku do objektu Zamestnanec, pokud se nepodaří, zachytí výjimku a zapíše chybu do logu
                    list.Add(Zamestnanec.Parse(radek));
                }  // Zachycení a logování případných chyb při parsování řádku
                catch (Exception ex)
                {
                    log.WriteLine($"{DateTime.Now}: Chybný řádek v zaměstnancích: \"{radek}\" – {ex.Message}");
                }
            }
            // Vrácení seznamu načtených zaměstnanců
            return list;
        }

        // Načte zvířata ze souboru, nevalidní řádky zapíše do logu
        private List<Zvire> NactiZvirataZeSouboru()
        {   // Vytvoření prázdného seznamu pro načtená zvířata
            var list = new List<Zvire>();
            // Otevření logovacího souboru pro zápis chyb (v režimu přidávání)
            using StreamWriter log = new(LogSoubor, append: true);
            // Kontrola existence souboru se zvířaty, pokud neexistuje, vrátí prázdný seznam
            if (!File.Exists(SouborZvirata))
                return list;
            // Procházení každého řádku v souboru se zvířaty
            foreach (var radek in File.ReadAllLines(SouborZvirata))
            {
                try
                {   // Kontrola, zda řádek není prázdný nebo pouze bílý, pokud ano, přeskočí ho
                    if (string.IsNullOrWhiteSpace(radek))
                        continue;
                    // Pokus o parsování řádku do objektu Zvire, pokud se nepodaří, zachytí výjimku a zapíše chybu do logu
                    list.Add(Zvire.Parse(radek));
                }
                catch (Exception ex)
                {
                    log.WriteLine($"{DateTime.Now}: Chybný řádek ve zvířatech: \"{radek}\" – {ex.Message}");
                }
            }
            // Vrácení seznamu načtených zvířat
            return list;
        }

        // Uloží všechny zaměstnance zpět do souboru (včetně automatické zálohy)
        public void UlozZamestnance()
        {
            try
            {
                // Zálohuje jen pokud původní soubor existuje
                if (File.Exists(SouborZamestnanci))
                {
                    File.Copy(SouborZamestnanci, SouborZamestnanci + ".bak", overwrite: true);
                }

                // na začátku vytvoří nový soubor, pokud neexistuje, a zapíše všechny zaměstnance do souboru
                File.WriteAllLines(SouborZamestnanci,
                    Zamestnanci.Select(z => z.ToFileString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání zaměstnanců: {ex.Message}");
            }
        }

        // Uloží všechna zvířata zpět do souboru (včetně automatické zálohy)
        public void UlozZvirata()
        {
            try
            {
                if (File.Exists(SouborZvirata))
                {   // Zálohuje jen pokud původní soubor existuje
                    File.Copy(SouborZvirata, SouborZvirata + ".bak", overwrite: true);
                }
                // na začátku vytvoří nový soubor, pokud neexistuje, a zapíše všechny zvířata do souboru
                File.WriteAllLines(SouborZvirata,
                    Zvirata.Select(z => z.ToFileString()));
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
