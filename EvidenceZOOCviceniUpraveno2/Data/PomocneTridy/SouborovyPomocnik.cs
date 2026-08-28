using System.Text.Json;
using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Data.PomocneTridy
{
    /// <summary>
    /// Statická pomocná třída poskytující bezpečné metody pro manipulaci se soubory.
    /// Zajišťuje odolnost vůči haváriím (crash-safety) a automatickou obnovu ze záloh.
    /// </summary>
    static class SouborovyPomocnik
    {
        /// <summary>
        /// Pokusí se načíst data z primárního souboru. Pokud soubor chybí nebo je poškozený,
        /// automaticky zahájí obnovu ze záložního souboru.
        /// </summary>
        public static List<T> NactiZeSouboru<T>(string hlavniSoubor, string zalohaSoubor,
            Func<string, List<T>> deserializace, string popisProHlasky)
        {
            // Pokud chybí hlavní soubor, ale záloha existuje, provede její kopii
            if (!File.Exists(hlavniSoubor) && File.Exists(zalohaSoubor))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(hlavniSoubor)!);
                File.Copy(zalohaSoubor, hlavniSoubor);                ;
                VypisyDoKonzole.VypisUspech($"Soubor {popisProHlasky} obnoven ze zálohy.");                
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
                VypisyDoKonzole.VypisVarovani($"Chyba při načítání {popisProHlasky}: {ex.Message}");                
                // Přechod na záložní plan – načtení ze zálohy
                return NactiZeZalohy(zalohaSoubor, deserializace, popisProHlasky);
            }
        }

        /// <summary>
        /// Zajišťuje záložní načtení dat, pokud se selhalo čtení z primárního souboru.
        /// </summary>
        public static List<T> NactiZeZalohy<T>(string zalohaSoubor,
            Func<string, List<T>> deserializace, string popisProHlasky)
        {
            if (!File.Exists(zalohaSoubor))
            {                
                VypisyDoKonzole.VypisVarovani($"!!! POZOR: Data ({popisProHlasky}) se nepodařilo načíst ani ze zálohy !!!");                
                return [];
            }

            try
            {
                string json = File.ReadAllText(zalohaSoubor);
                var data = deserializace(json);                
                VypisyDoKonzole.VypisUspech($"Data ({popisProHlasky}) úspěšně obnovena ze zálohy.");                
                return data;
            }
            catch (Exception ex)
            {                
                VypisyDoKonzole.VypisVarovani($"Záloha ({popisProHlasky}) je také poškozená: {ex.Message}");                
                return [];
            }
        }

        /// <summary>
        /// Serializuje objekt do JSON a zapíše jej do souboru včetně zálohování.
        /// </summary>
        public static void UlozDoSouboru<T>(T data, JsonSerializerOptions options,
            string cilovySoubor, string zalohovySoubor, string popisProHlasky)
        {
            try
            {
                string json = JsonSerializer.Serialize(data, options);
                ZapisSouborSeZalohou(cilovySoubor, zalohovySoubor, json);
            }
            catch (Exception ex)
            {                
                VypisyDoKonzole.VypisVarovani($"Chyba při ukládání ({popisProHlasky}): {ex.Message}");                
            }
        }

        /// <summary>
        /// Provede zápis dat do dočasného souboru (.tmp) a následně atomicky nahradí cílový soubor,
        /// přičemž původní verzi přesune do záložního souboru.
        /// </summary>
        public static void ZapisSouborSeZalohou(string cilovySoubor, string zalohovySoubor, string obsah)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(cilovySoubor)!);
            Directory.CreateDirectory(Path.GetDirectoryName(zalohovySoubor)!);

            // Zápis do dočasného souboru zamezuje poškození souboru v případě přerušení procesu
            string docasny = cilovySoubor + ".tmp";
            File.WriteAllText(docasny, obsah);

            // Výměna dočasného souboru za cílový se zachováním zálohy
            if (File.Exists(cilovySoubor))
                File.Replace(docasny, cilovySoubor, zalohovySoubor);
            else
                File.Move(docasny, cilovySoubor);
        }
    }
}