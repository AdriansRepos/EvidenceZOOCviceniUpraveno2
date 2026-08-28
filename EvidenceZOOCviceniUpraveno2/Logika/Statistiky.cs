using EvidenceZOOCviceniUpraveno2.Enumy;
using PohybHelper;
using EvidenceZOOCviceniUpraveno2.Entity;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    /// <summary>
    /// Poskytuje souhrnné statistické ukazatele a výpočty napříč všemi moduly ZOO.
    /// </summary>
    internal class Statistiky(Zoo zoo)
    {
        private readonly Zoo zoo = zoo;

        /// <summary>
        /// Zabezpečí načtení všech potřebných datových modulů.
        /// </summary>
        public void NactiData()
        {
            zoo.Zamestnanci.Nacti();
            zoo.Zvirata.Nacti();
            zoo.Pokladna.Nacti();
        }

        /// <summary>
        /// Vrací celkový počet evidovaných zvířat.
        /// </summary>
        public int PocetZvirat() => zoo.Zvirata.Zvirata.Count;
        
        /// <summary>
        /// Vrací celkový počet evidovaných zaměstnanců.
        /// </summary>
        public int PocetZamestnancu() => zoo.Zamestnanci.Zamestnanci.Count;
        
        /// <summary>
        /// Spočíta celkový měsíční objem vyplácených mezd.
        /// </summary>
        public int SoucetMezd() => zoo.Zamestnanci.Zamestnanci.Sum(z => z.Mzda);

        /// <summary>
        /// Vypočítá průměrný denní počet návštěvníků v daném měsíci a roce na základě prodaných a stornovaných vstupenek.
        /// </summary>
        public double PrumernaDenniNavstevnost(int rok, int mesic)
        {
            // Filtr pohybů pro zadaný rok a měsíc
            var pohybyVMesici = zoo.Pokladna.PokladniPohyby
                .Where(p => p.DatumCas.Year == rok && p.DatumCas.Month == mesic)
                .ToList();

            if (pohybyVMesici.Count == 0)
                return 0;

            // Agregace počtu návštěvníků s přihlédnutím k typům vstupenek a přičtení/odečtení storn
            int celkemOsob = pohybyVMesici.Sum(p =>
            {
                int pocetOsob = p.TypVstupenky is TypVstupenky.Detska or TypVstupenky.Dospela or TypVstupenky.ZTP or TypVstupenky.Duchodce
                    ? p.PocetKusu
                    : p.PocetDospelych + p.PocetDeti;

                return p.TypPohybu == PokladniTypPohybu.Prodej ? pocetOsob : -pocetOsob;
            });

            // Přepočet průměru na počet dní v příslušném měsíci
            int pocetDniVMesici = DateTime.DaysInMonth(rok, mesic);
            return (double)celkemOsob / pocetDniVMesici;
        }
    }
}