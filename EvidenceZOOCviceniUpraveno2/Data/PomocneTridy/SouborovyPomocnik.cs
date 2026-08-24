using System.Text.Json;
using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Data.PomocneTridy
{
    /// <summary>
    /// Sdílené, bezstavové pomocné metody pro čtení a zápis JSON souborů
    /// s automatickou obnovou ze zálohy a crash-safe zápisem.
    /// Používají ji všechny repository třídy.
    /// </summary>
    static class SouborovyPomocnik
    {
        public static List<T> NactiZeSouboru<T>(string hlavniSoubor, string zalohaSoubor,
            Func<string, List<T>> deserializace, string popisProHlasky)
        {
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
                return NactiZeZalohy(zalohaSoubor, deserializace, popisProHlasky);
            }
        }

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

        public static void ZapisSouborSeZalohou(string cilovySoubor, string zalohovySoubor, string obsah)
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