using EvidenceZOOCviceniUpraveno2.Data;

namespace EvidenceZOOCviceniUpraveno2.Zaloha
{
    class ArchivacniSluzba(Zoo zoo)
    {
        private readonly Zoo zoo = zoo;

        /// <summary>
        /// Volitelná cesta k externí/síťové záloze, nastavitelná uživatelem
        /// přes menu. Pokud není nastavena, tento krok zálohování se přeskočí.
        /// </summary>
        public string? ExterniZalohaSlozka { get; internal set; }

        /// <summary>
        /// Načte volitelnou cestu k externí záloze z config.ini, pokud byla
        /// dříve nastavena přes menu.
        /// </summary>
        public void NactiExterniZalohuCestu()
        {
            var hodnoty = KonfiguraceRepository.NactiKlicoveHodnoty();
            if (hodnoty.TryGetValue("externiZaloha", out var cesta) && !string.IsNullOrWhiteSpace(cesta))
                ExterniZalohaSlozka = cesta;
        }

        /// <summary>
        /// Nastaví (nebo změní) cestu k externí záloze a uloží ji do config.ini.
        /// Prázdný nebo whitespace řetězec externí zálohu vypne.
        /// </summary>
        public void NastavitExterniZalohuCestu(string cesta)
        {
            ExterniZalohaSlozka = string.IsNullOrWhiteSpace(cesta) ? null : cesta;
            zoo.Konfigurace.UlozKlic("externiZaloha", zoo.Konfigurace.ExterniZalohaSlozka ?? "");
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
            Directory.CreateDirectory(zoo.Konfigurace.ArchivSlozka);

            int aktualniRok = DateTime.Now.Year;
            int posledniArchivovanyRok = NactiPosledniArchivovanyRok();

            if (posledniArchivovanyRok == 0)
            {
                zoo.Konfigurace.UlozKlic("posledniArchivovanyRok", (aktualniRok - 1).ToString());
                return;
            }

            while (posledniArchivovanyRok < aktualniRok - 1)
            {
                int rokKArchivaci = posledniArchivovanyRok + 1;
                ArchivujRok(rokKArchivaci);
                posledniArchivovanyRok = rokKArchivaci;
                zoo.Konfigurace.UlozKlic("posledniArchivovanyRok", posledniArchivovanyRok.ToString());
            }

            VycistiStareArchivy(aktualniRok);
        }

        private static int NactiPosledniArchivovanyRok()
        {
            var hodnoty = KonfiguraceRepository.NactiKlicoveHodnoty();
            if (hodnoty.TryGetValue("posledniArchivovanyRok", out var hodnota)
                && int.TryParse(hodnota, out int rok))
                return rok;

            return 0;
        }

        private void ArchivujRok(int rok)
        {
            ArchivujSoubor(zoo.Zamestnanci.SouborZamestnanci, "zamestnanci", rok);
            ArchivujSlozku(zoo.Zvirata.SlozkaZvirat, "zvirata", rok);
            ArchivujSoubor(zoo.Sklad.SouborSkladu, "sklad", rok);
            ArchivujSoubor(zoo.Sklad.SouborHistorie, "sklad_historie", rok);
            ArchivujSoubor(zoo.Pokladna.SouborPokladny, "pokladna", rok);
            ArchivujSoubor(zoo.Logy.SouborAuditLogu, "audit", rok);
            ArchivujSoubor(zoo.Logy.SouborTechnickehoLogu, "technicky_log", rok);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Data za rok {rok} byla archivována do složky: {zoo.Konfigurace.ArchivSlozka}");
            Console.ResetColor();
        }

        private void ArchivujSoubor(string zdrojovySoubor, string nazevBaze, int rok)
        {
            if (!File.Exists(zdrojovySoubor))
                return;

            string cil = Path.Combine(zoo.Konfigurace.ArchivSlozka, $"{nazevBaze}_{rok}.json");

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

        private void ArchivujSlozku(string zdrojovaSlozka, string nazevPodslozky, int rok)
        {
            if (!Directory.Exists(zdrojovaSlozka))
                return;
        
            string cil = Path.Combine(zoo.Konfigurace.ArchivSlozka, $"{nazevPodslozky}_{rok}");
        
            try
            {
                Directory.CreateDirectory(cil);
                foreach (string soubor in Directory.GetFiles(zdrojovaSlozka, "*.json"))
                {
                    string cilovySoubor = Path.Combine(cil, Path.GetFileName(soubor));
                    File.Copy(soubor, cilovySoubor, overwrite: true);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při archivaci složky '{nazevPodslozky}' za rok {rok}: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void VycistiStareArchivy(int aktualniRok)
        {
            if (!Directory.Exists(zoo.Konfigurace.ArchivSlozka))
                return;

            foreach (string soubor in Directory.GetFiles(zoo.Konfigurace.ArchivSlozka, "*_*.json"))
            {
                string jmenoSouboru = Path.GetFileNameWithoutExtension(soubor);

                // Auditní log se z retenční politiky vyjímá - jde o trvalý doklad
                if (jmenoSouboru.StartsWith("audit_"))
                    continue;

                int podtrzitko = jmenoSouboru.LastIndexOf('_');
                if (podtrzitko < 0) continue;

                if (!int.TryParse(jmenoSouboru[(podtrzitko + 1)..], out int rokSouboru))
                    continue;

                // Zaměstnanecká/mzdová data podléhají zákonné retenční lhůtě 30 let,
                // ostatní data (zvířata, sklad, pokladna) jen 5 let.
                int hraniceRoku = jmenoSouboru.StartsWith("zamestnanci_") ? 30 : 5;

                if (aktualniRok - rokSouboru > hraniceRoku)
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

            // Složky (zvirata_2026/)
            foreach (string slozka in Directory.GetDirectories(zoo.Konfigurace.ArchivSlozka, "zvirata_*"))
            {
                string nazevSlozky = Path.GetFileName(slozka);
                int podtrzitko = nazevSlozky.LastIndexOf('_');
                if (podtrzitko < 0) continue;

                if (!int.TryParse(nazevSlozky[(podtrzitko + 1)..], out int rokSlozky))
                    continue;

                if (aktualniRok - rokSlozky > 5)
                {
                    try
                    {
                        Directory.Delete(slozka, recursive: true);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Stará archivní složka smazána: {nazevSlozky}");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Nepodařilo se smazat archivní složku {nazevSlozky}: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}