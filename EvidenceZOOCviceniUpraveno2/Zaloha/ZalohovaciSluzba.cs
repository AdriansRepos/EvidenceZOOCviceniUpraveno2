using EvidenceZOOCviceniUpraveno2.Data.Repozitare;
using EvidenceZOOCviceniUpraveno2.Entity;
using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Zaloha
{
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

            ZalohujDoSlozky(zdroj, KonfiguraceRepository.NouzovaZalohaSlozka, "nouzové zálohy (LOCALAPPDATA)");

            if (!string.IsNullOrWhiteSpace(zoo.Konfigurace.ExterniZalohaSlozka))
                ZalohujDoSlozky(zdroj, zoo.Konfigurace.ExterniZalohaSlozka, "externí zálohy");
            else
            {                
                VypisyDoKonzole.VypisInformaci("Externí záloha není nastavena (menu → Nastavit cestu k externí záloze).");                
            }
        }

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

            foreach (string soubor in Directory.GetFiles(zdroj))
            {
                string cilovySoubor = Path.Combine(cil, Path.GetFileName(soubor));
                File.Copy(soubor, cilovySoubor, overwrite: true);
            }

            foreach (string podslozka in Directory.GetDirectories(zdroj))
            {
                string cilovaPodslozka = Path.Combine(cil, Path.GetFileName(podslozka));
                KopirovatSlozkuRekurzivne(podslozka, cilovaPodslozka);
            }
        }
    }
}