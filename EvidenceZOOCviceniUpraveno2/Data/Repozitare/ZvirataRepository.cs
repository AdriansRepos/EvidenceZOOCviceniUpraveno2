using System.Text.Json;
using DateConverterForJson;
using EvidenceZOOCviceniUpraveno2.Data.PomocneTridy;
using EvidenceZOOCviceniUpraveno2.Entity;
using FileNameHelper;
using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Data.Repozitare
{
    /// <summary>
    /// Repozitář určený pro správu zvířat. Každé zvíře ukládá do samostatného JSON souboru.
    /// </summary>
    class ZvirataRepository(string korenovaSlozka)
    {
        private readonly string korenovaSlozka = korenovaSlozka;

        /// <summary> Seznam evidovaných zvířat. </summary>
        public List<Zvire> Zvirata { get; internal set; } = [];

        /// <summary> Konfigurace generování unikátních ID pro zvířata. </summary>
        public CisloZvireteKonfigurace CisloKonfigurace { get; internal set; } = new();

        internal string SlozkaZvirat => Path.Combine(korenovaSlozka, "Data", "Zvirata");
        private string SouborCisla => Path.Combine(SlozkaZvirat, "cislovani.json");

        /// <summary> Sestaví cestu k JSON souboru konkrétního zvířete. </summary>
        public string SouborZvirete(Zvire zvire) =>
            Path.Combine(SlozkaZvirat, $"{NazevSouboru.OcistiProNazevSouboru(zvire.Nazev)}_{zvire.Id}.json");

        private string ZalohaSlozkaZvirat => Path.Combine(korenovaSlozka, "Zalohy", "Zvirata");
        private string ZalohaCisla => Path.Combine(ZalohaSlozkaZvirat, "cislovani.json.bak");

        private string ZalohaZvirete(Zvire zvire) => Path.Combine(ZalohaSlozkaZvirat,
            $"{NazevSouboru.OcistiProNazevSouboru(zvire.Nazev)}_{zvire.Id}.json.bak");

        private static readonly JsonSerializerOptions Options = new()
        {
            Converters = { new DateOnlyConverter() }
        };

        private static readonly JsonSerializerOptions OptionsIndented = new()
        {
            WriteIndented = true,
            Converters = { new DateOnlyConverter() }
        };

        private static readonly JsonSerializerOptions CisloOptionsIndented = new()
        {
            WriteIndented = true
        };

        /// <summary>
        /// Prohledá adresář zvířat a načte všechna jednotlivá zvířata z jejich samostatných JSON souborů.
        /// </summary>
        public void Nacti()
        {
            Zvirata = [];

            if (Directory.Exists(SlozkaZvirat))
            {
                // Načtení každého souboru .json kromě konfiguračního souboru číslování
                foreach (string soubor in Directory.GetFiles(SlozkaZvirat, "*.json"))
                {
                    if (Path.GetFileName(soubor) == "cislovani.json")
                        continue;

                    try
                    {
                        string json = File.ReadAllText(soubor);
                        var zvire = JsonSerializer.Deserialize<Zvire>(json, Options);
                        if (zvire != null)
                            Zvirata.Add(zvire);
                    }
                    catch (Exception ex)
                    {                        
                        VypisyDoKonzole.VypisVarovani($"Chyba při načítání souboru zvířete '{Path.GetFileName(soubor)}': {ex.Message}");                        
                    }
                }
            }

            NactiCisloKonfiguraci();
        }

        /// <summary> Načte číselnou řadu pro generování nových ID zvířat. </summary>
        private void NactiCisloKonfiguraci()
        {
            if (!File.Exists(SouborCisla) && File.Exists(ZalohaCisla))
            {
                Directory.CreateDirectory(SlozkaZvirat);
                File.Copy(ZalohaCisla, SouborCisla);
            }

            if (!File.Exists(SouborCisla))
                return;

            try
            {
                string json = File.ReadAllText(SouborCisla);
                CisloKonfigurace = JsonSerializer.Deserialize<CisloZvireteKonfigurace>(json) ?? new();
            }
            catch (Exception ex)
            {                
                VypisyDoKonzole.VypisVarovani($"Chyba při načítání číslování zvířat: {ex.Message}");                
            }
        }

        /// <summary> Uloží nebo aktualizuje soubor konkrétního zvířete a smaže případný starý soubor při přejmenování. </summary>
        public void UlozZvire(Zvire zvire, string? puvodniNazevSouboru = null)
        {
            Directory.CreateDirectory(SlozkaZvirat);
            Directory.CreateDirectory(ZalohaSlozkaZvirat);

            // Odstranění původního souboru, pokud došlo ke změně názvu zvířete
            if (puvodniNazevSouboru != null && puvodniNazevSouboru != SouborZvirete(zvire))
            {
                if (File.Exists(puvodniNazevSouboru))
                    File.Delete(puvodniNazevSouboru);
            }

            string json = JsonSerializer.Serialize(zvire, OptionsIndented);
            SouborovyPomocnik.ZapisSouborSeZalohou(SouborZvirete(zvire), ZalohaZvirete(zvire), json);
        }

        /// <summary> Smaže soubor daného zvířete z disku. </summary>
        public void SmazatZvireSoubor(Zvire zvire)
        {
            string soubor = SouborZvirete(zvire);
            if (File.Exists(soubor))
                File.Delete(soubor);
        }

        /// <summary> Uloží aktuální stav konfigurace číslování zvířat. </summary>
        public void UlozCisloKonfiguraci()
        {
            try
            {
                string json = JsonSerializer.Serialize(CisloKonfigurace, CisloOptionsIndented);
                SouborovyPomocnik.ZapisSouborSeZalohou(SouborCisla, ZalohaCisla, json);
            }
            catch (Exception ex)
            {                
                VypisyDoKonzole.VypisVarovani($"Chyba při ukládání číslování zvířat: {ex.Message}");                
            }
        }
    }
}