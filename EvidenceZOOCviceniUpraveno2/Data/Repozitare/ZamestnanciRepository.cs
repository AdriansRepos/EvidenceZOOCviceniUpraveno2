using System.Text.Json;
using DateConverterForJson;
using EvidenceZOOCviceniUpraveno2.Data.PomocneTridy;
using EvidenceZOOCviceniUpraveno2.Entity;
using FileNameHelper;
using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Data.Repozitare
{
    /// <summary>
    /// Repozitář zodpovědný za správy zaměstnanců, číselné konfigurační řady a generování jejich dokumentových složek.
    /// </summary>
    class ZamestnanciRepository(string korenovaSlozka)
    {
        private readonly string korenovaSlozka = korenovaSlozka;

        /// <summary> Seznam zaměstnanců ZOO. </summary>
        public List<Zamestnanec> Zamestnanci { get; internal set; } = [];

        /// <summary> Konfigurace pro generování osobních čísel zaměstnanců. </summary>
        public CisloZamestnanceKonfigurace CisloKonfigurace { get; internal set; } = new();

        /// <summary> Cesta k hlavnímu souboru zaměstnanců. </summary>
        public string SouborZamestnanci => Path.Combine(korenovaSlozka, "Data", "Zamestnanci", "zamestnanci.json");

        /// <summary> Cesta k nastavení číselné řady zaměstnanců. </summary>
        public string SouborCisla => Path.Combine(korenovaSlozka, "Data", "Zamestnanci", "cislovani.json");

        /// <summary> Vrátí cestu ke složce určené pro doklady a dokumenty konkrétního zaměstnance. </summary>
        public string SlozkaZamestnance(Zamestnanec zam) => Path.Combine(korenovaSlozka, "Data", "Zamestnanci",
            $"{NazevSouboru.OcistiProNazevSouboru(zam.Prijmeni)}_{zam.OsobniCislo}");

        private string ZalohaZamestnanci => Path.Combine(korenovaSlozka, "Zalohy", "Zamestnanci", "zamestnanci.json.bak");
        private string ZalohaCisla => Path.Combine(korenovaSlozka, "Zalohy", "Zamestnanci", "cislovani.json.bak");

        private string ZalohaSlozkaZamestnance(Zamestnanec zam) => Path.Combine(korenovaSlozka, "Zalohy", "Zamestnanci",
            $"{NazevSouboru.OcistiProNazevSouboru(zam.Prijmeni)}_{zam.OsobniCislo}");

        // Konfigurace serializace s podporou typu DateOnly
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

        /// <summary> Načte seznam zaměstnanců a číselnou řadu ze souborů. </summary>
        public void Nacti()
        {
            Zamestnanci = SouborovyPomocnik.NactiZeSouboru(
                SouborZamestnanci, ZalohaZamestnanci,
                json => JsonSerializer.Deserialize<List<Zamestnanec>>(json, Options) ?? [],
                "zaměstnanců");

            NactiCisloKonfiguraci();
        }

        /// <summary> Načte konfiguraci číslování zaměstnanců. </summary>
        private void NactiCisloKonfiguraci()
        {
            if (!File.Exists(SouborCisla) && File.Exists(ZalohaCisla))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SouborCisla)!);
                File.Copy(ZalohaCisla, SouborCisla);
            }

            if (!File.Exists(SouborCisla))
                return;

            try
            {
                string json = File.ReadAllText(SouborCisla);
                CisloKonfigurace = JsonSerializer.Deserialize<CisloZamestnanceKonfigurace>(json) ?? new();
            }
            catch (Exception ex)
            {                
                VypisyDoKonzole.VypisVarovani($"Chyba při načítání číslování zaměstnanců: {ex.Message}");                
            }
        }

        /// <summary> Uloží aktuální seznam zaměstnanců na disk. </summary>
        public void Uloz()
        {
            SouborovyPomocnik.UlozDoSouboru(Zamestnanci, OptionsIndented,
                SouborZamestnanci, ZalohaZamestnanci, "zaměstnanců");
        }

        /// <summary> Uloží stav číselné řady zaměstnanců. </summary>
        public void UlozCisloKonfiguraci()
        {
            try
            {
                string json = JsonSerializer.Serialize(CisloKonfigurace, CisloOptionsIndented);
                SouborovyPomocnik.ZapisSouborSeZalohou(SouborCisla, ZalohaCisla, json);
            }
            catch (Exception ex)
            {                
                VypisyDoKonzole.VypisVarovani($"Chyba při ukládání číslování zaměstnanců: {ex.Message}");                
            }
        }

        /// <summary> Vytvoří osobní složku zaměstnance i její odpovídající záložní strukturu. </summary>
        public void VytvorSlozkuZamestnance(Zamestnanec zam)
        {
            Directory.CreateDirectory(SlozkaZamestnance(zam));
            Directory.CreateDirectory(ZalohaSlozkaZamestnance(zam));
        }

        /// <summary> Přejmenuje složku zaměstnance na základě změny jeho příjmení. </summary>
        public void PrejmenovatSlozkuZamestnance(Zamestnanec zam, string puvodniPrijmeni)
        {
            string stareJmeno = $"{NazevSouboru.OcistiProNazevSouboru(puvodniPrijmeni)}_{zam.OsobniCislo}";
            string staraCestaData = Path.Combine(korenovaSlozka, "Data", "Zamestnanci", stareJmeno);
            string staraCestaZaloha = Path.Combine(korenovaSlozka, "Zalohy", "Zamestnanci", stareJmeno);

            // Přejmenování složky v Data
            if (Directory.Exists(staraCestaData) && staraCestaData != SlozkaZamestnance(zam))
                Directory.Move(staraCestaData, SlozkaZamestnance(zam));

            // Přejmenování složky v Zalohy
            if (Directory.Exists(staraCestaZaloha) && staraCestaZaloha != ZalohaSlozkaZamestnance(zam))
                Directory.Move(staraCestaZaloha, ZalohaSlozkaZamestnance(zam));
        }

        /// <summary> Uloží specifický JSON dokument do osobní složky zaměstnance. </summary>
        public void UlozDokument<T>(Zamestnanec zam, string nazevDokumentu, T data)
        {
            VytvorSlozkuZamestnance(zam);

            string soubor = Path.Combine(SlozkaZamestnance(zam), $"{nazevDokumentu}.json");
            string zaloha = Path.Combine(ZalohaSlozkaZamestnance(zam), $"{nazevDokumentu}.json.bak");

            string json = JsonSerializer.Serialize(data, CisloOptionsIndented);
            SouborovyPomocnik.ZapisSouborSeZalohou(soubor, zaloha, json);
        }

        /// <summary> Vrátí seznam názvů všech dokumentů uložených ve složce daného zaměstnance. </summary>
        public List<string> VypisDokumenty(Zamestnanec zam)
        {
            string slozka = SlozkaZamestnance(zam);
            if (!Directory.Exists(slozka))
                return [];

            return [.. Directory.GetFiles(slozka, "*.json")
                .Select(Path.GetFileNameWithoutExtension)
                .Where(nazev => nazev != null)
                .Select(nazev => nazev!)];
        }
    }
}