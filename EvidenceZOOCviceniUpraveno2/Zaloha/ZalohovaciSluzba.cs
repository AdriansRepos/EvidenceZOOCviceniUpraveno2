using EvidenceZOOCviceniUpraveno2.Data.Repozitare;
using EvidenceZOOCviceniUpraveno2.Entity;
using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Zaloha
{
    /// <summary>
    /// Zajišťuje kompletní manuální/defenzivní zálohování datového adresáře ZOO
    /// do místního úložiště AppData a na volitelné externí médium.
    /// </summary>
    class ZalohovaciSluzba(Zoo zoo)
    {
        private readonly Zoo zoo = zoo;
               
        // -----------------------------
        // DEFENZIVNÍ ZÁLOHOVÁNÍ (ruční)
        // -----------------------------

        /// <summary>
        /// Provede ruční zálohu celé datové složky (Data/) do nouzové zálohy
        /// v LOCALAPPDATA a případně i na volitelnou externí cestu, pokud je
        /// nastavena. Chrání proti ztrátě/poškození celé kořenové složky,
        /// nezávisle na běžné crash-safe záloze u jednotlivých souborů.
        /// </summary>
        public void ProvedRucniZalohu()
        {
            string zdroj = Path.Combine(zoo.Konfigurace.KorenovaSlozka, "Data");

            if (!Directory.Exists(zdroj))
            {                
                VypisyDoKonzole.VypisInformaci("Datová složka neexistuje, není co zálohovat.");                
                return;
            }

            // 1. Záloha do systémového LocalAppData
            ZalohujDoSlozky(zdroj, KonfiguraceRepository.NouzovaZalohaSlozka, "nouzové zálohy (LOCALAPPDATA)");

            // 2. Záloha na uživatelsky definované externí úložiště (USB, síťový disk atd.)
            if (!string.IsNullOrWhiteSpace(zoo.Konfigurace.ExterniZalohaSlozka))
                ZalohujDoSlozky(zdroj, zoo.Konfigurace.ExterniZalohaSlozka, "externí zálohy");
            else
            {                
                VypisyDoKonzole.VypisInformaci("Externí záloha není nastavena (menu → Nastavit cestu k externí záloze).");                
            }
        }

        /// <summary>
        /// Obalovací metoda pro bezpečné vytvoření složky a spuštění rekurze zálohování.
        /// </summary>
        private static void ZalohujDoSlozky(string zdroj, string cil, string popis)
        {
            try
            {
                Directory.CreateDirectory(cil);
                KopirovatSlozkuRekurzivne(zdroj, cil);
                                
                VypisyDoKonzole.VypisUspech($"Vytvořena kopie ({popis}) do: {cil}");                
            }
            catch (Exception ex)
            {                
                VypisyDoKonzole.VypisVarovani($"Chyba při vytváření {popis}: {ex.Message}");                
            }
        }

        /// <summary>
        /// Rekurzivně zkopíruje obsah zdrojové složky (včetně podsložek)
        /// do cílové složky, přepíše existující soubory.
        /// </summary>
        private static void KopirovatSlozkuRekurzivne(string zdroj, string cil)
        {
            Directory.CreateDirectory(cil);

            // Zkopírování všech souborů v aktuálním adresáři
            foreach (string soubor in Directory.GetFiles(zdroj))
            {
                string cilovySoubor = Path.Combine(cil, Path.GetFileName(soubor));
                File.Copy(soubor, cilovySoubor, overwrite: true);
            }

            // Rekurzivní prochod všemi podadresáři
            foreach (string podslozka in Directory.GetDirectories(zdroj))
            {
                string cilovaPodslozka = Path.Combine(cil, Path.GetFileName(podslozka));
                KopirovatSlozkuRekurzivne(podslozka, cilovaPodslozka);
            }
        }
    }
}