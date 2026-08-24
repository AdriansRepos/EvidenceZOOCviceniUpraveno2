using System;
namespace EvidenceZOOCviceniUpraveno2.Vypisy.Statistiky
{
    internal static class StatistikyVypis
    {
        public static void VypisPocetZvirat(int pocet)
        {
            Console.WriteLine($"Počet zvířat: {pocet}");
        }

        public static void VypisPocetZamestnancu(int pocet)
        {
            Console.WriteLine($"Počet zaměstnanců: {pocet}");
        }

        public static void VypisSoucetMezd(int soucet)
        {
            Console.WriteLine($"Součet mezd: {soucet} Kč");
        }

        public static void VypisNavstevnost(int mesic, int rok, double navstevnost)
        {
            Console.WriteLine($"Průměrná denní návštěvnost za {mesic}/{rok}: {navstevnost:0.##} osob/den");
        }
    }
}
