using System.Text.Json;

namespace EvidenceZOOCviceniUpraveno2.Data
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
                File.Copy(zalohaSoubor, hlavniSoubor);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Soubor {popisProHlasky} obnoven ze zálohy.");
                Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání {popisProHlasky}: {ex.Message}");
                Console.ResetColor();
                return NactiZeZalohy(zalohaSoubor, deserializace, popisProHlasky);
            }
        }

        public static List<T> NactiZeZalohy<T>(string zalohaSoubor,
            Func<string, List<T>> deserializace, string popisProHlasky)
        {
            if (!File.Exists(zalohaSoubor))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"!!! POZOR: Data ({popisProHlasky}) se nepodařilo načíst ani ze zálohy !!!");
                Console.ResetColor();
                return [];
            }

            try
            {
                string json = File.ReadAllText(zalohaSoubor);
                var data = deserializace(json);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Data ({popisProHlasky}) úspěšně obnovena ze zálohy.");
                Console.ResetColor();
                return data;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Záloha ({popisProHlasky}) je také poškozená: {ex.Message}");
                Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání ({popisProHlasky}): {ex.Message}");
                Console.ResetColor();
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