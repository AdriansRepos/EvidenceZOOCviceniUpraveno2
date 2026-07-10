using System.Text.Json;
using DateConverterForJson;

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
        private static readonly string KonfigSoubor =
                 Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 "EvidenceZOOCviceniUpraveno2",
                 "config.ini");

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

        private static readonly JsonSerializerOptions ZvireJsonOptionsIndented = new()
        {
            WriteIndented = true,
            Converters = { new DateOnlyConverter() }
        };
        
        private static readonly JsonSerializerOptions ZvireJsonOptions = new()
        {
            Converters = { new DateOnlyConverter() }
        };

        // ----------------------------------------------------------------
        // METODY PRO PRÁCI S KLÍČI (registrace a zajištění načtení modulů)
        // ----------------------------------------------------------------

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

        // ------------------------
        // KONFIGURACE (config.ini)
        // ------------------------

        /// <summary>
        /// Při prvním spuštění se zeptá na složku a uloží cesty do config.ini.
        /// Při každém dalším spuštění načte cesty z config.ini automaticky.
        /// </summary>
        public static (string souborZam, string souborZvir) NactiNeboSeZeptejNaCesty()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(KonfigSoubor)!);

            // Pokud config.ini chybí, ale záloha existuje, obnov ji
            if (!File.Exists(KonfigSoubor) && File.Exists(KonfigSoubor + ".bak"))
            {
                File.Copy(KonfigSoubor + ".bak", KonfigSoubor);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Config.ini obnoven ze zálohy.");
                Console.ResetColor();
            }

            if (File.Exists(KonfigSoubor))
            {
                var hodnoty = new Dictionary<string, string>();
                foreach (string radek in File.ReadAllLines(KonfigSoubor))
                {
                    string upraven = radek.Trim();
                    if (upraven.Length == 0 || upraven.StartsWith("#") || upraven.StartsWith(";"))
                        continue;

                    int idx = upraven.IndexOf('=');
                    if (idx <= 0) continue;

                    string klic = upraven[..idx].Trim();
                    string hodnota = upraven[(idx + 1)..].Trim();
                    hodnoty[klic] = hodnota;
                }

                if (hodnoty.TryGetValue("souborZam", out var zamUlozeny)
                    && hodnoty.TryGetValue("souborZvir", out var zvirUlozeny)
                    && !string.IsNullOrWhiteSpace(zamUlozeny)
                    && !string.IsNullOrWhiteSpace(zvirUlozeny))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Načteny uložené cesty z: {KonfigSoubor}");
                    Console.ResetColor();
                    return (zamUlozeny, zvirUlozeny);
                }
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=== PRVNÍ SPUŠTĚNÍ — NASTAVENÍ CEST ===");
            Console.ResetColor();
            Console.Write("Zadej cestu ke složce pro ukládání dat: ");
            string slozka = Console.ReadLine()!.Trim();

            Directory.CreateDirectory(slozka);
            string zam = Path.Combine(slozka, "zamestnanci.json");
            string zvir = Path.Combine(slozka, "zvirata.json");

            ZapisKonfigSeZalohou(zam, zvir);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Cesty uloženy. Data budou ukládána do: {slozka}");
            Console.ResetColor();
            return (zam, zvir);
        }

        /// <summary>
        /// Zapíše config.ini bezpečně – nejprve zálohuje starý, pak zapíše nový
        /// přes dočasný soubor, aby při pádu aplikace uprostřed zápisu
        /// nezůstal config.ini poškozený nebo prázdný.
        /// </summary>
        private static void ZapisKonfigSeZalohou(string souborZam, string souborZvir)
        {
            // 1) Zálohuj aktuální config.ini, pokud existuje
            if (File.Exists(KonfigSoubor))
                File.Copy(KonfigSoubor, KonfigSoubor + ".bak", overwrite: true);
        
            // 2) Zapiš nejdřív do dočasného souboru
            string docasny = KonfigSoubor + ".tmp";
            File.WriteAllLines(docasny, [
                $"souborZam={souborZam}",
                $"souborZvir={souborZvir}"
            ]);
        
            // 3) Atomicky nahraď – File.Replace zajistí, že buď proběhne celé,
            // nebo se nic nezmění (na rozdíl od přímého přepsání File.WriteAllLines)
            if (File.Exists(KonfigSoubor))
                File.Replace(docasny, KonfigSoubor, null);
            else
                File.Move(docasny, KonfigSoubor);
        }

        // ------------------------------
        // VEŘEJNÉ METODY PRO NAČTENÍ DAT
        // ------------------------------

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
        /// Načte zaměstnance ze souboru. Pokud je soubor poškozený (nevalidní JSON),
        /// automaticky zkusí obnovit data ze zálohy (.bak). Pokud soubor chybí úplně,
        /// ale záloha existuje, obnoví ji jako hlavní soubor.
        /// </summary>
        /// <returns>Seznam načtených zaměstnanců.</returns>
        private List<Zamestnanec> NactiZamestnanceZeSouboru()
        {
            // Pokud .json chybí, ale .bak existuje, obnov ze zálohy
            if (!File.Exists(SouborZamestnanci) && File.Exists(SouborZamestnanci + ".bak"))
            {
                File.Copy(SouborZamestnanci + ".bak", SouborZamestnanci);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Soubor zaměstnanců obnoven ze zálohy.");
                Console.ResetColor();
            }
        
            // Pokud soubor neexistuje ani po pokusu o obnovu, vrátí prázdný seznam
            if (!File.Exists(SouborZamestnanci))
                return [];
        
            try
            {
                string json = File.ReadAllText(SouborZamestnanci);
                return JsonSerializer.Deserialize<List<Zamestnanec>>(json, ZamestnanecJsonOptions) ?? [];
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání zaměstnanců: {ex.Message}");
                Console.ResetColor();
                return NactiZeZalohyZamestnanci();
            }
        }
        
        /// <summary>
        /// Pokusí se obnovit zaměstnance ze záložního souboru (.bak) poté,
        /// co se nepodařilo načíst hlavní soubor (např. kvůli poškození).
        /// </summary>
        /// <returns>Seznam zaměstnanců ze zálohy, nebo prázdný seznam, pokud záloha
        /// neexistuje nebo je také poškozená.</returns>
        private List<Zamestnanec> NactiZeZalohyZamestnanci()
        {
            string zaloha = SouborZamestnanci + ".bak";
        
            if (!File.Exists(zaloha))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("!!! POZOR: Data zaměstnanců se nepodařilo načíst ani ze zálohy !!!");
                Console.ResetColor();
                return [];
            }
        
            try
            {
                string json = File.ReadAllText(zaloha);
                var data = JsonSerializer.Deserialize<List<Zamestnanec>>(json, ZamestnanecJsonOptions) ?? [];
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Zaměstnanci úspěšně obnoveni ze zálohy (.bak).");
                Console.ResetColor();
                return data;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Záloha zaměstnanců je také poškozená: {ex.Message}");
                Console.ResetColor();
                return [];
            }
        }
        
        /// <summary>
        /// Načte zvířata ze souboru. Pokud je soubor poškozený (nevalidní JSON),
        /// automaticky zkusí obnovit data ze zálohy (.bak). Pokud soubor chybí úplně,
        /// ale záloha existuje, obnoví ji jako hlavní soubor.
        /// </summary>
        /// <returns>Seznam načtených zvířat.</returns>
        private List<Zvire> NactiZvirataZeSouboru()
        {
            // Pokud .json chybí, ale .bak existuje, obnov ze zálohy
            if (!File.Exists(SouborZvirata) && File.Exists(SouborZvirata + ".bak"))
            {
                File.Copy(SouborZvirata + ".bak", SouborZvirata);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Soubor zvířat obnoven ze zálohy.");
                Console.ResetColor();
            }
        
            // Pokud soubor neexistuje ani po pokusu o obnovu, vrátí prázdný seznam
            if (!File.Exists(SouborZvirata))
                return [];
        
            try
            {
                string json = File.ReadAllText(SouborZvirata);
                return JsonSerializer.Deserialize<List<Zvire>>(json) ?? [];
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání zvířat: {ex.Message}");
                Console.ResetColor();
                return NactiZeZalohyZvirata();
            }
        }

        /// <summary>
        /// Pokusí se obnovit zvířata ze záložního souboru (.bak) poté,
        /// co se nepodařilo načíst hlavní soubor (např. kvůli poškození).
        /// </summary>
        /// <returns>Seznam zvířat ze zálohy, nebo prázdný seznam, pokud záloha
        /// neexistuje nebo je také poškozená.</returns>
        private List<Zvire> NactiZeZalohyZvirata()
        {
            string zaloha = SouborZvirata + ".bak";
        
            if (!File.Exists(zaloha))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("!!! POZOR: Data zvířat se nepodařilo načíst ani ze zálohy !!!");
                Console.ResetColor();
                return [];
            }
        
            try
            {
                string json = File.ReadAllText(zaloha);
                var data = JsonSerializer.Deserialize<List<Zvire>>(json) ?? [];
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Zvířata úspěšně obnovena ze zálohy (.bak).");
                Console.ResetColor();
                return data;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Záloha zvířat je také poškozená: {ex.Message}");
                Console.ResetColor();
                return [];
            }
        }

        /// <summary>
        /// Uloží všechny zaměstnance zpět do souboru bezpečně (přes dočasný soubor)
        /// a vytvoří zálohu předchozí verze.
        /// </summary>
        public void UlozZamestnance()
        {
            try
            {
                string json = JsonSerializer.Serialize(Zamestnanci, ZamestnanecJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborZamestnanci, json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání zaměstnanců: {ex.Message}");
                Console.ResetColor();
            }
        }
        
        /// <summary>
        /// Uloží všechna zvířata zpět do souboru bezpečně (přes dočasný soubor)
        /// a vytvoří zálohu předchozí verze.
        /// </summary>
        public void UlozZvirata()
        {
            try
            {
                string json = JsonSerializer.Serialize(Zvirata, ZvireJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborZvirata, json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání zvířat: {ex.Message}");
                Console.ResetColor();
            }
        }
        
        /// <summary>
        /// Bezpečně zapíše text do cílového souboru: nejprve zálohuje starou verzi
        /// (.bak), poté zapíše nový obsah do dočasného souboru (.tmp) a teprve
        /// po úspěšném zápisu ho atomicky nahradí na místo cílového souboru.
        /// Zajišťuje, že při pádu aplikace uprostřed zápisu nedojde k poškození
        /// ani ztrátě posledního platného souboru.
        /// </summary>
        /// <param name="cilovySoubor">Cesta k cílovému souboru (např. zamestnanci.json).</param>
        /// <param name="obsah">Textový obsah k zapsání (např. JSON).</param>
        private static void ZapisSouborSeZalohou(string cilovySoubor, string obsah)
        {
            string docasny = cilovySoubor + ".tmp";
            string zaloha = cilovySoubor + ".bak";
        
            // 1) Zapiš nejdřív do dočasného souboru – pokud zápis selže
            // (např. dojde místo na disku), originál ani záloha nejsou ohroženy
            File.WriteAllText(docasny, obsah);
        
            // 2) Atomicky nahraď cílový soubor dočasným, přičemž File.Replace
            // rovnou vytvoří i zálohu (třetí parametr) v jediném atomickém kroku
            if (File.Exists(cilovySoubor))
            {
                File.Replace(docasny, cilovySoubor, zaloha);
            }
            else
            {
                // Cílový soubor ještě neexistuje (první uložení) – není co nahrazovat
                File.Move(docasny, cilovySoubor);
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
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n=== STATISTIKY ===");
                Console.WriteLine("\t1. Počet zvířat");
                Console.WriteLine("\t2. Počet zaměstnanců");
                Console.WriteLine("\t3. Součet mezd zaměstnanců");
                Console.WriteLine("\t4. Návrat do hlavního menu");
                Console.ResetColor();
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
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Neplatná volba, opakujte zadání:");
                        Console.ResetColor();
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