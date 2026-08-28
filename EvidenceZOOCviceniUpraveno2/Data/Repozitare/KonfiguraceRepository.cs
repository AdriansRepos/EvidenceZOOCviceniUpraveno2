using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Data.Repozitare
{
    /// <summary>
    /// Spravuje konfiguraci aplikace (config.ini), určuje kořenovou datovou složku,
    /// zajišťuje inicializaci adresářové struktury a správu externích záloh.
    /// </summary>
    class KonfiguraceRepository(string korenovaSlozka)
    {
        /// <summary>
        /// Cesta ke kořenové složce, ve které jsou uložena veškerá data aplikace.
        /// </summary>
        public string KorenovaSlozka { get; private set; } = korenovaSlozka;

        /// <summary>
        /// Absolutní cesta k adresáři určenému pro archivaci.
        /// </summary>
        public string ArchivSlozka => Path.Combine(KorenovaSlozka, "Archiv");

        /// <summary>
        /// Volitelná cesta k externímu adresáři pro provádění záloh.
        /// </summary>
        public string? ExterniZalohaSlozka { get; private set; }

        /// <summary>
        /// Cesta k nouzovému adresáři záloh umístěnému v LocalAppData uživatele.
        /// </summary>
        public static string NouzovaZalohaSlozka =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "EvidenceZOOCviceniUpraveno2", "NouzovaZaloha");

        /// <summary>
        /// Cesta k hlavnímu konfiguračnímu souboru config.ini v AppData.
        /// </summary>
        private static readonly string KonfigSoubor =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EvidenceZOOCviceniUpraveno2", "config.ini");

        /// <summary>
        /// Načte cestu ke kořenové datové složce z config.ini. 
        /// Pokud soubor neexistuje, vyzve uživatele k zadání cesty a pokusí se obnovit konfiguraci ze zálohy.
        /// </summary>

        public static string NactiNeboSeZeptejNaCesty()
        {
            // Zajištění existence adresáře pro uložení konfiguračního souboru
            Directory.CreateDirectory(Path.GetDirectoryName(KonfigSoubor)!);

            // Pokud konfigurační soubor existuje, pokusíme se načíst uloženou cestu
            if (File.Exists(KonfigSoubor))
            {
                var hodnoty = NactiKlicoveHodnoty();
                if (hodnoty.TryGetValue("korenovaSlozka", out var ulozena)
                    && !string.IsNullOrWhiteSpace(ulozena))
                {
                    VypisyDoKonzole.VypisUspech($"Načtena uložená složka dat z: {KonfigSoubor}");                    
                    return ulozena;
                }
            }
                        
            // První spuštění – výzva uživateli k zadání cesty k datům
            VypisyDoKonzole.VypisHlavickuMenu("NASTAVENÍ SLOŽKY PRO DATA");             
            Console.Write("\nZadej cestu ke složce pro ukládání dat: ");
            string slozka = Console.ReadLine()!.Trim();

            // Kontrola, zda v zadané složce neexistuje záloha konfigurace
            string zalohaKonfigu = ZalohaKonfigu(slozka);

            if (File.Exists(zalohaKonfigu))
            {
                // Obnovení konfigurace ze zálohy v datové složce
                File.Copy(zalohaKonfigu, KonfigSoubor, overwrite: true);                
                VypisyDoKonzole.VypisUspech("V zadané složce byla nalezena záloha nastavení – config.ini obnoven, nic nebylo ztraceno.");                

                VytvorStrukturuSlozek(slozka);
                return slozka;
            }

            // Vytvoření adresářové struktury a zápis nové konfigurace
            VytvorStrukturuSlozek(slozka);
            ZapisKonfigSoubor([$"korenovaSlozka={slozka}"], slozka);
                        
            VypisyDoKonzole.VypisUspech($"Nastaveno. Data budou ukládána do: {slozka}");            
            return slozka;
        }

        /// <summary>
        /// Vytvoří kompletní adresářovou strukturu pro data a jejich
        /// zálohy uvnitř zadané kořenové složky.
        /// </summary>
        public static void VytvorStrukturuSlozek(string korenovaSlozka)
        {
            // Názvy základních podsložek pro moduly aplikace
            string[] podslozky =
            [
                "Zamestnanci",
                "Zvirata",
                "Sklad",
                "Log",
                Path.Combine("Ucetnictvi", "Pokladna")
            ];

            // Vytvoření adresářové struktury pro data i zálohy
            foreach (string zaklad in new[] { "Data", "Zalohy" })
                foreach (string podslozka in podslozky)
                    Directory.CreateDirectory(Path.Combine(korenovaSlozka, zaklad, podslozka));

            // Vytvoření samostatné složky pro archiv
            Directory.CreateDirectory(Path.Combine(korenovaSlozka, "Archiv"));
        }

        /// <summary>
        /// Přečte konfigurační soubor config.ini a vrátí obsažené klíče a hodnoty jako slovník.
        /// Během čtení ignoruje prázdné řádky a komentáře.
        /// </summary>

        public static Dictionary<string, string> NactiKlicoveHodnoty()
        {
            var hodnoty = new Dictionary<string, string>();

            if (!File.Exists(KonfigSoubor))
                return hodnoty;

            // Procházení konfiguračního souboru řádek po řádku
            foreach (string radek in File.ReadAllLines(KonfigSoubor))
            {
                string upraven = radek.Trim();
                // Přeskakování prázdných řádků a komentářů začínajících # nebo ;
                if (upraven.Length == 0 || upraven.StartsWith("#") || upraven.StartsWith(";"))
                    continue;

                int idx = upraven.IndexOf('=');
                if (idx <= 0) continue;

                // Rozdělení řádku na klíč a hodnotu podle prvního znaku '='
                string klic = upraven[..idx].Trim();
                string hodnota = upraven[(idx + 1)..].Trim();
                hodnoty[klic] = hodnota;
            }

            return hodnoty;
        }

        /// <summary>
        /// Bezpečně zapíše řádky do souboru config.ini pomocí dočasného souboru a vytvoří jeho zálohu v datové složce.
        /// </summary>

        public static void ZapisKonfigSoubor(IEnumerable<string> radky, string korenovaSlozka)
        {
            // Zápis do dočasného souboru pro prevenci poškození dat při výpadku
            string docasny = KonfigSoubor + ".tmp";
            File.WriteAllLines(docasny, radky);

            // Nahrazení původního souboru novým dočasným souborem
            if (File.Exists(KonfigSoubor))
                File.Replace(docasny, KonfigSoubor, null);
            else
                File.Move(docasny, KonfigSoubor);

            // Synchronizace záložní kopie v datovém adresáři
            if (!string.IsNullOrWhiteSpace(korenovaSlozka))
            {
                string cestaKZaloze = ZalohaKonfigu(korenovaSlozka);
                Directory.CreateDirectory(Path.GetDirectoryName(cestaKZaloze)!);
                File.Copy(KonfigSoubor, cestaKZaloze, overwrite: true);
            }
        }

        /// <summary>
        /// Zapíše nebo aktualizuje jeden klíč v config.ini, aniž by smazal ostatní existující klíče.
        /// </summary>
        public void UlozKlic(string klic, string hodnota)
        {
            var radky = File.Exists(KonfigSoubor)
                ? File.ReadAllLines(KonfigSoubor).ToList()
                : [];

            bool nalezeno = false;
            // Hledání existujícího klíče a jeho aktualizace
            for (int i = 0; i < radky.Count; i++)
            {
                if (radky[i].TrimStart().StartsWith(klic + "="))
                {
                    radky[i] = $"{klic}={hodnota}";
                    nalezeno = true;
                    break;
                }
            }

            // Pokud klíč neexistoval, přidáme jej na konec
            if (!nalezeno)
                radky.Add($"{klic}={hodnota}");

            ZapisKonfigSoubor(radky, KorenovaSlozka);
        }

        /// <summary>
        /// Vrátí cestu k záloze config.ini uložené v datové složce uživatele.
        /// </summary>
        public static string ZalohaKonfigu(string korenovaSlozka)
            => Path.Combine(korenovaSlozka, "Zalohy", "config.ini.bak");

        /// <summary>
        /// Načte nastavenou cestu k externí záloze z konfiguračního souboru.
        /// </summary>
        public void NactiExterniZalohuCestu()
        {
            var hodnoty = NactiKlicoveHodnoty();
            if (hodnoty.TryGetValue("externiZaloha", out var cesta) && !string.IsNullOrWhiteSpace(cesta))
                ExterniZalohaSlozka = cesta;
        }

        /// <summary>
        /// Nastaví novou cestu pro externí zálohu a uloží ji do konfiguračního souboru.
        /// </summary>
        public void NastavitExterniZalohuCestu(string cesta)
        {
            ExterniZalohaSlozka = string.IsNullOrWhiteSpace(cesta) ? null : cesta;
            UlozKlic("externiZaloha", ExterniZalohaSlozka ?? "");
        }
    }
}