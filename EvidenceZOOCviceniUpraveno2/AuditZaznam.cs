using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Jeden záznam auditního logu – zaznamenává důležitou změnu
    /// (mzdy, ceník, pokladní a skladové pohyby, smazání záznamů)
    /// včetně původní a nové hodnoty, kdy a kým byla provedena.
    /// Log je určen jako append-only – existující záznamy se v aplikaci
    /// nikde neupravují ani nemažou.
    /// </summary>
    [method: JsonConstructor]
    class AuditZaznam(string modul, TypAkce akce, string popis,
        string? puvodniHodnota, string? novaHodnota, DateTime cas, string uzivatel)
    {
        /// <summary>
        /// Modul, ve kterém ke změně došlo (např. "Zaměstnanci", "Pokladna", "Sklad").
        /// </summary>
        [JsonPropertyName("modul")]
        public string Modul { get; internal set; } = modul;

        [JsonPropertyName("akce")]
        public TypAkce Akce { get; internal set; } = akce;

        /// <summary>
        /// Stručný popis, čeho se změna týká (např. "Novák Jan – mzda").
        /// </summary>
        [JsonPropertyName("popis")]
        public string Popis { get; internal set; } = popis;

        [JsonPropertyName("puvodniHodnota")]
        public string? PuvodniHodnota { get; internal set; } = puvodniHodnota;

        [JsonPropertyName("novaHodnota")]
        public string? NovaHodnota { get; internal set; } = novaHodnota;

        [JsonPropertyName("cas")]
        public DateTime Cas { get; internal set; } = cas;

        /// <summary>
        /// Uživatel, který změnu provedl (aktuálně přihlášený Windows uživatel).
        /// </summary>
        [JsonPropertyName("uzivatel")]
        public string Uzivatel { get; internal set; } = uzivatel;

        /// <summary>
        /// Vytvoří nový auditní záznam s automaticky vyplněným časem
        /// a aktuálně přihlášeným uživatelem.
        /// </summary>
        public static AuditZaznam Vytvor(string modul, TypAkce akce, string popis,
            string? puvodniHodnota = null, string? novaHodnota = null)
        {
            return new AuditZaznam(modul, akce, popis, puvodniHodnota, novaHodnota,
                DateTime.Now, Environment.UserName);
        }

        public void VypisZaznam()
        {
            Console.ForegroundColor = Akce 
            switch
            {
                TypAkce.Smazano => ConsoleColor.Red,
                TypAkce.Upraveno => ConsoleColor.Yellow,
                _ => ConsoleColor.Green
            };

            string zmena = PuvodniHodnota != null || NovaHodnota != null
                ? $" ({PuvodniHodnota ?? "-"} → {NovaHodnota ?? "-"})"
                : "";

            Console.WriteLine($"{Cas,-20:g} [{Modul}] {Akce}: {Popis}{zmena} – {Uzivatel}");
            Console.ResetColor();
        }
    }
}