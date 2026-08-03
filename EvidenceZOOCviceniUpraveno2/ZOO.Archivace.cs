using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceZOOCviceniUpraveno2
{
    partial class ZOO
    {
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
        }
    }
}
