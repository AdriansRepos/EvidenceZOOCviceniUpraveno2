using System.Text.Json;
using EvidenceZOOCviceniUpraveno2.Data.PomocneTridy;
using EvidenceZOOCviceniUpraveno2.Entity;
using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Data.Repozitare
{
    /// <summary>
    /// Repozitář zajišťující perzistenci skladových zásob a kompletace historie skladových pohybů.
    /// </summary>
    class SkladRepository(string korenovaSlozka)
    {
        private readonly string korenovaSlozka = korenovaSlozka;

        /// <summary> Seznam aktuálních položek na skladě. </summary>
        public List<SkladovaPolozka> Sklad { get; internal set; } = [];

        /// <summary> Historie všech proběhlých skladových pohybů (příjmy, výdeje). </summary>
        public List<SkladovyPohyb> SkladovaHistorie { get; internal set; } = [];

        /// <summary> Cesta k souboru se stavem skladu. </summary>
        public string SouborSkladu => Path.Combine(korenovaSlozka, "Data", "Sklad", "sklad.json");

        /// <summary> Cesta k souboru s historií skladu. </summary>
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

        /// <summary>
        /// Načte položky skladu i jeho historii ze souborů.
        /// </summary>
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

        /// <summary> Uloží stav skladu na disk. </summary>
        public void UlozSklad()
        {
            SouborovyPomocnik.UlozDoSouboru(Sklad, SkladOptionsIndented,
                SouborSkladu, ZalohaSkladu, "skladu");
        }

        /// <summary> Uloží historii skladových pohybů na disk. </summary>
        public void UlozHistorii()
        {
            SouborovyPomocnik.UlozDoSouboru(SkladovaHistorie, HistorieOptionsIndented,
                SouborHistorie, ZalohaHistorie, "historie skladu");
        }

        /// <summary>
        /// Atomicky uloží sklad i jeho historii jako jednu logickou transakční jednotku. 
        /// Pokud uložení historie selže, vrátí stav skladu ze zálohy zpět pro zachování konzistence dat.
        /// </summary>
        public void UlozSkladSHistorii()
        {
            bool skladUlozen = false;

            try
            {
                // 1. Krok: Uložení stavu skladu
                string jsonSklad = JsonSerializer.Serialize(Sklad, SkladOptionsIndented);
                SouborovyPomocnik.ZapisSouborSeZalohou(SouborSkladu, ZalohaSkladu, jsonSklad);
                skladUlozen = true;

                // 2. Krok: Uložení historie skladu
                string jsonHistorie = JsonSerializer.Serialize(SkladovaHistorie, HistorieOptionsIndented);
                SouborovyPomocnik.ZapisSouborSeZalohou(SouborHistorie, ZalohaHistorie, jsonHistorie);
            }
            catch (Exception ex)
            {                
                VypisyDoKonzole.VypisVarovani($"Chyba při ukládání skladu/historie: {ex.Message}");                

                // Rollback skladu ze zálohy, pokud selhal zápis historie
                if (skladUlozen && File.Exists(ZalohaSkladu))
                    File.Copy(ZalohaSkladu, SouborSkladu, overwrite: true);

                throw;
            }
        }
    }
}