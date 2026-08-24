using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Enumy;
using PohybHelper;

namespace EvidenceZOOCviceniUpraveno2.Vypisy
{
    internal static class PokladnaVypisy
    {
        public static void VypisJedenPohyb(PokladniPohyb pohyb)
        {
            Console.ForegroundColor = pohyb.TypPohybu == PokladniTypPohybu.Prodej
                ? ConsoleColor.Green
                : ConsoleColor.DarkYellow;

            Console.WriteLine(pohyb.ToString());
            Console.ResetColor();
        }

        public static void VypisUzaverku(List<PokladniPohyb> pohyby, string nadpis)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"=== {nadpis} ===");
            Console.ResetColor();

            var prodeje = pohyby.Where(p => p.TypPohybu == PokladniTypPohybu.Prodej).ToList();
            var storna = pohyby.Where(p => p.TypPohybu == PokladniTypPohybu.Storno).ToList();

            if (prodeje.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("PRODEJE PODLE KATEGORIÍ:");
                VypisRozpadPodleKategorie(prodeje);
            }

            if (storna.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("STORNA PODLE KATEGORIÍ:");
                VypisRozpadPodleKategorie(storna);
            }

            decimal celkovyPrijemZProdeju = prodeje.Sum(p => p.Castka);
            decimal celkoveStorno = storna.Sum(p => p.Castka);
            decimal cistyPrijem = celkovyPrijemZProdeju - celkoveStorno;

            Console.WriteLine();

            if (prodeje.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Celkový příjem z prodejů: {celkovyPrijemZProdeju:0.##} Kč");
                Console.ResetColor();
            }

            if (storna.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Celkové storno:           {celkoveStorno:0.##} Kč");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Čistý příjem:             {cistyPrijem:0.##} Kč");
            Console.ResetColor();
        }

        private static void VypisRozpadPodleKategorie(List<PokladniPohyb> pohyby)
        {
            if (pohyby.Count == 0)
            {
                Console.WriteLine("  (žádné)");
                return;
            }

            var skupiny = pohyby.GroupBy(p => p.TypVstupenky);

            foreach (var skupina in skupiny)
            {
                string nazevKategorie = PopiskyHelper.ZiskejPopisek(skupina.Key);
                int pocetOsob = skupina.Sum(p =>
                    p.TypVstupenky is TypVstupenky.Detska or TypVstupenky.Dospela or TypVstupenky.ZTP or TypVstupenky.Duchodce
                        ? p.PocetKusu
                        : p.PocetDospelych + p.PocetDeti);
                decimal castka = skupina.Sum(p => p.Castka);

                Console.WriteLine($"  {nazevKategorie,-45} {pocetOsob,4} osob   {castka,10:0.##} Kč");
            }
        }
    }
}