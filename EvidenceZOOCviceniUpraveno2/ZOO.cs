using System.Text.Json;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje správu zoologické zahrady – načítání dat, ukládání,
    /// roční archivaci, poskytování statistik, práci se soubory a s pokladnou pro vstupné.
    /// Data i jejich zálohy jsou organizovány v oddělených složkách
    /// Data/ a Zalohy/ uvnitř zvolené kořenové složky.
    /// </summary>
    /// <param name="korenovaSlozka">Kořenová složka, ve které jsou uložena všechna data a jejich zálohy.</param>
    partial class ZOO(string korenovaSlozka)
    {
        public List<Zvire> Zvirata { get; internal set; } = [];
        public CisloZvireteKonfigurace CisloZvireteKonfigurace { get; internal set; } = new();
        public List<Zamestnanec> Zamestnanci { get; internal set; } = [];
        public CisloZamestnanceKonfigurace CisloZamestnanceKonfigurace { get; internal set; } = new();
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

        /// <summary>
        /// Vrátí cestu k záloze config.ini uložené v datové složce uživatele
        /// (ne v AppData), aby přežila i reinstalaci aplikace nebo systému.
        /// </summary>
        /// <param name="korenovaSlozka">Kořenová složka, ve které jsou uložena data uživatele.</param>
        private static string ZalohaKonfigu(string korenovaSlozka)
            => Path.Combine(korenovaSlozka, "Zalohy", "config.ini.bak");

        /// <summary>
        /// Složka pro roční archivy dat.
        /// </summary>
        public string ArchivSlozka => Path.Combine(KorenovaSlozka, "Archiv");

        /// <summary>
        /// Cesta k nouzové sekundární záloze, uložené mimo kořenovou složku
        /// uživatele (v LOCALAPPDATA). Chrání proti ztrátě/poškození celé
        /// kořenové složky. Aktualizuje se jen na ruční pokyn uživatele, ne
        /// při každém zápisu.
        /// </summary>
        private static string NouzovaZalohaSlozka =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "EvidenceZOOCviceniUpraveno2", "NouzovaZaloha");

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
    }
}