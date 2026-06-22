
namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje správu zoologické zahrady – načítání dat, ukládání,
    /// poskytování statistik a práce se soubory.
    /// </summary>
    class ZOO
    {
        /// <summary>
        /// Seznam všech zvířat načtených ze souboru.
        /// </summary>
        public List<Zvire> Zvirata { get; private set; }

        /// <summary>
        /// Seznam všech zaměstnanců načtených ze souboru.
        /// </summary>
        public List<Zamestnanec> Zamestnanci { get; private set; }

        /// <summary>
        /// Cesta k souboru se zaměstnanci.
        /// </summary>
        public string SouborZamestnanci { get; private set; } = "";

        /// <summary>
        /// Cesta k souboru se zvířaty.
        /// </summary>
        public string SouborZvirata { get; private set; } = "";

        /// <summary>
        /// Cesta k logovacímu souboru, kam se zapisují chybné řádky.
        /// </summary>
        private readonly string LogSoubor = "";

        /// <summary>
        /// Inicializuje instanci třídy ZOO, uloží cesty k souborům
        /// a načte zaměstnance i zvířata.
        /// </summary>
        /// <param name="souborZam">Cesta k souboru se zaměstnanci.</param>
        /// <param name="souborZvir">Cesta k souboru se zvířaty.</param>
        /// <param name="logSoubor">Cesta k logovacímu souboru.</param>
        public ZOO(string souborZam, string souborZvir, string logSoubor)
        {
            SouborZamestnanci = souborZam;
            SouborZvirata = souborZvir;
            LogSoubor = logSoubor;

            Zamestnanci = NactiZamestnanceZeSouboru();
            Zvirata = NactiZvirataZeSouboru();
        }

        /// <summary>
        /// Načte zaměstnance ze souboru. Nevalidní řádky zapisuje do logu.
        /// </summary>
        /// <returns>Seznam načtených zaměstnanců.</returns>
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

        /// <summary>
        /// Načte zvířata ze souboru. Nevalidní řádky zapisuje do logu.
        /// </summary>
        /// <returns>Seznam načtených zvířat.</returns>
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

        /// <summary>
        /// Uloží všechny zaměstnance zpět do souboru a vytvoří zálohu.
        /// </summary>
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

        /// <summary>
        /// Uloží všechna zvířata zpět do souboru a vytvoří zálohu.
        /// </summary>
        public void UlozZvirata()
        {
            try
            {
                if (File.Exists(SouborZvirata))
                {   // Zálohuje jen pokud původní soubor existuje
                    File.Copy(SouborZvirata, SouborZvirata + ".bak", overwrite: true);
                }
                // na začátku vytvoří nový soubor, pokud neexistuje, a zapíše všechny zaměstnance do souboru
                File.WriteAllLines(SouborZvirata,
                    Zvirata.Select(z => z.ToFileString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání zvířat: {ex.Message}");
            }
        }

        /// <summary>
        /// Zobrazí hlavní menu statistik a umožní uživateli vybrat požadovanou akci.
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
                        break;

                    default:
                        Console.WriteLine("Neplatná volba, opakujte zadání:");
                        break;
                }

            }
            while (volba != '4');
        }

        /// <summary>
        /// Vrátí počet zvířat v seznamu.
        /// </summary>
        public int PocetZvirat()
        {
            return Zvirata.Count;
        }

        /// <summary>
        /// Vrátí počet zaměstnanců v seznamu.
        /// </summary>
        public int PocetZamestnancu()
        {
            return Zamestnanci.Count;
        }

        /// <summary>
        /// Vrátí součet mezd všech zaměstnanců.
        /// </summary>
        public int SoucetMezd()
        {
            return Zamestnanci.Sum(z => z.Mzda);
        }
    }
}
