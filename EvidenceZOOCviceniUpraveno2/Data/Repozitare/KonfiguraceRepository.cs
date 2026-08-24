using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Data.Repozitare
{
    /// <summary>
    /// Spravuje config.ini, cestu ke kořenové datové složce, adresářovou
    /// strukturu a volitelnou cestu k externí záloze. Ostatní repository
    /// třídy dostávají KorenovaSlozka jako parametr konstruktoru.
    /// </summary>
    class KonfiguraceRepository(string korenovaSlozka)
    {
        public string KorenovaSlozka { get; private set; } = korenovaSlozka;

        public string ArchivSlozka => Path.Combine(KorenovaSlozka, "Archiv");

        public string? ExterniZalohaSlozka { get; private set; }

        public static string NouzovaZalohaSlozka =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "EvidenceZOOCviceniUpraveno2", "NouzovaZaloha");

        private static readonly string KonfigSoubor =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EvidenceZOOCviceniUpraveno2", "config.ini");

        /// <summary>
        /// Zjistí kořenovou složku pro data. Při prvním spuštění (chybí
        /// config.ini v AppData) se zeptá na cestu; pokud v ní najde
        /// zálohu config.ini, obnoví nastavení automaticky.
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
                    VypisyDoKonzole.VypisUspech($"Načtena uložená složka dat z: {KonfigSoubor}");                    
                    return ulozena;
                }
            }
                        
            VypisyDoKonzole.VypisHlavickuMenu("NASTAVENÍ SLOŽKY PRO DATA");             
            Console.Write("\nZadej cestu ke složce pro ukládání dat: ");
            string slozka = Console.ReadLine()!.Trim();

            string zalohaKonfigu = ZalohaKonfigu(slozka);

            if (File.Exists(zalohaKonfigu))
            {
                File.Copy(zalohaKonfigu, KonfigSoubor, overwrite: true);                
                VypisyDoKonzole.VypisUspech("V zadané složce byla nalezena záloha nastavení – config.ini obnoven, nic nebylo ztraceno.");                

                VytvorStrukturuSlozek(slozka);
                return slozka;
            }

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

        public static Dictionary<string, string> NactiKlicoveHodnoty()
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

        public static void ZapisKonfigSoubor(IEnumerable<string> radky, string korenovaSlozka)
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
        /// smazal ostatní existující klíče.
        /// </summary>
        public void UlozKlic(string klic, string hodnota)
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

        /// <summary>
        /// Vrátí cestu k záloze config.ini uložené v datové složce
        /// uživatele (přežije reinstalaci aplikace/systému).
        /// </summary>
        public static string ZalohaKonfigu(string korenovaSlozka)
            => Path.Combine(korenovaSlozka, "Zalohy", "config.ini.bak");

        public void NactiExterniZalohuCestu()
        {
            var hodnoty = NactiKlicoveHodnoty();
            if (hodnoty.TryGetValue("externiZaloha", out var cesta) && !string.IsNullOrWhiteSpace(cesta))
                ExterniZalohaSlozka = cesta;
        }

        public void NastavitExterniZalohuCestu(string cesta)
        {
            ExterniZalohaSlozka = string.IsNullOrWhiteSpace(cesta) ? null : cesta;
            UlozKlic("externiZaloha", ExterniZalohaSlozka ?? "");
        }
    }
}