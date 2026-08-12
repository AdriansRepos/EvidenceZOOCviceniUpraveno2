using EvidenceZOOCviceniUpraveno2.Enumy;
using System.Text.Json;

namespace EvidenceZOOCviceniUpraveno2.Data
{
    /// <summary>
    /// Zodpovídá výhradně za perzistenci auditního a technického logu.
    /// Audit log je append-only.
    /// </summary>
    class LogyRepository(string korenovaSlozka)
    {
        private readonly string korenovaSlozka = korenovaSlozka;

        public List<AuditZaznam> AuditLog { get; internal set; } = [];
        public List<TechnickyZaznam> TechnickyLog { get; internal set; } = [];

        public string SouborAuditLogu => Path.Combine(korenovaSlozka, "Data", "Log", "audit.json");
        public string SouborTechnickehoLogu => Path.Combine(korenovaSlozka, "Data", "Log", "technicky.json");

        private string ZalohaAuditLogu => Path.Combine(korenovaSlozka, "Zalohy", "Log", "audit.json.bak");
        private string ZalohaTechnickehoLogu => Path.Combine(korenovaSlozka, "Zalohy", "Log", "technicky.json.bak");

        private static readonly JsonSerializerOptions Options = new();

        private static readonly JsonSerializerOptions OptionsIndented = new()
        {
            WriteIndented = true
        };

        public void Nacti()
        {
            AuditLog = SouborovyPomocnik.NactiZeSouboru(
                SouborAuditLogu, ZalohaAuditLogu,
                json => JsonSerializer.Deserialize<List<AuditZaznam>>(json, Options) ?? [],
                "auditního logu");

            TechnickyLog = SouborovyPomocnik.NactiZeSouboru(
                SouborTechnickehoLogu, ZalohaTechnickehoLogu,
                json => JsonSerializer.Deserialize<List<TechnickyZaznam>>(json, Options) ?? [],
                "technického logu");
        }

        public void ZapisAudit(AuditZaznam zaznam)
        {
            AuditLog.Add(zaznam);
            SouborovyPomocnik.UlozDoSouboru(AuditLog, OptionsIndented, SouborAuditLogu, ZalohaAuditLogu, "auditního logu");
        }

        public void ZapisTechnickyLog(UrovenLogu uroven, string zprava)
        {
            TechnickyLog.Add(new TechnickyZaznam(uroven, zprava, DateTime.Now));
            SouborovyPomocnik.UlozDoSouboru(TechnickyLog, OptionsIndented, SouborTechnickehoLogu, ZalohaTechnickehoLogu, "technického logu");
        }
    }
}