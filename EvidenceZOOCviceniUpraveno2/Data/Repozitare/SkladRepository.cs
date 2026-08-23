using System.Text.Json;
using EvidenceZOOCviceniUpraveno2.Data.PomocneTridy;
using EvidenceZOOCviceniUpraveno2.Entity;
using PohybHelper;


namespace EvidenceZOOCviceniUpraveno2.Data.Repozitare
{
    /// <summary>
    /// Zodpovídá výhradně za perzistenci skladu a historie skladových pohybů.
    /// </summary>
    class SkladRepository(string korenovaSlozka)
    {
        private readonly string korenovaSlozka = korenovaSlozka;

        public List<SkladovaPolozka> Sklad { get; internal set; } = [];
        public List<SkladovyPohyb> SkladovaHistorie { get; internal set; } = [];

        public string SouborSkladu => Path.Combine(korenovaSlozka, "Data", "Sklad", "sklad.json");
        public string SouborHistorie => Path.Combine(korenovaSlozka, "Data", "Sklad", "sklad_historie.json");

        private string ZalohaSkladu => Path.Combine(korenovaSlozka, "Zalohy", "Sklad", "sklad.json.bak");
        private string ZalohaHistorie => Path.Combine(korenovaSlozka, "Zalohy", "Sklad", "sklad_historie.json.bak");

        private static readonly JsonSerializerOptions SkladOptions = new();

        private static readonly JsonSerializerOptions SkladOptionsIndented = new()
        {
            WriteIndented = true
        };

        private static readonly JsonSerializerOptions HistorieOptions = new();

        private static readonly JsonSerializerOptions HistorieOptionsIndented = new()
        {
            WriteIndented = true
        };

        public void Nacti()
        {
            Sklad = SouborovyPomocnik.NactiZeSouboru(
                SouborSkladu, ZalohaSkladu,
                json => JsonSerializer.Deserialize<List<SkladovaPolozka>>(json, SkladOptions) ?? [],
                "skladu");

            SkladovaHistorie = SouborovyPomocnik.NactiZeSouboru(
                SouborHistorie, ZalohaHistorie,
                json => JsonSerializer.Deserialize<List<SkladovyPohyb>>(json, HistorieOptions) ?? [],
                "historie skladu");
        }

        public void UlozSklad()
        {
            SouborovyPomocnik.UlozDoSouboru(Sklad, SkladOptionsIndented,
                SouborSkladu, ZalohaSkladu, "skladu");
        }

        public void UlozHistorii()
        {
            SouborovyPomocnik.UlozDoSouboru(SkladovaHistorie, HistorieOptionsIndented,
                SouborHistorie, ZalohaHistorie, "historie skladu");
        }

        /// <summary>
        /// Uloží sklad i historii jako jednu logickou jednotku. Pokud se
        /// nepodaří uložit historii poté, co byl sklad úspěšně uložen,
        /// obnoví sklad zpět z jeho čerstvé zálohy.
        /// </summary>
        public void UlozSkladSHistorii()
        {
            bool skladUlozen = false;

            try
            {
                string jsonSklad = JsonSerializer.Serialize(Sklad, SkladOptionsIndented);
                SouborovyPomocnik.ZapisSouborSeZalohou(SouborSkladu, ZalohaSkladu, jsonSklad);
                skladUlozen = true;

                string jsonHistorie = JsonSerializer.Serialize(SkladovaHistorie, HistorieOptionsIndented);
                SouborovyPomocnik.ZapisSouborSeZalohou(SouborHistorie, ZalohaHistorie, jsonHistorie);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání skladu/historie: {ex.Message}");
                Console.ResetColor();

                if (skladUlozen && File.Exists(ZalohaSkladu))
                    File.Copy(ZalohaSkladu, SouborSkladu, overwrite: true);

                throw;
            }
        }
    }
}