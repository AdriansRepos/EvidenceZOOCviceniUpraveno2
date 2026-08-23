using EvidenceZOOCviceniUpraveno2.Data.PomocneTridy;
using EvidenceZOOCviceniUpraveno2.Entity;
using System.Text.Json;

namespace EvidenceZOOCviceniUpraveno2.Data.Repozitare
{
    /// <summary>
    /// Zodpovídá výhradně za perzistenci pokladních pohybů a ceníku.
    /// </summary>
    class PokladnaRepository(string korenovaSlozka)
    {
        private readonly string korenovaSlozka = korenovaSlozka;

        public List<PokladniPohyb> PokladniPohyby { get; internal set; } = [];
        public Cenik Cenik { get; internal set; } = new();

        public string SouborPokladny => Path.Combine(korenovaSlozka, "Data", "Ucetnictvi", "Pokladna", "pokladna.json");
        public string SouborCeniku => Path.Combine(korenovaSlozka, "Data", "Ucetnictvi", "Pokladna", "cenik.json");

        private string ZalohaPokladny => Path.Combine(korenovaSlozka, "Zalohy", "Ucetnictvi", "Pokladna", "pokladna.json.bak");
        private string ZalohaCeniku => Path.Combine(korenovaSlozka, "Zalohy", "Ucetnictvi", "Pokladna", "cenik.json.bak");

        private static readonly JsonSerializerOptions Options = new();

        private static readonly JsonSerializerOptions OptionsIndented = new()
        {
            WriteIndented = true
        };

        private static readonly JsonSerializerOptions CenikOptionsIndented = new()
        {
            WriteIndented = true
        };

        public void Nacti()
        {
            PokladniPohyby = SouborovyPomocnik.NactiZeSouboru(
                SouborPokladny, ZalohaPokladny,
                json => JsonSerializer.Deserialize<List<PokladniPohyb>>(json, Options) ?? [],
                "pokladny");

            NactiCenik();
        }

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
            SouborovyPomocnik.UlozDoSouboru(PokladniPohyby, OptionsIndented,
                SouborPokladny, ZalohaPokladny, "pokladny");
        }

        public void UlozCenik()
        {
            try
            {
                string json = JsonSerializer.Serialize(Cenik, CenikOptionsIndented);
                SouborovyPomocnik.ZapisSouborSeZalohou(SouborCeniku, ZalohaCeniku, json);
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