using EvidenceZOOCviceniUpraveno2.Enumy;
using EvidenceZOOCviceniUpraveno2.Data;
using PohybHelper;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    internal class Statistiky(Zoo zoo)
    {
        private readonly Zoo zoo = zoo;

        public void NactiData()
        {
            zoo.Zamestnanci.Nacti();
            zoo.Zvirata.Nacti();
            zoo.Pokladna.Nacti();
        }

        public int PocetZvirat() => zoo.Zvirata.Zvirata.Count;
        
        public int PocetZamestnancu() => zoo.Zamestnanci.Zamestnanci.Count;
        
        public int SoucetMezd() => zoo.Zamestnanci.Zamestnanci.Sum(z => z.Mzda);

        public double PrumernaDenniNavstevnost(int rok, int mesic)
        {
            var pohybyVMesici = zoo.Pokladna.PokladniPohyby
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