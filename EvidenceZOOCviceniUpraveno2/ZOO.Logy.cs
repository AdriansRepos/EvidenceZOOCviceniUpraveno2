using System.Text.Json;

namespace EvidenceZOOCviceniUpraveno2
{
    partial class ZOO
    {
        // -----------------------------
        // CESTY K DATOVÝM SOUBORŮM (Data/...)
        // -----------------------------

        public string SouborAuditLogu => Path.Combine(KorenovaSlozka, "Data", "Log", "audit.json");
        public string SouborTechnickehoLogu => Path.Combine(KorenovaSlozka, "Data", "Log", "technicky.json");

        // -----------------------------
        // CESTY K ZÁLOHÁM (Zalohy/...)
        // -----------------------------

        private string ZalohaAuditLogu => Path.Combine(KorenovaSlozka, "Zalohy", "Log", "audit.json.bak");
        private string ZalohaTechnickehoLogu => Path.Combine(KorenovaSlozka, "Zalohy", "Log", "technicky.json.bak");

        private static readonly JsonSerializerOptions LogJsonOptions = new();

        private static readonly JsonSerializerOptions LogJsonOptionsIndented = new()
        {
            WriteIndented = true
        };

         public void NactiLogy()
        {
            AuditLog = NactiZeSouboru(
                SouborAuditLogu, ZalohaAuditLogu,
                json => JsonSerializer.Deserialize<List<AuditZaznam>>(json, LogJsonOptions) ?? [],
                "auditního logu");

            TechnickyLog = NactiZeSouboru(
                SouborTechnickehoLogu, ZalohaTechnickehoLogu,
                json => JsonSerializer.Deserialize<List<TechnickyZaznam>>(json, LogJsonOptions) ?? [],
                "technického logu");
        }

         /// <summary>
        /// Přidá nový záznam do auditního logu a okamžitě jej uloží.
        /// Log je append-only – existující záznamy se nikdy neupravují ani nemažou.
        /// </summary>
        public void ZapisAudit(AuditZaznam zaznam)
        {
            AuditLog.Add(zaznam);
            UlozDoSouboru(AuditLog, LogJsonOptionsIndented, SouborAuditLogu, ZalohaAuditLogu, "auditního logu");
        }

        /// <summary>
        /// Přidá nový záznam do technického logu a okamžitě jej uloží.
        /// </summary>
        public void ZapisTechnickyLog(UrovenLogu uroven, string zprava)
        {
            TechnickyLog.Add(new TechnickyZaznam(uroven, zprava, DateTime.Now));
            UlozDoSouboru(TechnickyLog, LogJsonOptionsIndented, SouborTechnickehoLogu, ZalohaTechnickehoLogu, "technického logu");
        }
    }
}
