using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Enumy;

namespace EvidenceZOOCviceniUpraveno2.Vypisy.Audity
{
    static class AuditVypisy
    {
        public static void VypisZaznam(AuditZaznam zaznam)
        {
            Console.ForegroundColor = zaznam.Akce switch
            {
                TypAkce.Smazano => ConsoleColor.Red,
                TypAkce.Upraveno => ConsoleColor.Yellow,
                _ => ConsoleColor.Green
            };

            string zmena = zaznam.PuvodniHodnota != null || zaznam.NovaHodnota != null
                ? $" ({zaznam.PuvodniHodnota ?? "-"} → {zaznam.NovaHodnota ?? "-"})"
                : "";

            Console.WriteLine($"{zaznam.Cas,-20:g} [{zaznam.Modul}] {zaznam.Akce}: {zaznam.Popis}{zmena} – {zaznam.Uzivatel}");
            Console.ResetColor();
        }
    }
}
