using System.Text.Json;


namespace EvidenceZOOCviceniUpraveno2
{
    partial class ZOO
    {
        // -----------------------------
        // CESTY K DATOVÝM SOUBORŮM (Data/...)
        // -----------------------------

        public string SouborSkladu => Path.Combine(KorenovaSlozka, "Data", "Sklad", "sklad.json");
        public string SouborSkladoveHistorie => Path.Combine(KorenovaSlozka, "Data", "Sklad", "sklad_historie.json");

        // -----------------------------
        // CESTY K ZÁLOHÁM (Zalohy/...)
        // -----------------------------

         private string ZalohaSkladu => Path.Combine(KorenovaSlozka, "Zalohy", "Sklad", "sklad.json.bak");
        private string ZalohaSkladoveHistorie => Path.Combine(KorenovaSlozka, "Zalohy", "Sklad", "sklad_historie.json.bak");

        private static readonly JsonSerializerOptions SkladJsonOptions = new();

        private static readonly JsonSerializerOptions SkladJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

        private static readonly JsonSerializerOptions HistorieJsonOptions = new();

        private static readonly JsonSerializerOptions HistorieJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

        public void NactiSklad()
        {
            Sklad = NactiZeSouboru(
                SouborSkladu, ZalohaSkladu,
                json => JsonSerializer.Deserialize<List<SkladovaPolozka>>(json, SkladJsonOptions) ?? [],
                "skladu");

            SkladovaHistorie = NactiZeSouboru(
                SouborSkladoveHistorie, ZalohaSkladoveHistorie,
                json => JsonSerializer.Deserialize<List<SkladovyPohyb>>(json, HistorieJsonOptions) ?? [],
                "historie skladu");
        }

        public void UlozSklad()
        {
            UlozDoSouboru(Sklad, SkladJsonOptionsIndented,
                SouborSkladu, ZalohaSkladu, "skladu");
        }

        public void UlozSkladovouHistorii()
        {
            UlozDoSouboru(SkladovaHistorie, HistorieJsonOptionsIndented,
                SouborSkladoveHistorie, ZalohaSkladoveHistorie, "historie skladu");
        }

        /// <summary>
        /// Uloží sklad i historii skladových pohybů jako jednu logickou jednotku.
        /// Pokud se nepodaří uložit historii poté, co už byl úspěšně uložen sklad,
        /// obnoví sklad zpět z jeho čerstvě vytvořené zálohy, aby oba soubory
        /// zůstaly navzájem konzistentní.
        /// </summary>
        public void UlozSkladSHistorii()
        {
            bool skladUlozen = false;

            try
            {
                string jsonSklad = JsonSerializer.Serialize(Sklad, SkladJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborSkladu, ZalohaSkladu, jsonSklad);
                skladUlozen = true;

                string jsonHistorie = JsonSerializer.Serialize(SkladovaHistorie, HistorieJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborSkladoveHistorie, ZalohaSkladoveHistorie, jsonHistorie);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání skladu/historie: {ex.Message}");
                Console.ResetColor();

                if (skladUlozen && File.Exists(ZalohaSkladu))
                {
                    File.Copy(ZalohaSkladu, SouborSkladu, overwrite: true);
                }

                throw;
            }
        }
    }
}
