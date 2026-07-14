namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Vypočítá cenu vstupenky podle jejího typu a ceníku. U jednoduchých
    /// kategorií (dětská, dospělá, ZTP, důchodce) vrací cenu za jeden kus.
    /// U rodinných a skupinových vstupenek počítá součet cen jednotlivých
    /// členů se slevou.
    /// </summary>
    static class VypocetCenyVstupenky
    {
        public static decimal Vypocitej(TypVstupenky typ, Cenik cenik,
            int pocetDospelych = 0, int pocetDeti = 0)
        {
            return typ switch
            {
                TypVstupenky.Detska => cenik.CenaDetska,
                TypVstupenky.Dospela => cenik.CenaDospela,
                TypVstupenky.ZTP => cenik.CenaZTP,
                TypVstupenky.Duchodce => cenik.CenaDuchodce,

                TypVstupenky.RodinaUplna =>
                    SeSlevou(2 * cenik.CenaDospela + pocetDeti * cenik.CenaDetska, cenik.SlevaRodinaProcenta),

                TypVstupenky.RodinaNeuplna =>
                    SeSlevou(cenik.CenaDospela + pocetDeti * cenik.CenaDetska, cenik.SlevaRodinaProcenta),

                TypVstupenky.Skupina =>
                    SeSlevou(pocetDospelych * cenik.CenaDospela + pocetDeti * cenik.CenaDetska, cenik.SlevaSkupinaProcenta),

                _ => throw new ArgumentOutOfRangeException(nameof(typ), "Neznámý typ vstupenky.")
            };
        }

        private static decimal SeSlevou(decimal zakladniCena, decimal slevaProcenta)
            => zakladniCena * (1 - slevaProcenta / 100m);
    }
}
