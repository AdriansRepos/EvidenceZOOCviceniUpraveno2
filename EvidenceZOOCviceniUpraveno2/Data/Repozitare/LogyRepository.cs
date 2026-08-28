using EvidenceZOOCviceniUpraveno2.Data.PomocneTridy;
using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Enumy;
using System.Text.Json;

namespace EvidenceZOOCviceniUpraveno2.Data.Repozitare
{
    /// <summary>
    /// Repozitář zodpovědný výhradně za načítání, uchovávání a zápis auditních a technických logů do JSON souborů.
    /// </summary>
    class LogyRepository(string korenovaSlozka)
    {
        private readonly string korenovaSlozka = korenovaSlozka;

        /// <summary>
        /// Seznam zaznamenaných auditních událostí.
        /// </summary>
        public List<AuditZaznam> AuditLog { get; internal set; } = [];

        /// <summary>
        /// Seznam zaznamenaných technických systémových logů.
        /// </summary>
        public List<TechnickyZaznam> TechnickyLog { get; internal set; } = [];

        /// <summary> Cesta k souboru auditního logu. </summary>
        public string SouborAuditLogu => Path.Combine(korenovaSlozka, "Data", "Log", "audit.json");

        /// <summary> Cesta k souboru technického logu. </summary>
        public string SouborTechnickehoLogu => Path.Combine(korenovaSlozka, "Data", "Log", "technicky.json");

        /// <summary> Cesta k záložnímu souboru auditního logu. </summary>
        private string ZalohaAuditLogu => Path.Combine(korenovaSlozka, "Zalohy", "Log", "audit.json.bak");

        /// <summary> Cesta k záložnímu souboru technického logu. </summary>
        private string ZalohaTechnickehoLogu => Path.Combine(korenovaSlozka, "Zalohy", "Log", "technicky.json.bak");

        // Konfigurace JSON serializace
        private static readonly JsonSerializerOptions Options = new();

        private static readonly JsonSerializerOptions OptionsIndented = new()
        {
            WriteIndented = true // Formátování s odsazením pro lepší čitelnost
        };

        /// <summary>
        /// Načte auditní i technické logy ze souborů na disku. Při poškození automaticky použije zálohu.
        /// </summary>
        public void Nacti()
        {
            // Načtení auditního logu pomocí obecného pomocníka
            AuditLog = SouborovyPomocnik.NactiZeSouboru(
                SouborAuditLogu, ZalohaAuditLogu,
                json => JsonSerializer.Deserialize<List<AuditZaznam>>(json, Options) ?? [],
                "auditního logu");

            // Načtení technického logu
            TechnickyLog = SouborovyPomocnik.NactiZeSouboru(
                SouborTechnickehoLogu, ZalohaTechnickehoLogu,
                json => JsonSerializer.Deserialize<List<TechnickyZaznam>>(json, Options) ?? [],
                "technického logu");
        }

        /// <summary>
        /// Přidá nový záznam do auditního logu a ihned uplatní persistentní zápis na disk.
        /// </summary>
        public void ZapisAudit(AuditZaznam zaznam)
        {
            AuditLog.Add(zaznam);
            SouborovyPomocnik.UlozDoSouboru(AuditLog, OptionsIndented, SouborAuditLogu, ZalohaAuditLogu, "auditního logu");
        }

        /// <summary>
        /// Vytvoří a uloží nový technický záznam s danou úrovní a zprávou.
        /// </summary>
        public void ZapisTechnickyLog(UrovenLogu uroven, string zprava)
        {
            TechnickyLog.Add(new TechnickyZaznam(uroven, zprava, DateTime.Now));
            SouborovyPomocnik.UlozDoSouboru(TechnickyLog, OptionsIndented, SouborTechnickehoLogu, ZalohaTechnickehoLogu, "technického logu");
        }
    }
}