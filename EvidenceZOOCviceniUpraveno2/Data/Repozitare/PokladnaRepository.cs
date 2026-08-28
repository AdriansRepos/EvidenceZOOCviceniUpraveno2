using EvidenceZOOCviceniUpraveno2.Data.PomocneTridy;
using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.Entity;
using System.Text.Json;

namespace EvidenceZOOCviceniUpraveno2.Data.Repozitare
{
    /// <summary>
    /// Zodpovídá za perzistenci a správu pokladních pohybů a ceníku služeb/vstupného v ZOO.
    /// </summary>
    class PokladnaRepository(string korenovaSlozka)
    {
        private readonly string korenovaSlozka = korenovaSlozka;

        /// <summary> Seznam všech evidovaných pokladních transakcí. </summary>
        public List<PokladniPohyb> PokladniPohyby { get; internal set; } = [];

        /// <summary> Aktuální ceník ZOO. </summary>
        public Cenik Cenik { get; internal set; } = new();

        /// <summary> Cesta k JSON souboru pokladních pohybů. </summary>
        public string SouborPokladny => Path.Combine(korenovaSlozka, "Data", "Ucetnictvi", "Pokladna", "pokladna.json");

        /// <summary> Cesta k JSON souboru ceníku. </summary>
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

        /// <summary>
        /// Načte data pokladny i ceníku ze souborů na disku.
        /// </summary>
        public void Nacti()
        {
            PokladniPohyby = SouborovyPomocnik.NactiZeSouboru(
                SouborPokladny, ZalohaPokladny,
                json => JsonSerializer.Deserialize<List<PokladniPohyb>>(json, Options) ?? [],
                "pokladny");

            NactiCenik();
        }

        /// <summary>
        /// Načte ceník ze souboru. Pokud primární soubor chybí nebo je poškozený, pokusí se obnovit data ze zálohy.
        /// </summary>
        private void NactiCenik()
        {
            // Obnova ze zálohy, pokud primární soubor neexistuje
            if (!File.Exists(SouborCeniku) && File.Exists(ZalohaCeniku))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SouborCeniku)!);
                File.Copy(ZalohaCeniku, SouborCeniku);                
                VypisyDoKonzole.VypisUspech("Ceník obnoven ze zálohy.");                
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
                VypisyDoKonzole.VypisVarovani($"Chyba při načítání ceníku: {ex.Message}");                

                // Pokus o záchranu načtením ze záložního souboru při selhání deserializace
                if (File.Exists(ZalohaCeniku))
                {
                    try
                    {
                        string json = File.ReadAllText(ZalohaCeniku);
                        Cenik = JsonSerializer.Deserialize<Cenik>(json) ?? new Cenik();                        
                        VypisyDoKonzole.VypisUspech("Ceník úspěšně obnoven ze zálohy.");                       
                    }
                    catch
                    {                        
                        VypisyDoKonzole.VypisVarovani("Záloha ceníku je také poškozená, používám výchozí hodnoty.");                        
                    }
                }
            }
        }

        /// <summary> Uloží seznam pokladních pohybů do souboru se současným zálohováním. </summary>
        public void UlozPokladnu()
        {
            SouborovyPomocnik.UlozDoSouboru(PokladniPohyby, OptionsIndented,
                SouborPokladny, ZalohaPokladny, "pokladny");
        }

        /// <summary> Uloží aktuální ceník do souboru a vytvoří jeho záložní kopii. </summary>
        public void UlozCenik()
        {
            try
            {
                string json = JsonSerializer.Serialize(Cenik, CenikOptionsIndented);
                SouborovyPomocnik.ZapisSouborSeZalohou(SouborCeniku, ZalohaCeniku, json);
            }
            catch (Exception ex)
            {                
                VypisyDoKonzole.VypisVarovani($"Chyba při ukládání ceníku: {ex.Message}");                
            }
        }
    }
}