using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.Logika;
using EvidenceZOOCviceniUpraveno2.Vypisy.Statistiky;
using TextHelper;

namespace EvidenceZOOCviceniUpraveno2.Menu
{
    internal class MenuStatistiky(Statistiky statistiky)
    {
        private readonly Statistiky statistiky = statistiky;

        public void Zobraz()
        {
            statistiky.NactiData();
            char volba;

            do            
            {                
                VypisyDoKonzole.VypisHlavickuMenu("STATISTIKY");
                VypisyDoKonzole.VypisTeloMenu(
                    "1. Počet zvířat",
                    "2. Počet zaměstnanců",
                    "3. Součet mezd zaměstnanců",
                    "4. Průměrná denní návštěvnost (za měsíc)",
                    "n. Návrat do hlavního menu"
                );
                VypisyDoKonzole.VypisVyzvuKZadani();

                volba = char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();

                switch (volba)
                {
                    case '1':
                        StatistikyVypisy.VypisPocetZvirat(statistiky.PocetZvirat());
                        break;

                    case '2':
                        StatistikyVypisy.VypisPocetZamestnancu(statistiky.PocetZamestnancu());
                        break;

                    case '3':
                        StatistikyVypisy.VypisSoucetMezd(statistiky.SoucetMezd());
                        break;

                    case '4':
                        ZobrazNavstevnost();
                        break;

                    case 'n':
                        break;

                    default:
                        VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání: ");
                        break;
                }
            }
            while (volba != 'n');
        }

        private void ZobrazNavstevnost()
        {
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

            double navstevnost = statistiky.PrumernaDenniNavstevnost(rokNavstevnost, mesicNavstevnost);

            StatistikyVypisy.VypisNavstevnost(mesicNavstevnost, rokNavstevnost, navstevnost);
        }
    }
}