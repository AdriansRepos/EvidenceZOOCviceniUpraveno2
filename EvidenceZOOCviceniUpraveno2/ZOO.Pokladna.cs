using System.Text.Json;

namespace EvidenceZOOCviceniUpraveno2
{
    partial class ZOO
    {
        // -----------------------------
        // CESTY K DATOVÝM SOUBORŮM (Data/...)
        // -----------------------------

        public string SouborPokladny => Path.Combine(KorenovaSlozka, "Data", "Ucetnictvi", "Pokladna", "pokladna.json");
        public string SouborCeniku => Path.Combine(KorenovaSlozka, "Data", "Ucetnictvi", "Pokladna", "cenik.json");

        // -----------------------------
        // CESTY K ZÁLOHÁM (Zalohy/...)
        // -----------------------------

        private string ZalohaPokladny => Path.Combine(KorenovaSlozka, "Zalohy", "Ucetnictvi", "Pokladna", "pokladna.json.bak");
        private string ZalohaCeniku => Path.Combine(KorenovaSlozka, "Zalohy", "Ucetnictvi", "Pokladna", "cenik.json.bak");

        private static readonly JsonSerializerOptions PokladnaJsonOptions = new();

        private static readonly JsonSerializerOptions PokladnaJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

        private static readonly JsonSerializerOptions CenikJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

        public void NactiPokladnu()
        {
            PokladniPohyby = NactiZeSouboru(
                SouborPokladny, ZalohaPokladny,
                json => JsonSerializer.Deserialize<List<PokladniPohyb>>(json, PokladnaJsonOptions) ?? [],
                "pokladny");

            NactiCenik();
        }

        /// <summary>
        /// Načte ceník ze souboru. Pokud soubor neexistuje (první spuštění),
        /// zůstane zachován výchozí ceník s předvyplněnými hodnotami.
        /// Pokud je soubor poškozený, zkusí obnovit ceník ze zálohy.
        /// </summary>
        private void NactiCenik()
        {
            if (!File.Exists(SouborCeniku) && File.Exists(ZalohaCeniku))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SouborCeniku)!);
                File.Copy(ZalohaCeniku, SouborCeniku);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Ceník obnoven ze zálohy.");
                Console.ResetColor();
            }

            if (!File.Exists(SouborCeniku))
                return;

            try
            {
                string json = File.ReadAllText(SouborCeniku);
                Cenik = JsonSerializer.Deserialize<Cenik>(json) ?? new Cenik();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání ceníku: {ex.Message}");
                Console.ResetColor();

                if (File.Exists(ZalohaCeniku))
                {
                    try
                    {
                        string json = File.ReadAllText(ZalohaCeniku);
                        Cenik = JsonSerializer.Deserialize<Cenik>(json) ?? new Cenik();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Ceník úspěšně obnoven ze zálohy.");
                        Console.ResetColor();
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Záloha ceníku je také poškozená, používám výchozí hodnoty.");
                        Console.ResetColor();
                    }
                }
            }
        }

        public void UlozPokladnu()
        {
            UlozDoSouboru(PokladniPohyby, PokladnaJsonOptionsIndented,
                SouborPokladny, ZalohaPokladny, "pokladny");
        }

        public void UlozCenik()
        {
            try
            {
                string json = JsonSerializer.Serialize(Cenik, CenikJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborCeniku, ZalohaCeniku, json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání ceníku: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
