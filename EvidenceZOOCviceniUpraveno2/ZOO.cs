using DateConverterForJson;
using PohybHelper;
using System.Text.Json;
using TextHelper;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje správu zoologické zahrady – načítání dat, ukládání,
    /// roční archivaci, poskytování statistik, práci se soubory a s pokladnou pro vstupné.
    /// Data i jejich zálohy jsou organizovány v oddělených složkách
    /// Data/ a Zalohy/ uvnitř zvolené kořenové složky.
    /// </summary>
    /// <param name="korenovaSlozka">Kořenová složka, ve které jsou uložena všechna data a jejich zálohy.</param>
    class ZOO(string korenovaSlozka)
    {
        public List<Zvire> Zvirata { get; internal set; } = [];
        public List<Zamestnanec> Zamestnanci { get; internal set; } = [];
        public List<SkladovaPolozka> Sklad { get; internal set; } = [];
        public List<SkladovyPohyb> SkladovaHistorie { get; internal set; } = [];
        public List<PokladniPohyb> PokladniPohyby { get; internal set; } = [];
        public Cenik Cenik { get; internal set; } = new();
        public List<AuditZaznam> AuditLog { get; internal set; } = [];
        public List<TechnickyZaznam> TechnickyLog { get; internal set; } = [];

        /// <summary>
        /// Kořenová složka zvolená uživatelem, ve které jsou podsložky
        /// Data (aktuální soubory) a Zalohy (jejich zálohy).
        /// </summary>
        public string KorenovaSlozka { get; private set; } = korenovaSlozka;

        // -----------------------------
        // CESTY K DATOVÝM SOUBORŮM (Data/...)
        // -----------------------------

        public string SouborZamestnanci => Path.Combine(KorenovaSlozka, "Data", "Zamestnanci", "zamestnanci.json");
        public string SouborZvirata => Path.Combine(KorenovaSlozka, "Data", "Zvirata", "zvirata.json");
        public string SouborSkladu => Path.Combine(KorenovaSlozka, "Data", "Sklad", "sklad.json");
        public string SouborSkladoveHistorie => Path.Combine(KorenovaSlozka, "Data", "Sklad", "sklad_historie.json");
        public string SouborPokladny => Path.Combine(KorenovaSlozka, "Data", "Ucetnictvi", "Pokladna", "pokladna.json");
        public string SouborCeniku => Path.Combine(KorenovaSlozka, "Data", "Ucetnictvi", "Pokladna", "cenik.json");
        public string SouborAuditLogu => Path.Combine(KorenovaSlozka, "Data", "Log", "audit.json");
        public string SouborTechnickehoLogu => Path.Combine(KorenovaSlozka, "Data", "Log", "technicky.json");

        // -----------------------------
        // CESTY K ZÁLOHÁM (Zalohy/...)
        // -----------------------------

        private string ZalohaZamestnanci => Path.Combine(KorenovaSlozka, "Zalohy", "Zamestnanci", "zamestnanci.json.bak");
        private string ZalohaZvirata => Path.Combine(KorenovaSlozka, "Zalohy", "Zvirata", "zvirata.json.bak");
        private string ZalohaSkladu => Path.Combine(KorenovaSlozka, "Zalohy", "Sklad", "sklad.json.bak");
        private string ZalohaSkladoveHistorie => Path.Combine(KorenovaSlozka, "Zalohy", "Sklad", "sklad_historie.json.bak");
        private string ZalohaPokladny => Path.Combine(KorenovaSlozka, "Zalohy", "Ucetnictvi", "Pokladna", "pokladna.json.bak");
        private string ZalohaCeniku => Path.Combine(KorenovaSlozka, "Zalohy", "Ucetnictvi", "Pokladna", "cenik.json.bak");
        private string ZalohaAuditLogu => Path.Combine(KorenovaSlozka, "Zalohy", "Log", "audit.json.bak");
        private string ZalohaTechnickehoLogu => Path.Combine(KorenovaSlozka, "Zalohy", "Log", "technicky.json.bak");

        /// <summary>
        /// Cesta k záloze config.ini, uložené v datové složce uživatele
        /// (ne v AppData), aby přežila i reinstalaci aplikace nebo systému.
        /// </summary>
        private string ZalohaKonfigu => Path.Combine(KorenovaSlozka, "Zalohy", "config.ini.bak");

        /// <summary>
        /// Složka pro roční archivy dat.
        /// </summary>
        public string ArchivSlozka => Path.Combine(KorenovaSlozka, "Archiv");

        private readonly Dictionary<string, Action> akceNacitani = [];
        private readonly HashSet<string> nacteno = [];

        /// <summary>
        /// Cesta ke konfiguračnímu souboru v AppData. Slouží jen jako
        /// "ukazatel" na kořenovou složku dat – jeho záloha je uložena
        /// přímo v datové složce uživatele (viz ZalohaKonfigu), protože
        /// AppData je smazána při reinstalaci aplikace/systému.
        /// </summary>
        private static readonly string KonfigSoubor =
                 Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 "EvidenceZOOCviceniUpraveno2",
                 "config.ini");

        private static readonly JsonSerializerOptions ZamestnanecJsonOptions = new()
        {
            Converters = { new DateOnlyConverter() }
        };

        private static readonly JsonSerializerOptions ZamestnanecJsonOptionsIndented = new()
        {
            WriteIndented = true,
            Converters = { new DateOnlyConverter() }
        };

        private static readonly JsonSerializerOptions ZvireJsonOptions = new()
        {
            Converters = { new DateOnlyConverter() }
        };

        private static readonly JsonSerializerOptions ZvireJsonOptionsIndented = new()
        {
            WriteIndented = true,
            Converters = { new DateOnlyConverter() }
        };

        private static readonly JsonSerializerOptions SkladJsonOptions = new();

        private static readonly JsonSerializerOptions SkladJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

        private static readonly JsonSerializerOptions HistorieJsonOptions = new();

        private static readonly JsonSerializerOptions HistorieJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

        private static readonly JsonSerializerOptions PokladnaJsonOptions = new();
        
        private static readonly JsonSerializerOptions PokladnaJsonOptionsIndented = new()
        {
            WriteIndented = true
        };
        
        private static readonly JsonSerializerOptions CenikJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

        private static readonly JsonSerializerOptions LogJsonOptions = new();
        
        private static readonly JsonSerializerOptions LogJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

        // ----------------------------------------------------------------
        // METODY PRO PRÁCI S KLÍČI (registrace a zajištění načtení modulů)
        // ----------------------------------------------------------------

        public void RegistrujNacitani(string klic, Action akce)
        {
            akceNacitani[klic] = akce;
        }

        public void ZajistiData(string klic)
        {
            if (!nacteno.Contains(klic))
            {
                if (akceNacitani.TryGetValue(klic, out var akce))
                {
                    akce.Invoke();
                    nacteno.Add(klic);
                }
                else
                {
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
        /// Zjistí kořenovou složku pro data. Při prvním spuštění na daném
        /// počítači (chybí config.ini v AppData) se uživatele zeptá na
        /// cestu; pokud v ní najde zálohu config.ini (např. po reinstalaci
        /// aplikace nebo systému), obnoví z ní nastavení automaticky, aniž
        /// by uživatel o cokoliv přišel. Pokud záloha neexistuje, jde
        /// o čerstvou instalaci a vytvoří se kompletní nová struktura složek.
        /// </summary>
        public static string NactiNeboSeZeptejNaCesty()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(KonfigSoubor)!);

            if (File.Exists(KonfigSoubor))
            {
                var hodnoty = NactiKlicoveHodnoty();
                if (hodnoty.TryGetValue("korenovaSlozka", out var ulozena)
                    && !string.IsNullOrWhiteSpace(ulozena))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Načtena uložená složka dat z: {KonfigSoubor}");
                    Console.ResetColor();
                    return ulozena;
                }
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=== NASTAVENÍ SLOŽKY PRO DATA ===");
            Console.ResetColor();
            Console.Write("Zadej cestu ke složce pro ukládání dat: ");
            string slozka = Console.ReadLine()!.Trim();

            string zalohaKonfigu = Path.Combine(slozka, "Zalohy", "config.ini.bak");

            if (File.Exists(zalohaKonfigu))
            {
                File.Copy(zalohaKonfigu, KonfigSoubor, overwrite: true);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("V zadané složce byla nalezena záloha nastavení – config.ini obnoven, nic nebylo ztraceno.");
                Console.ResetColor();

                VytvorStrukturuSlozek(slozka);
                return slozka;
            }

            // Čerstvá instalace – žádná záloha nastavení nenalezena
            VytvorStrukturuSlozek(slozka);
            ZapisKonfigSoubor([$"korenovaSlozka={slozka}"], slozka);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Nastaveno. Data budou ukládána do: {slozka}");
            Console.ResetColor();
            return slozka;
        }

        /// <summary>
        /// Vytvoří kompletní adresářovou strukturu pro data a jejich
        /// zálohy uvnitř zadané kořenové složky, včetně podsložek
        /// pro jednotlivé moduly (zaměstnanci, zvířata, sklad, účetnictví
        /// a jeho podsložka pokladna).
        /// </summary>
        private static void VytvorStrukturuSlozek(string korenovaSlozka)
        {
            string[] podslozky =
            [
                "Zamestnanci",
                "Zvirata",
                "Sklad",
                "Log",
                Path.Combine("Ucetnictvi", "Pokladna")
            ];
        
            foreach (string zaklad in new[] { "Data", "Zalohy" })
                foreach (string podslozka in podslozky)
                    Directory.CreateDirectory(Path.Combine(korenovaSlozka, zaklad, podslozka));
        
            Directory.CreateDirectory(Path.Combine(korenovaSlozka, "Archiv"));
        }

        /// <summary>
        /// Načte všechny klíč=hodnota páry z config.ini do slovníku.
        /// </summary>
        private static Dictionary<string, string> NactiKlicoveHodnoty()
        {
            var hodnoty = new Dictionary<string, string>();

            if (!File.Exists(KonfigSoubor))
                return hodnoty;

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

            return hodnoty;
        }

        /// <summary>
        /// Zapíše config.ini v AppData bezpečně (přes dočasný soubor
        /// a atomickou náhradu) a zároveň synchronizuje jeho zálohu
        /// do datové složky uživatele (Zalohy/config.ini.bak).
        /// </summary>
        private static void ZapisKonfigSoubor(IEnumerable<string> radky, string korenovaSlozka)
        {
            string docasny = KonfigSoubor + ".tmp";
            File.WriteAllLines(docasny, radky);

            if (File.Exists(KonfigSoubor))
                File.Replace(docasny, KonfigSoubor, null);
            else
                File.Move(docasny, KonfigSoubor);

            if (!string.IsNullOrWhiteSpace(korenovaSlozka))
            {
                string zalohaSlozka = Path.Combine(korenovaSlozka, "Zalohy");
                Directory.CreateDirectory(zalohaSlozka);
                File.Copy(KonfigSoubor, Path.Combine(zalohaSlozka, "config.ini.bak"), overwrite: true);
            }
        }

        /// <summary>
        /// Zapíše nebo aktualizuje jeden klíč v config.ini, aniž by
        /// smazal ostatní existující klíče. Zároveň synchronizuje
        /// zálohu config.ini v datové složce.
        /// </summary>
        private void UlozKlic(string klic, string hodnota)
        {
            var radky = File.Exists(KonfigSoubor)
                ? File.ReadAllLines(KonfigSoubor).ToList()
                : [];

            bool nalezeno = false;
            for (int i = 0; i < radky.Count; i++)
            {
                if (radky[i].TrimStart().StartsWith(klic + "="))
                {
                    radky[i] = $"{klic}={hodnota}";
                    nalezeno = true;
                    break;
                }
            }

            if (!nalezeno)
                radky.Add($"{klic}={hodnota}");

            ZapisKonfigSoubor(radky, KorenovaSlozka);
        }

        // ------------------------------
        // VEŘEJNÉ METODY PRO NAČTENÍ DAT
        // ------------------------------

        public void NactiZamestnance()
        {
            Zamestnanci = NactiZeSouboru(
                SouborZamestnanci, ZalohaZamestnanci,
                json => JsonSerializer.Deserialize<List<Zamestnanec>>(json, ZamestnanecJsonOptions) ?? [],
                "zaměstnanců");
        }

        public void NactiZvirata()
        {
            Zvirata = NactiZeSouboru(
                SouborZvirata, ZalohaZvirata,
                json => JsonSerializer.Deserialize<List<Zvire>>(json, ZvireJsonOptions) ?? [],
                "zvířat");
        }

        public void NactiSklad()
        {
            Sklad = NactiZeSouboru(
                SouborSkladu, ZalohaSkladu,
                json => JsonSerializer.Deserialize<List<SkladovaPolozka>>(json, SkladJsonOptions) ?? [],
                "skladu");

            SkladovaHistorie = NactiZeSouboru(
                SouborSkladoveHistorie, ZalohaSkladoveHistorie,
                json => JsonSerializer.Deserialize<List<SkladovyPohyb>>(json, HistorieJsonOptions) ?? [],
                "historie skladu");
        }

        public void NactiPokladnu()
        {
            PokladniPohyby = NactiZeSouboru(
                SouborPokladny, ZalohaPokladny,
                json => JsonSerializer.Deserialize<List<PokladniPohyb>>(json, PokladnaJsonOptions) ?? [],
                "pokladny");
        
            NactiCenik();
        }

        /// <summary>
        /// Načte ceník ze souboru. Pokud soubor neexistuje (první spuštění),
        /// zůstane zachován výchozí ceník s předvyplněnými hodnotami.
        /// Pokud je soubor poškozený, zkusí obnovit ceník ze zálohy.
        /// </summary>
        private void NactiCenik()
        {
            if (!File.Exists(SouborCeniku) && File.Exists(ZalohaCeniku))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SouborCeniku)!);
                File.Copy(ZalohaCeniku, SouborCeniku);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Ceník obnoven ze zálohy.");
                Console.ResetColor();
            }
        
            if (!File.Exists(SouborCeniku))
                return; // zůstává výchozí ceník definovaný v Cenik.cs
        
            try
            {
                string json = File.ReadAllText(SouborCeniku);
                Cenik = JsonSerializer.Deserialize<Cenik>(json) ?? new Cenik();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání ceníku: {ex.Message}");
                Console.ResetColor();
        
                if (File.Exists(ZalohaCeniku))
                {
                    try
                    {
                        string json = File.ReadAllText(ZalohaCeniku);
                        Cenik = JsonSerializer.Deserialize<Cenik>(json) ?? new Cenik();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Ceník úspěšně obnoven ze zálohy.");
                        Console.ResetColor();
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Záloha ceníku je také poškozená, používám výchozí hodnoty.");
                        Console.ResetColor();
                    }
                }
            }
        }

        public void NactiLogy()
        {
            AuditLog = NactiZeSouboru(
                SouborAuditLogu, ZalohaAuditLogu,
                json => JsonSerializer.Deserialize<List<AuditZaznam>>(json, LogJsonOptions) ?? [],
                "auditního logu");
        
            TechnickyLog = NactiZeSouboru(
                SouborTechnickehoLogu, ZalohaTechnickehoLogu,
                json => JsonSerializer.Deserialize<List<TechnickyZaznam>>(json, LogJsonOptions) ?? [],
                "technického logu");
        }

        /// <summary>
        /// Obecná načítací logika pro libovolný datový soubor: pokud
        /// hlavní soubor chybí, ale záloha existuje, obnoví ji na místo
        /// hlavního souboru. Pokud se hlavní soubor nepodaří deserializovat
        /// (poškozený obsah), automaticky zkusí načíst ze zálohy.
        /// </summary>
        private static List<T> NactiZeSouboru<T>(string hlavniSoubor, string zalohaSoubor,
            Func<string, List<T>> deserializace, string popisProHlasky)
        {
            if (!File.Exists(hlavniSoubor) && File.Exists(zalohaSoubor))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(hlavniSoubor)!);
                File.Copy(zalohaSoubor, hlavniSoubor);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Soubor {popisProHlasky} obnoven ze zálohy.");
                Console.ResetColor();
            }

            if (!File.Exists(hlavniSoubor))
                return [];

            try
            {
                string json = File.ReadAllText(hlavniSoubor);
                return deserializace(json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání {popisProHlasky}: {ex.Message}");
                Console.ResetColor();
                return NactiZeZalohy(zalohaSoubor, deserializace, popisProHlasky);
            }
        }

        private static List<T> NactiZeZalohy<T>(string zalohaSoubor,
            Func<string, List<T>> deserializace, string popisProHlasky)
        {
            if (!File.Exists(zalohaSoubor))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"!!! POZOR: Data ({popisProHlasky}) se nepodařilo načíst ani ze zálohy !!!");
                Console.ResetColor();
                return [];
            }

            try
            {
                string json = File.ReadAllText(zalohaSoubor);
                var data = deserializace(json);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Data ({popisProHlasky}) úspěšně obnovena ze zálohy.");
                Console.ResetColor();
                return data;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Záloha ({popisProHlasky}) je také poškozená: {ex.Message}");
                Console.ResetColor();
                return [];
            }
        }

        // -----------------------------
        // VEŘEJNÉ METODY PRO UKLÁDÁNÍ DAT
        // -----------------------------

        public void UlozZamestnance()
        {
            UlozDoSouboru(Zamestnanci, ZamestnanecJsonOptionsIndented,
                SouborZamestnanci, ZalohaZamestnanci, "zaměstnanců");
        }

        public void UlozZvirata()
        {
            UlozDoSouboru(Zvirata, ZvireJsonOptionsIndented,
                SouborZvirata, ZalohaZvirata, "zvířat");
        }

        public void UlozSklad()
        {
            UlozDoSouboru(Sklad, SkladJsonOptionsIndented,
                SouborSkladu, ZalohaSkladu, "skladu");
        }

        public void UlozSkladovouHistorii()
        {
            UlozDoSouboru(SkladovaHistorie, HistorieJsonOptionsIndented,
                SouborSkladoveHistorie, ZalohaSkladoveHistorie, "historie skladu");
        }

        /// <summary>
        /// Uloží sklad i historii skladových pohybů jako jednu logickou jednotku.
        /// Pokud se nepodaří uložit historii poté, co už byl úspěšně uložen sklad,
        /// obnoví sklad zpět z jeho čerstvě vytvořené zálohy, aby oba soubory
        /// zůstaly navzájem konzistentní.
        /// </summary>
        public void UlozSkladSHistorii()
        {
            bool skladUlozen = false;
        
            try
            {
                string jsonSklad = JsonSerializer.Serialize(Sklad, SkladJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborSkladu, ZalohaSkladu, jsonSklad);
                skladUlozen = true;
        
                string jsonHistorie = JsonSerializer.Serialize(SkladovaHistorie, HistorieJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborSkladoveHistorie, ZalohaSkladoveHistorie, jsonHistorie);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání skladu/historie: {ex.Message}");
                Console.ResetColor();
        
                if (skladUlozen && File.Exists(ZalohaSkladu))
                {
                    // Historie se nepovedla uložit - vrať sklad zpět na předchozí
                    // stav, aby soubory na disku zůstaly vzájemně konzistentní.
                    File.Copy(ZalohaSkladu, SouborSkladu, overwrite: true);
                }
        
                throw; // předej výjimku dál, ať ji zachytí Transakce.ProvedSUlozenim a udělá rollback v paměti
            }
        }

        public void UlozPokladnu()
        {
            UlozDoSouboru(PokladniPohyby, PokladnaJsonOptionsIndented,
                SouborPokladny, ZalohaPokladny, "pokladny");
        }
        
        public void UlozCenik()
        {
            try
            {
                string json = JsonSerializer.Serialize(Cenik, CenikJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborCeniku, ZalohaCeniku, json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání ceníku: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Přidá nový záznam do auditního logu a okamžitě jej uloží.
        /// Log je append-only – existující záznamy se nikdy neupravují ani nemažou.
        /// </summary>
        public void ZapisAudit(AuditZaznam zaznam)
        {
            AuditLog.Add(zaznam);
            UlozDoSouboru(AuditLog, LogJsonOptionsIndented, SouborAuditLogu, ZalohaAuditLogu, "auditního logu");
        }
        
        /// <summary>
        /// Přidá nový záznam do technického logu a okamžitě jej uloží.
        /// </summary>
        public void ZapisTechnickyLog(UrovenLogu uroven, string zprava)
        {
            TechnickyLog.Add(new TechnickyZaznam(uroven, zprava, DateTime.Now));
            UlozDoSouboru(TechnickyLog, LogJsonOptionsIndented, SouborTechnickehoLogu, ZalohaTechnickehoLogu, "technického logu");
        }

        private static void UlozDoSouboru<T>(T data, JsonSerializerOptions options,
            string cilovySoubor, string zalohovySoubor, string popisProHlasky)
        {
            try
            {
                string json = JsonSerializer.Serialize(data, options);
                ZapisSouborSeZalohou(cilovySoubor, zalohovySoubor, json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání ({popisProHlasky}): {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Bezpečně zapíše text do cílového souboru: zapíše nový obsah
        /// do dočasného souboru (.tmp) a atomicky ho nahradí na místo
        /// cílového souboru, přičemž File.Replace zároveň přesune
        /// předchozí obsah cílového souboru do zálohy (i pokud leží
        /// v jiné složce, např. paralelní strom Zalohy/...).
        /// </summary>
        private static void ZapisSouborSeZalohou(string cilovySoubor, string zalohovySoubor, string obsah)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(cilovySoubor)!);
            Directory.CreateDirectory(Path.GetDirectoryName(zalohovySoubor)!);

            string docasny = cilovySoubor + ".tmp";
            File.WriteAllText(docasny, obsah);

            if (File.Exists(cilovySoubor))
                File.Replace(docasny, cilovySoubor, zalohovySoubor);
            else
                File.Move(docasny, cilovySoubor);
        }

        // -----------------------------
        // ROČNÍ ARCHIVACE DAT
        // -----------------------------

        /// <summary>
        /// Zkontroluje, zda je potřeba provést roční archivaci dat.
        /// Volá se jednou při startu aplikace.
        /// </summary>
        public void ZkontrolujRocniArchivaci()
        {
            Directory.CreateDirectory(ArchivSlozka);

            int aktualniRok = DateTime.Now.Year;
            int posledniArchivovanyRok = NactiPosledniArchivovanyRok();

            if (posledniArchivovanyRok == 0)
            {
                UlozKlic("posledniArchivovanyRok", (aktualniRok - 1).ToString());
                return;
            }

            while (posledniArchivovanyRok < aktualniRok - 1)
            {
                int rokKArchivaci = posledniArchivovanyRok + 1;
                ArchivujRok(rokKArchivaci);
                posledniArchivovanyRok = rokKArchivaci;
                UlozKlic("posledniArchivovanyRok", posledniArchivovanyRok.ToString());
            }

            VycistiStareArchivy(aktualniRok);
        }

        private static int NactiPosledniArchivovanyRok()
        {
            var hodnoty = NactiKlicoveHodnoty();
            if (hodnoty.TryGetValue("posledniArchivovanyRok", out var hodnota)
                && int.TryParse(hodnota, out int rok))
                return rok;

            return 0;
        }

        private void ArchivujRok(int rok)
        {
            ArchivujSoubor(SouborZamestnanci, "zamestnanci", rok);
            ArchivujSoubor(SouborZvirata, "zvirata", rok);
            ArchivujSoubor(SouborSkladu, "sklad", rok);
            ArchivujSoubor(SouborSkladoveHistorie, "sklad_historie", rok);
            ArchivujSoubor(SouborPokladny, "pokladna", rok);
            ArchivujSoubor(SouborAuditLogu, "audit", rok);
            ArchivujSoubor(SouborTechnickehoLogu, "technicky_log", rok);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Data za rok {rok} byla archivována do složky: {ArchivSlozka}");
            Console.ResetColor();
        }

        private void ArchivujSoubor(string zdrojovySoubor, string nazevBaze, int rok)
        {
            if (!File.Exists(zdrojovySoubor))
                return;

            string cil = Path.Combine(ArchivSlozka, $"{nazevBaze}_{rok}.json");

            try
            {
                File.Copy(zdrojovySoubor, cil, overwrite: true);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při archivaci souboru '{nazevBaze}' za rok {rok}: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void VycistiStareArchivy(int aktualniRok)
        {
            if (!Directory.Exists(ArchivSlozka))
                return;
        
            foreach (string soubor in Directory.GetFiles(ArchivSlozka, "*_*.json"))
            {
                string jmenoSouboru = Path.GetFileNameWithoutExtension(soubor);
        
                // Auditní log se z retenční politiky vyjímá - jde o trvalý doklad
                if (jmenoSouboru.StartsWith("audit_"))
                    continue;
        
                int podtrzitko = jmenoSouboru.LastIndexOf('_');
                if (podtrzitko < 0) continue;
        
                if (!int.TryParse(jmenoSouboru[(podtrzitko + 1)..], out int rokSouboru))
                    continue;
        
                if (aktualniRok - rokSouboru > 5)
                {
                    try
                    {
                        File.Delete(soubor);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Starý archivní soubor smazán: {Path.GetFileName(soubor)}");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Nepodařilo se smazat archivní soubor {Path.GetFileName(soubor)}: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
        }

        // -----------------------------
        // STATISTIKY
        // -----------------------------

        public void MenuStatistiky()
        {
            ZajistiData("Zaměstnanci");
            ZajistiData("Zvířata");
            ZajistiData("Pokladna");
            char volba;
            do
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n=== STATISTIKY ===");
                Console.WriteLine("\t1. Počet zvířat");
                Console.WriteLine("\t2. Počet zaměstnanců");
                Console.WriteLine("\t3. Součet mezd zaměstnanců");
                Console.WriteLine("\t4. Průměrná denní návštěvnost (za měsíc)");
                Console.WriteLine("\t5. Návrat do hlavního menu");
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
                        int rokNavstevnost = UpravaVstupu.ZeptejSeAUprav(
                            DateTime.Now.Year, "rok (např. 2026)",
                            v => v.ToString(), s => int.Parse(s), jeNove: true);

                        int mesicNavstevnost = UpravaVstupu.ZeptejSeAUprav(
                            DateTime.Now.Month, "měsíc (1-12)",
                            v => v.ToString(),
                            s =>
                            {
                                int m = int.Parse(s);
                                if (m < 1 || m > 12)
                                    throw new ArgumentOutOfRangeException(nameof(s), "Měsíc musí být 1-12.");
                                return m;
                            },
                            jeNove: true);

                        double navstevnost = PrumernaDenniNavstevnost(rokNavstevnost, mesicNavstevnost);
                                                
                        Console.WriteLine($"Průměrná denní návštěvnost za {mesicNavstevnost}/{rokNavstevnost}: {navstevnost:0.##} osob/den");
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Neplatná volba, opakujte zadání:");
                        Console.ResetColor();
                        break;
                }
            }
            while (volba != '5');
        }

        public int PocetZvirat() => Zvirata.Count;
        public int PocetZamestnancu() => Zamestnanci.Count;
        public int SoucetMezd() => Zamestnanci.Sum(z => z.Mzda);    
        public double PrumernaDenniNavstevnost(int rok, int mesic)
        {
            var pohybyVMesici = PokladniPohyby
                .Where(p => p.DatumCas.Year == rok && p.DatumCas.Month == mesic)
                .ToList();
        
            if (pohybyVMesici.Count == 0)
                return 0;
        
            int celkemOsob = pohybyVMesici.Sum(p =>
            {
                int pocetOsob = p.TypVstupenky is TypVstupenky.Detska or TypVstupenky.Dospela or TypVstupenky.ZTP or TypVstupenky.Duchodce
                    ? p.PocetKusu
                    : p.PocetDospelych + p.PocetDeti;
        
                return p.TypPohybu == PokladniTypPohybu.Prodej ? pocetOsob : -pocetOsob;
            });
        
            int pocetDniVMesici = DateTime.DaysInMonth(rok, mesic);
            return (double)celkemOsob / pocetDniVMesici;
        }
    }
}