using System.Text.Json;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje správu zoologické zahrady – načítání dat, ukládání,
    /// poskytování statistik a práce se soubory.
    /// </summary>
    /// <remarks>
    /// Inicializuje instanci třídy ZOO, uloží cesty k souborům
    /// a načte zaměstnance i zvířata.
    /// </remarks>
    /// <param name="souborZam">Cesta k souboru se zaměstnanci.</param>
    /// <param name="souborZvir">Cesta k souboru se zvířaty.</param>        
    class ZOO(string souborZam, string souborZvir)
    {
        /// <summary>
        /// Seznam všech zvířat načtených ze souboru.
        /// </summary>
        public List<Zvire> Zvirata { get; internal set; } = [];

        /// <summary>
        /// Seznam všech zaměstnanců načtených ze souboru.
        /// </summary>
        public List<Zamestnanec> Zamestnanci { get; internal set; } = [];

        /// <summary>
        /// Cesta k souboru se zaměstnanci.
        /// </summary>
        public string SouborZamestnanci { get; private set; } = souborZam;

        /// <summary>
        /// Cesta k souboru se zvířaty.
        /// </summary>
        public string SouborZvirata { get; private set; } = souborZvir;

        /// <summary>
        /// Registr načítacích akcí pro jednotlivé datové moduly.
        /// Klíč = název modulu (např. "Zaměstnanci", "Zvířata").
        /// Hodnota = delegát, který provede načtení dat pro daný modul.
        /// </summary>
        private readonly Dictionary<string, Action> akceNacitani = [];

        /// <summary>
        /// Sada modulů, které již byly načteny.
        /// Slouží k zajištění, že se každý modul načte pouze jednou
        /// bez ohledu na to, kolikrát je požadován.
        /// </summary>
        private readonly HashSet<string> nacteno = [];

        /// <summary>
        /// Cesta ke konfiguračnímu souboru, který ukládá umístění JSON souborů
        /// se zaměstnanci a zvířaty. Používá se při startu aplikace.
        /// </summary>
        private static readonly string KonfigSoubor = @"..\..\..\konfig.txt";

        /// <summary>
        /// Nastavení JSON serializace pro zaměstnance.
        /// Obsahuje konvertor pro DateOnly.
        /// </summary>
        private static readonly JsonSerializerOptions ZamestnanecJsonOptions = new()
        {
            Converters = { new DateOnlyConverter() }
        };

        /// <summary>
        /// Nastavení JSON serializace pro zaměstnance s odsazením,
        /// používané při ukládání do souboru.
        /// </summary>
        private static readonly JsonSerializerOptions ZamestnanecJsonOptionsIndented = new()
        {
            WriteIndented = true,
            Converters = { new DateOnlyConverter() }
        };

        /// <summary>
        /// Nastavení JSON serializace pro zvířata s odsazením.
        /// </summary>
        private static readonly JsonSerializerOptions ZvireJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

        /// <summary>
        /// Při prvním spuštění se zeptá na složku a uloží cesty do konfig.txt.
        /// Při každém dalším spuštění načte cesty z konfig.txt automaticky.
        /// </summary>
        public static (string souborZam, string souborZvir) NactiNeboSeZeptejNaCesty()
        {
            // Pokud konfig existuje, načti cesty z něj
            if (File.Exists(KonfigSoubor))
            {
                string[] radky = File.ReadAllLines(KonfigSoubor);
                if (radky.Length == 2
                    && !string.IsNullOrWhiteSpace(radky[0])
                    && !string.IsNullOrWhiteSpace(radky[1]))
                {
                    Console.WriteLine($"Načteny uložené cesty z: {KonfigSoubor}");
                    return (radky[0], radky[1]);
                }
            }

            // První spuštění — zeptej se na složku
            Console.WriteLine("=== PRVNÍ SPUŠTĚNÍ — NASTAVENÍ CEST ===");
            Console.Write("Zadej cestu ke složce pro ukládání dat: ");
            string slozka = Console.ReadLine()!.Trim();

            // Vytvoří složku, pokud neexistuje
            Directory.CreateDirectory(slozka);

            string zam = Path.Combine(slozka, "zamestnanci.json");
            string zvir = Path.Combine(slozka, "zvirata.json");

            // Uloží cesty trvale do konfig.txt
            File.WriteAllLines(KonfigSoubor, [zam, zvir]);

            Console.WriteLine($"Cesty uloženy. Data budou ukládána do: {slozka}");
            return (zam, zvir);
        }

        /// <summary>
        /// Zaregistruje načítací akci pro daný datový modul.
        /// Klíč určuje název modulu (např. "Zaměstnanci", "Zvířata").
        /// Akce je delegát, který provede načtení dat.
        /// </summary>
        /// <param name="klic">Identifikátor modulu, pro který se registruje načítání.</param>
        /// <param name="akce">Delegát obsahující logiku načtení dat.</param>
        public void RegistrujNacitani(string klic, Action akce)
        {
            akceNacitani[klic] = akce;   // uloží nebo přepíše načítací akci pro daný klíč
        }

        /// <summary>
        /// Zajistí, že data pro daný modul budou načtena.
        /// Pokud již byla načtena dříve, znovu se nenačítají.
        /// </summary>
        /// <param name="klic">Identifikátor modulu, jehož data mají být zajištěna.</param>
        public void ZajistiData(string klic)
        {
            if (!nacteno.Contains(klic))                 // pokud modul ještě nebyl načten
            {
                if (akceNacitani.TryGetValue(klic, out var akce))   // najde registrovanou akci
                {
                    akce.Invoke();                      // provede načtení dat
                    nacteno.Add(klic);                   // označí modul jako načtený
                }
                else
                {
                    // pokud není registrována žádná akce, jde o chybu návrhu
                    throw new InvalidOperationException(
                        $"Pro klíč '{klic}' není registrováno načítání dat."
                    );
                }
            }
        }

        // -----------------------------
        // VEŘEJNÉ METODY PRO NAČTENÍ DAT
        // -----------------------------

        public void NactiZamestnance(string cesta)
        {
            SouborZamestnanci = cesta;
            Zamestnanci = NactiZamestnanceZeSouboru();
        }

        public void NactiZvirata(string cesta)
        {
            SouborZvirata = cesta;
            Zvirata = NactiZvirataZeSouboru();
        }

        /// <summary>
        /// Načte zaměstnance ze souboru.
        /// </summary>
        /// <returns>Seznam načtených zaměstnanců.</returns>
        private List<Zamestnanec> NactiZamestnanceZeSouboru()
        {
            // Pokud .json chybí, ale .bak existuje, obnov ze zálohy
            if (!File.Exists(SouborZamestnanci) && File.Exists(SouborZamestnanci + ".bak"))
            {
                File.Copy(SouborZamestnanci + ".bak", SouborZamestnanci);
                Console.WriteLine("Soubor zaměstnanců obnoven ze zálohy.");
            }

            // Pokud soubor neexistuje, vrátí prázdný seznam
            if (!File.Exists(SouborZamestnanci))
                return [];

            try
            {
                string json = File.ReadAllText(SouborZamestnanci);
                return JsonSerializer.Deserialize<List<Zamestnanec>>(json, ZamestnanecJsonOptions) ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání zaměstnanců: {ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// Načte zvířata ze souboru.
        /// </summary>
        /// <returns>Seznam načtených zvířat.</returns>
        private List<Zvire> NactiZvirataZeSouboru()
        {
            // Pokud .json chybí, ale .bak existuje, obnov ze zálohy
            if (!File.Exists(SouborZvirata) && File.Exists(SouborZvirata + ".bak"))
            {
                File.Copy(SouborZvirata + ".bak", SouborZvirata);
                Console.WriteLine("Soubor zvířat obnoven ze zálohy.");
            }

            // Pokud soubor neexistuje, vrátí prázdný seznam
            if (!File.Exists(SouborZvirata))
                return [];

            try
            {
                string json = File.ReadAllText(SouborZvirata);
                return JsonSerializer.Deserialize<List<Zvire>>(json) ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání zvířat: {ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// Uloží všechny zaměstnance zpět do souboru a vytvoří zálohu.
        /// </summary>
        public void UlozZamestnance()
        {
            try
            {
                if (File.Exists(SouborZamestnanci))
                    File.Copy(SouborZamestnanci, SouborZamestnanci + ".bak", overwrite: true);

                string json = JsonSerializer.Serialize(Zamestnanci, ZamestnanecJsonOptionsIndented);
                File.WriteAllText(SouborZamestnanci, json);
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
                    File.Copy(SouborZvirata, SouborZvirata + ".bak", overwrite: true);

                string json = JsonSerializer.Serialize(Zvirata, ZvireJsonOptionsIndented);
                File.WriteAllText(SouborZvirata, json);
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
            ZajistiData("Zaměstnanci");
            ZajistiData("Zvířata");
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