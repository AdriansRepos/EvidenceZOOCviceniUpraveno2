using PohybHelper;
using TextHelper;

namespace EvidenceZOOCviceniUpraveno2
{
    class Statistiky(ZOO zoo)
    {
        private readonly ZOO zoo = zoo;
                
        public void MenuStatistiky()
        {
            zoo.ZajistiData("Zaměstnanci");
            zoo.ZajistiData("Zvířata");
            zoo.ZajistiData("Pokladna");
            char volba;
            do
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n=== STATISTIKY ===");
                Console.WriteLine("\t1. Počet zvířat");
                Console.WriteLine("\t2. Počet zaměstnanců");
                Console.WriteLine("\t3. Součet mezd zaměstnanců");
                Console.WriteLine("\t4. Průměrná denní návštěvnost (za měsíc)");
                Console.WriteLine("\t5. Návrat do hlavního menu");
                Console.ResetColor();
                Console.Write("Vyber možnost: ");

                volba = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (volba)
                {
                    case '1':
                        Console.WriteLine($"Počet zvířat: {PocetZvirat()}");
                        break;

                    case '2':
                        Console.WriteLine($"Počet zaměstnanců: {PocetZamestnancu()}");
                        break;

                    case '3':
                        Console.WriteLine($"Součet mezd: {SoucetMezd()} Kč");
                        break;

                    case '4':
                        int rokNavstevnost = UpravaVstupu.ZeptejSeAUprav(
                            DateTime.Now.Year, "rok (např. 2026)",
                            v => v.ToString(), s => int.Parse(s), jeNove: true);

                        int mesicNavstevnost = UpravaVstupu.ZeptejSeAUprav(
                            DateTime.Now.Month, "měsíc (1-12)",
                            v => v.ToString(),
                            s =>
                            {
                                int m = int.Parse(s);
                                if (m < 1 || m > 12)
                                    throw new ArgumentOutOfRangeException(nameof(s), "Měsíc musí být 1-12.");
                                return m;
                            },
                            jeNove: true);

                        double navstevnost = PrumernaDenniNavstevnost(rokNavstevnost, mesicNavstevnost);

                        Console.WriteLine($"Průměrná denní návštěvnost za {mesicNavstevnost}/{rokNavstevnost}: {navstevnost:0.##} osob/den");
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Neplatná volba, opakujte zadání:");
                        Console.ResetColor();
                        break;
                }
            }
            while (volba != '5');
        }

        public int PocetZvirat() => zoo.Zvirata.Count;
        public int PocetZamestnancu() => zoo.Zamestnanci.Count;
        public int SoucetMezd() => zoo.Zamestnanci.Sum(z => z.Mzda);

        public double PrumernaDenniNavstevnost(int rok, int mesic)
        {
            var pohybyVMesici = zoo.PokladniPohyby
                .Where(p => p.DatumCas.Year == rok && p.DatumCas.Month == mesic)
                .ToList();

            if (pohybyVMesici.Count == 0)
                return 0;

            int celkemOsob = pohybyVMesici.Sum(p =>
            {
                int pocetOsob = p.TypVstupenky is TypVstupenky.Detska or TypVstupenky.Dospela or TypVstupenky.ZTP or TypVstupenky.Duchodce
                    ? p.PocetKusu
                    : p.PocetDospelych + p.PocetDeti;

                return p.TypPohybu == PokladniTypPohybu.Prodej ? pocetOsob : -pocetOsob;
            });

            int pocetDniVMesici = DateTime.DaysInMonth(rok, mesic);
            return (double)celkemOsob / pocetDniVMesici;
        }
    }
}
