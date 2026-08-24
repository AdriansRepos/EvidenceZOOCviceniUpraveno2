using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Enumy;

namespace EvidenceZOOCviceniUpraveno2.Vypisy.System
{
    static class TechnickyZaznamVypisy
    {
        public static void VypisTechnickyZaznam(TechnickyZaznam zaznam)
        {
            Console.ForegroundColor = zaznam.Uroven switch
            {
                UrovenLogu.Chyba => ConsoleColor.Red,
                UrovenLogu.Varovani => ConsoleColor.Yellow,
                _ => ConsoleColor.Gray
            };

            Console.WriteLine($"{zaznam.Cas,-20:g} [{zaznam.Uroven}] {zaznam.Zprava}");
            Console.ResetColor();
        }
    }
}
