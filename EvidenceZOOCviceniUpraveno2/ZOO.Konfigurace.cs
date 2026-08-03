
namespace EvidenceZOOCviceniUpraveno2
{
    partial class ZOO
    {
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

            string zalohaKonfigu = ZalohaKonfigu(slozka);

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
                string cestaKZaloze = ZalohaKonfigu(korenovaSlozka);
                Directory.CreateDirectory(Path.GetDirectoryName(cestaKZaloze)!);
                File.Copy(KonfigSoubor, cestaKZaloze, overwrite: true);
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
    }
}
