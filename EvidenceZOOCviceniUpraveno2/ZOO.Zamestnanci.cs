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

        public string SouborZamestnanci => Path.Combine(KorenovaSlozka, "Data", "Zamestnanci", "zamestnanci.json");
        public string SouborCislaZamestnance => Path.Combine(KorenovaSlozka, "Data", "Zamestnanci", "cislovani.json");

        private string SlozkaZamestnance(Zamestnanec zam) => Path.Combine(KorenovaSlozka, "Data", "Zamestnanci",
            $"{NazevSouboru.OcistiProNazevSouboru(zam.Prijmeni)}_{zam.OsobniCislo}");

        // -----------------------------
        // CESTY K ZÁLOHÁM (Zalohy/...)
        // -----------------------------

        private string ZalohaZamestnanci => Path.Combine(KorenovaSlozka, "Zalohy", "Zamestnanci", "zamestnanci.json.bak");
        private string ZalohaCislaZamestnance => Path.Combine(KorenovaSlozka, "Zalohy", "Zamestnanci", "cislovani.json.bak");

        private string ZalohaSlozkaZamestnance(Zamestnanec zam) => Path.Combine(KorenovaSlozka, "Zalohy", "Zamestnanci",
            $"{NazevSouboru.OcistiProNazevSouboru(zam.Prijmeni)}_{zam.OsobniCislo}");

        private static readonly JsonSerializerOptions ZamestnanecJsonOptions = new()
        {
            Converters = { new DateOnlyConverter() }
        };

        private static readonly JsonSerializerOptions ZamestnanecJsonOptionsIndented = new()
        {
            WriteIndented = true,
            Converters = { new DateOnlyConverter() }
        };

        private static readonly JsonSerializerOptions CisloZamestnanceJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

        public void NactiZamestnance()
        {
            Zamestnanci = NactiZeSouboru(
                SouborZamestnanci, ZalohaZamestnanci,
                json => JsonSerializer.Deserialize<List<Zamestnanec>>(json, ZamestnanecJsonOptions) ?? [],
                "zaměstnanců");

            NactiCisloZamestnanceKonfiguraci();
        }

        private void NactiCisloZamestnanceKonfiguraci()
        {
            if (!File.Exists(SouborCislaZamestnance) && File.Exists(ZalohaCislaZamestnance))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SouborCislaZamestnance)!);
                File.Copy(ZalohaCislaZamestnance, SouborCislaZamestnance);
            }

            if (!File.Exists(SouborCislaZamestnance))
                return;

            try
            {
                string json = File.ReadAllText(SouborCislaZamestnance);
                CisloZamestnanceKonfigurace = JsonSerializer.Deserialize<CisloZamestnanceKonfigurace>(json) ?? new();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání číslování zaměstnanců: {ex.Message}");
                Console.ResetColor();
            }
        }

        public void UlozZamestnance()
        {
            UlozDoSouboru(Zamestnanci, ZamestnanecJsonOptionsIndented,
                SouborZamestnanci, ZalohaZamestnanci, "zaměstnanců");
        }

        public void UlozCisloZamestnanceKonfiguraci()
        {
            try
            {
                string json = JsonSerializer.Serialize(CisloZamestnanceKonfigurace, CisloZamestnanceJsonOptionsIndented);
                ZapisSouborSeZalohou(SouborCislaZamestnance, ZalohaCislaZamestnance, json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při ukládání číslování zaměstnanců: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Vytvoří dokumentovou složku zaměstnance (Data i Zálohy), pokud
        /// ještě neexistuje.
        /// </summary>
        public void VytvorSlozkuZamestnance(Zamestnanec zam)
        {
            Directory.CreateDirectory(SlozkaZamestnance(zam));
            Directory.CreateDirectory(ZalohaSlozkaZamestnance(zam));
        }

        /// <summary>
        /// Přejmenuje dokumentovou složku zaměstnance (Data i Zálohy) podle
        /// nového příjmení. Volej PŘED tím, než se Prijmeni na objektu skutečně změní.
        /// </summary>
        public void PrejmenovatSlozkuZamestnance(Zamestnanec zam, string puvodniPrijmeni)
        {
            string stareJmeno = $"{NazevSouboru.OcistiProNazevSouboru(puvodniPrijmeni)}_{zam.OsobniCislo}";
            string staraCestaData = Path.Combine(KorenovaSlozka, "Data", "Zamestnanci", stareJmeno);
            string staraCestaZaloha = Path.Combine(KorenovaSlozka, "Zalohy", "Zamestnanci", stareJmeno);

            if (Directory.Exists(staraCestaData) && staraCestaData != SlozkaZamestnance(zam))
                Directory.Move(staraCestaData, SlozkaZamestnance(zam));

            if (Directory.Exists(staraCestaZaloha) && staraCestaZaloha != ZalohaSlozkaZamestnance(zam))
                Directory.Move(staraCestaZaloha, ZalohaSlozkaZamestnance(zam));
        }

        /// <summary>
        /// Uloží libovolný dokument zaměstnance (výplatní páska, roční
        /// zúčtování, srážka...) jako samostatný JSON soubor v jeho
        /// dokumentové složce, včetně crash-safe zápisu a zálohy.
        /// </summary>
        public void UlozDokumentZamestnance<T>(Zamestnanec zam, string nazevDokumentu, T data)
        {
            VytvorSlozkuZamestnance(zam);

            string soubor = Path.Combine(SlozkaZamestnance(zam), $"{nazevDokumentu}.json");
            string zaloha = Path.Combine(ZalohaSlozkaZamestnance(zam), $"{nazevDokumentu}.json.bak");

            string json = JsonSerializer.Serialize(data, CisloZamestnanceJsonOptionsIndented);
            ZapisSouborSeZalohou(soubor, zaloha, json);
        }

        /// <summary>
        /// Vrátí seznam názvů všech dokumentů uložených ve složce zaměstnance.
        /// </summary>
        public List<string> VypisDokumentyZamestnance(Zamestnanec zam)
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
