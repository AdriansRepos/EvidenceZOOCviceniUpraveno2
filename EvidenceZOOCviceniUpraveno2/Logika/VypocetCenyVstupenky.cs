using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Enumy;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    /// <summary>
    /// Vypočítá cenu vstupenky podle jejího typu a ceníku. U jednoduchých
    /// kategorií (dětská, dospělá, ZTP, důchodce) vrací cenu za jeden kus.
    /// U rodinných a skupinových vstupenek počítá součet cen jednotlivých
    /// členů se slevou.
    /// </summary>
    static class VypocetCenyVstupenky
    {
        /// <summary>
        /// Hlavní kalkulační metoda přijímající typ vstupenky, ceník a počty osob.
        /// </summary>
        public static decimal Vypocitej(TypVstupenky typ, Cenik cenik,
            int pocetDospelych = 0, int pocetDeti = 0)
        {
            return typ switch
            {
                TypVstupenky.Detska => cenik.CenaDetska,
                TypVstupenky.Dospela => cenik.CenaDospela,
                TypVstupenky.ZTP => cenik.CenaZTP,
                TypVstupenky.Duchodce => cenik.CenaDuchodce,

                // Úplná rodina: 2 dospělí + volitelný počet dětí se slevou
                TypVstupenky.RodinaUplna =>
                    SeSlevou(2 * cenik.CenaDospela + pocetDeti * cenik.CenaDetska, cenik.SlevaRodinaProcenta),

                // Neúplná rodina: 1 dospělý + volitelný počet dětí se slevou
                TypVstupenky.RodinaNeuplna =>
                    SeSlevou(cenik.CenaDospela + pocetDeti * cenik.CenaDetska, cenik.SlevaRodinaProcenta),

                // Skupinové vstupné: na základě zadaného počtu dospělých a dětí se slevou
                TypVstupenky.Skupina =>
                    SeSlevou(pocetDospelych * cenik.CenaDospela + pocetDeti * cenik.CenaDetska, cenik.SlevaSkupinaProcenta),

                _ => throw new ArgumentOutOfRangeException(nameof(typ), "Neznámý typ vstupenky.")
            };
        }

        /// <summary>
        /// Pomocná metoda pro uplatnění procentuální slevy z základní ceny.
        /// </summary>
        private static decimal SeSlevou(decimal zakladniCena, decimal slevaProcenta)
            => zakladniCena * (1 - slevaProcenta / 100m);
    }
}