using System.Text.Json;
using DateConverterForJson;
using EvidenceZOOCviceniUpraveno2.Data.PomocneTridy;
using EvidenceZOOCviceniUpraveno2.Entity;
using FileNameHelper;
using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Data.Repozitare
{
    /// <summary>
    /// Zodpovídá výhradně za perzistenci zaměstnanců – čtení, zápis,
    /// dokumentové složky, číslování.
    /// </summary>
    class ZamestnanciRepository(string korenovaSlozka)
    {
        private readonly string korenovaSlozka = korenovaSlozka;

        public List<Zamestnanec> Zamestnanci { get; internal set; } = [];
        public CisloZamestnanceKonfigurace CisloKonfigurace { get; internal set; } = new();

        public string SouborZamestnanci => Path.Combine(korenovaSlozka, "Data", "Zamestnanci", "zamestnanci.json");
        public string SouborCisla => Path.Combine(korenovaSlozka, "Data", "Zamestnanci", "cislovani.json");

        public string SlozkaZamestnance(Zamestnanec zam) => Path.Combine(korenovaSlozka, "Data", "Zamestnanci",
            $"{NazevSouboru.OcistiProNazevSouboru(zam.Prijmeni)}_{zam.OsobniCislo}");

        private string ZalohaZamestnanci => Path.Combine(korenovaSlozka, "Zalohy", "Zamestnanci", "zamestnanci.json.bak");
        private string ZalohaCisla => Path.Combine(korenovaSlozka, "Zalohy", "Zamestnanci", "cislovani.json.bak");

        private string ZalohaSlozkaZamestnance(Zamestnanec zam) => Path.Combine(korenovaSlozka, "Zalohy", "Zamestnanci",
            $"{NazevSouboru.OcistiProNazevSouboru(zam.Prijmeni)}_{zam.OsobniCislo}");

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

        public void Nacti()
        {
            Zamestnanci = SouborovyPomocnik.NactiZeSouboru(
                SouborZamestnanci, ZalohaZamestnanci,
                json => JsonSerializer.Deserialize<List<Zamestnanec>>(json, Options) ?? [],
                "zaměstnanců");

            NactiCisloKonfiguraci();
        }

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

        public void Uloz()
        {
            SouborovyPomocnik.UlozDoSouboru(Zamestnanci, OptionsIndented,
                SouborZamestnanci, ZalohaZamestnanci, "zaměstnanců");
        }

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

        public void VytvorSlozkuZamestnance(Zamestnanec zam)
        {
            Directory.CreateDirectory(SlozkaZamestnance(zam));
            Directory.CreateDirectory(ZalohaSlozkaZamestnance(zam));
        }

        public void PrejmenovatSlozkuZamestnance(Zamestnanec zam, string puvodniPrijmeni)
        {
            string stareJmeno = $"{NazevSouboru.OcistiProNazevSouboru(puvodniPrijmeni)}_{zam.OsobniCislo}";
            string staraCestaData = Path.Combine(korenovaSlozka, "Data", "Zamestnanci", stareJmeno);
            string staraCestaZaloha = Path.Combine(korenovaSlozka, "Zalohy", "Zamestnanci", stareJmeno);

            if (Directory.Exists(staraCestaData) && staraCestaData != SlozkaZamestnance(zam))
                Directory.Move(staraCestaData, SlozkaZamestnance(zam));

            if (Directory.Exists(staraCestaZaloha) && staraCestaZaloha != ZalohaSlozkaZamestnance(zam))
                Directory.Move(staraCestaZaloha, ZalohaSlozkaZamestnance(zam));
        }

        public void UlozDokument<T>(Zamestnanec zam, string nazevDokumentu, T data)
        {
            VytvorSlozkuZamestnance(zam);

            string soubor = Path.Combine(SlozkaZamestnance(zam), $"{nazevDokumentu}.json");
            string zaloha = Path.Combine(ZalohaSlozkaZamestnance(zam), $"{nazevDokumentu}.json.bak");

            string json = JsonSerializer.Serialize(data, CisloOptionsIndented);
            SouborovyPomocnik.ZapisSouborSeZalohou(soubor, zaloha, json);
        }

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