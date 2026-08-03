using FileNameHelper;
using DateConverterForJson;
using System.Text.Json;


namespace EvidenceZOOCviceniUpraveno2
{
    partial class ZOO
    {
        // -----------------------------
        // CESTY K DATOVÝM SOUBORŮM (Data/...)
        // -----------------------------

        public string SouborZvirata => Path.Combine(KorenovaSlozka, "Data", "Zvirata", "zvirata.json");
        private string SlozkaZvirat => Path.Combine(KorenovaSlozka, "Data", "Zvirata");
        private string SouborCislaZvirete => Path.Combine(SlozkaZvirat, "cislovani.json");

        public string SouborZvirete(Zvire zvire) =>
            Path.Combine(SlozkaZvirat, $"{NazevSouboru.OcistiProNazevSouboru(zvire.Nazev)}_{zvire.Id}.json");

        // -----------------------------
        // CESTY K ZÁLOHÁM (Zalohy/...)
        // -----------------------------

        private string ZalohaZvirata => Path.Combine(KorenovaSlozka, "Zalohy", "Zvirata", "zvirata.json.bak");
        private string ZalohaSlozkaZvirat => Path.Combine(KorenovaSlozka, "Zalohy", "Zvirata");
        private string ZalohaCislaZvirete => Path.Combine(ZalohaSlozkaZvirat, "cislovani.json.bak");

        private string ZalohaZvirete(Zvire zvire) => Path.Combine(ZalohaSlozkaZvirat,
            $"{NazevSouboru.OcistiProNazevSouboru(zvire.Nazev)}_{zvire.Id}.json.bak");

        private static readonly JsonSerializerOptions ZvireJsonOptions = new()
        {
            Converters = { new DateOnlyConverter() }
        };

        private static readonly JsonSerializerOptions ZvireJsonOptionsIndented = new()
        {
            WriteIndented = true,
            Converters = { new DateOnlyConverter() }
        };

        public void NactiZvirata()
        {
            Zvirata = [];

            if (Directory.Exists(SlozkaZvirat))
            {
                foreach (string soubor in Directory.GetFiles(SlozkaZvirat, "*.json"))
                {
                    if (Path.GetFileName(soubor) == "cislovani.json")
                        continue;

                    try
                    {
                        string json = File.ReadAllText(soubor);
                        var zvire = JsonSerializer.Deserialize<Zvire>(json, ZvireJsonOptions);
                        if (zvire != null)
                            Zvirata.Add(zvire);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Chyba při načítání souboru zvířete '{Path.GetFileName(soubor)}': {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }

            NactiCisloZvireteKonfiguraci();
        }

        private void NactiCisloZvireteKonfiguraci()
        {
            if (!File.Exists(SouborCislaZvirete) && File.Exists(ZalohaCislaZvirete))
            {
                Directory.CreateDirectory(SlozkaZvirat);
                File.Copy(ZalohaCislaZvirete, SouborCislaZvirete);
            }

            if (!File.Exists(SouborCislaZvirete))
                return;

            try
            {
                string json = File.ReadAllText(SouborCislaZvirete);
                CisloZvireteKonfigurace = JsonSerializer.Deserialize<CisloZvireteKonfigurace>(json) ?? new();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání číslování zvířat: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Uloží jedno zvíře do jeho vlastního souboru. Pokud se změnil název
        /// zvířete (a tedy i název souboru), stará verze souboru se smaže.
        /// </summary>
        public void UlozZvire(Zvire zvire, string? puvodniNazevSouboru = null)
        {
            Directory.CreateDirectory(SlozkaZvirat);
            Directory.CreateDirectory(ZalohaSlozkaZvirat);

            if (puvodniNazevSouboru != null && puvodniNazevSouboru != SouborZvirete(zvire))
            {
                if (File.Exists(puvodniNazevSouboru))
                    File.Delete(puvodniNazevSouboru);
            }

            string json = JsonSerializer.Serialize(zvire, ZvireJsonOptionsIndented);
            ZapisSouborSeZalohou(SouborZvirete(zvire), ZalohaZvirete(zvire), json);
        }

        /// <summary>
        /// Smaže soubor konkrétního zvířete.
        /// </summary>
        public void SmazatZvireSoubor(Zvire zvire)
        {
            string soubor = SouborZvirete(zvire);
            if (File.Exists(soubor))
                File.Delete(soubor);
        }

        public void UlozCisloZvireteKonfiguraci()
        {
            try
            {
                string json = JsonSerializer.Serialize(CisloZvireteKonfigurace, CisloZamestnanceJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborCislaZvirete, ZalohaCislaZvirete, json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání číslování zvířat: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
