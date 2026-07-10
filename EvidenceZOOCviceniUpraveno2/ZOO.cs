using System.Text.Json;
using DateConverterForJson;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje správu zoologické zahrady – načítání dat, ukládání,
    /// roční archivaci, poskytování statistik a práci se soubory.
    /// </summary>
    /// <param name="souborZam">Cesta k souboru se zaměstnanci.</param>
    /// <param name="souborZvir">Cesta k souboru se zvířaty.</param>
    /// <param name="souborSklad">Cesta k souboru se skladem.</param>
    class ZOO(string souborZam, string souborZvir, string souborSklad)
    {
        public List<Zvire> Zvirata { get; internal set; } = [];
        public List<Zamestnanec> Zamestnanci { get; internal set; } = [];
        public List<SkladovaPolozka> Sklad { get; internal set; } = [];
        public List<SkladovyPohyb> SkladovaHistorie { get; internal set; } = [];

        public string SouborZamestnanci { get; private set; } = souborZam;
        public string SouborZvirata { get; private set; } = souborZvir;
        public string SouborSkladu { get; private set; } = souborSklad;

        /// <summary>
        /// Cesta k souboru s historií skladových pohybů. Odvozena
        /// automaticky ze složky, kde jsou uložena ostatní data.
        /// </summary>
        public string SouborSkladoveHistorie => Path.Combine(SlozkaDat, "sklad_historie.json");

        /// <summary>
        /// Datová složka, ve které jsou uloženy hlavní datové soubory.
        /// </summary>
        public string SlozkaDat => Path.GetDirectoryName(SouborZamestnanci)!;

        /// <summary>
        /// Složka pro roční archivy dat, umístěná uvnitř datové složky.
        /// </summary>
        public string ArchivSlozka => Path.Combine(SlozkaDat, "Archiv");

        private readonly Dictionary<string, Action> akceNacitani = [];
        private readonly HashSet<string> nacteno = [];

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

        public static (string souborZam, string souborZvir, string souborSklad) NactiNeboSeZeptejNaCesty()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(KonfigSoubor)!);

            if (!File.Exists(KonfigSoubor) && File.Exists(KonfigSoubor + ".bak"))
            {
                File.Copy(KonfigSoubor + ".bak", KonfigSoubor);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Config.ini obnoven ze zálohy.");
                Console.ResetColor();
            }

            if (File.Exists(KonfigSoubor))
            {
                var hodnoty = NactiKlicoveHodnoty();

                if (hodnoty.TryGetValue("souborZam", out var zamUlozeny)
                    && hodnoty.TryGetValue("souborZvir", out var zvirUlozeny)
                    && hodnoty.TryGetValue("souborSklad", out var skladUlozeny)
                    && !string.IsNullOrWhiteSpace(zamUlozeny)
                    && !string.IsNullOrWhiteSpace(zvirUlozeny)
                    && !string.IsNullOrWhiteSpace(skladUlozeny))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Načteny uložené cesty z: {KonfigSoubor}");
                    Console.ResetColor();
                    return (zamUlozeny, zvirUlozeny, skladUlozeny);
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
            string sklad = Path.Combine(slozka, "sklad.json");

            ZapisKonfigSeZalohou(zam, zvir, sklad);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Cesty uloženy. Data budou ukládána do: {slozka}");
            Console.ResetColor();
            return (zam, zvir, sklad);
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

        private static void ZapisKonfigSeZalohou(string souborZam, string souborZvir, string souborSklad)
        {
            if (File.Exists(KonfigSoubor))
                File.Copy(KonfigSoubor, KonfigSoubor + ".bak", overwrite: true);

            string docasny = KonfigSoubor + ".tmp";
            File.WriteAllLines(docasny, [
                $"souborZam={souborZam}",
                $"souborZvir={souborZvir}",
                $"souborSklad={souborSklad}"
            ]);

            if (File.Exists(KonfigSoubor))
                File.Replace(docasny, KonfigSoubor, null);
            else
                File.Move(docasny, KonfigSoubor);
        }

        /// <summary>
        /// Zapíše nebo aktualizuje jeden klíč v config.ini, aniž by
        /// smazal ostatní existující klíče.
        /// </summary>
        private static void UlozKlic(string klic, string hodnota)
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

            string docasny = KonfigSoubor + ".tmp";
            File.WriteAllLines(docasny, radky);

            if (File.Exists(KonfigSoubor))
                File.Replace(docasny, KonfigSoubor, KonfigSoubor + ".bak");
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

        public void NactiSklad(string cesta)
        {
            SouborSkladu = cesta;
            Sklad = NactiSkladZeSouboru();
            SkladovaHistorie = NactiSkladovouHistoriiZeSouboru();
        }

        private List<Zamestnanec> NactiZamestnanceZeSouboru()
        {
            if (!File.Exists(SouborZamestnanci) && File.Exists(SouborZamestnanci + ".bak"))
            {
                File.Copy(SouborZamestnanci + ".bak", SouborZamestnanci);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Soubor zaměstnanců obnoven ze zálohy.");
                Console.ResetColor();
            }

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

        private List<Zvire> NactiZvirataZeSouboru()
        {
            if (!File.Exists(SouborZvirata) && File.Exists(SouborZvirata + ".bak"))
            {
                File.Copy(SouborZvirata + ".bak", SouborZvirata);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Soubor zvířat obnoven ze zálohy.");
                Console.ResetColor();
            }

            if (!File.Exists(SouborZvirata))
                return [];

            try
            {
                string json = File.ReadAllText(SouborZvirata);
                return JsonSerializer.Deserialize<List<Zvire>>(json, ZvireJsonOptions) ?? [];
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání zvířat: {ex.Message}");
                Console.ResetColor();
                return NactiZeZalohyZvirata();
            }
        }

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
                var data = JsonSerializer.Deserialize<List<Zvire>>(json, ZvireJsonOptions) ?? [];
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

        private List<SkladovaPolozka> NactiSkladZeSouboru()
        {
            if (!File.Exists(SouborSkladu) && File.Exists(SouborSkladu + ".bak"))
            {
                File.Copy(SouborSkladu + ".bak", SouborSkladu);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Soubor skladu obnoven ze zálohy.");
                Console.ResetColor();
            }

            if (!File.Exists(SouborSkladu))
                return [];

            try
            {
                string json = File.ReadAllText(SouborSkladu);
                return JsonSerializer.Deserialize<List<SkladovaPolozka>>(json, SkladJsonOptions) ?? [];
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání skladu: {ex.Message}");
                Console.ResetColor();
                return NactiZeZalohySkladu();
            }
        }

        private List<SkladovaPolozka> NactiZeZalohySkladu()
        {
            string zaloha = SouborSkladu + ".bak";

            if (!File.Exists(zaloha))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("!!! POZOR: Data skladu se nepodařilo načíst ani ze zálohy !!!");
                Console.ResetColor();
                return [];
            }

            try
            {
                string json = File.ReadAllText(zaloha);
                var data = JsonSerializer.Deserialize<List<SkladovaPolozka>>(json, SkladJsonOptions) ?? [];
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Sklad úspěšně obnoven ze zálohy (.bak).");
                Console.ResetColor();
                return data;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Záloha skladu je také poškozená: {ex.Message}");
                Console.ResetColor();
                return [];
            }
        }

        private List<SkladovyPohyb> NactiSkladovouHistoriiZeSouboru()
        {
            string soubor = SouborSkladoveHistorie;

            if (!File.Exists(soubor) && File.Exists(soubor + ".bak"))
            {
                File.Copy(soubor + ".bak", soubor);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Historie skladu obnovena ze zálohy.");
                Console.ResetColor();
            }

            if (!File.Exists(soubor))
                return [];

            try
            {
                string json = File.ReadAllText(soubor);
                return JsonSerializer.Deserialize<List<SkladovyPohyb>>(json, HistorieJsonOptions) ?? [];
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání historie skladu: {ex.Message}");
                Console.ResetColor();
                return NactiZeZalohySkladoveHistorie();
            }
        }

        private List<SkladovyPohyb> NactiZeZalohySkladoveHistorie()
        {
            string zaloha = SouborSkladoveHistorie + ".bak";

            if (!File.Exists(zaloha))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("!!! POZOR: Historie skladu se nepodařilo načíst ani ze zálohy !!!");
                Console.ResetColor();
                return [];
            }

            try
            {
                string json = File.ReadAllText(zaloha);
                var data = JsonSerializer.Deserialize<List<SkladovyPohyb>>(json, HistorieJsonOptions) ?? [];
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Historie skladu úspěšně obnovena ze zálohy (.bak).");
                Console.ResetColor();
                return data;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Záloha historie skladu je také poškozená: {ex.Message}");
                Console.ResetColor();
                return [];
            }
        }

        // -----------------------------
        // VEŘEJNÉ METODY PRO UKLÁDÁNÍ DAT
        // -----------------------------

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

        public void UlozSklad()
        {
            try
            {
                string json = JsonSerializer.Serialize(Sklad, SkladJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborSkladu, json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání skladu: {ex.Message}");
                Console.ResetColor();
            }
        }

        public void UlozSkladovouHistorii()
        {
            try
            {
                string json = JsonSerializer.Serialize(SkladovaHistorie, HistorieJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborSkladoveHistorie, json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání historie skladu: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void ZapisSouborSeZalohou(string cilovySoubor, string obsah)
        {
            string docasny = cilovySoubor + ".tmp";
            string zaloha = cilovySoubor + ".bak";

            File.WriteAllText(docasny, obsah);

            if (File.Exists(cilovySoubor))
                File.Replace(docasny, cilovySoubor, zaloha);
            else
                File.Move(docasny, cilovySoubor);
        }

        // -----------------------------
        // ROČNÍ ARCHIVACE DAT
        // -----------------------------

        /// <summary>
        /// Zkontroluje, zda je potřeba provést roční archivaci dat
        /// (soubory jsou platné vždy jen do konce kalendářního roku,
        /// bez ohledu na to, kdy byly založeny). Pokud ano, vytvoří
        /// kopie aktuálních souborů do složky Archiv a zároveň odstraní
        /// archivní soubory starší než 5 let. Volá se jednou při startu
        /// aplikace.
        /// </summary>
        public void ZkontrolujRocniArchivaci()
        {
            Directory.CreateDirectory(ArchivSlozka);

            int aktualniRok = DateTime.Now.Year;
            int posledniArchivovanyRok = NactiPosledniArchivovanyRok();

            // První spuštění aplikace vůbec – není co archivovat,
            // jen si zapamatujeme výchozí stav pro budoucí porovnání.
            if (posledniArchivovanyRok == 0)
            {
                UlozKlic("posledniArchivovanyRok", (aktualniRok - 1).ToString());
                return;
            }

            // Dožene i více přeskočených let najednou (např. pokud
            // aplikace nebyla spuštěna přes přelom více let).
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

        /// <summary>
        /// Vytvoří kopie aktuálních datových souborů pro daný rok
        /// ve složce Archiv. Původní pracovní soubory zůstávají
        /// nedotčené a aplikace v nich pokračuje dál.
        /// </summary>
        private void ArchivujRok(int rok)
        {
            ArchivujSoubor(SouborZamestnanci, "zamestnanci", rok);
            ArchivujSoubor(SouborZvirata, "zvirata", rok);
            ArchivujSoubor(SouborSkladu, "sklad", rok);
            ArchivujSoubor(SouborSkladoveHistorie, "sklad_historie", rok);

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

        /// <summary>
        /// Odstraní archivní soubory starší než 5 let vzhledem
        /// k aktuálnímu roku.
        /// </summary>
        private void VycistiStareArchivy(int aktualniRok)
        {
            if (!Directory.Exists(ArchivSlozka))
                return;

            foreach (string soubor in Directory.GetFiles(ArchivSlozka, "*_*.json"))
            {
                string jmenoSouboru = Path.GetFileNameWithoutExtension(soubor);
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

        public int PocetZvirat() => Zvirata.Count;
        public int PocetZamestnancu() => Zamestnanci.Count;
        public int SoucetMezd() => Zamestnanci.Sum(z => z.Mzda);
    }
}