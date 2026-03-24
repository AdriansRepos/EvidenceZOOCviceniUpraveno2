
namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Vytvoření instance zaměstnance
    /// </summary>
    /// <param name="jmeno">jméno zaměstnance</param>
    /// <param name="prijmeni">příjmení zaměstnance</param>
    /// <param name="datumNarozeni">datum narození zaměstnance</param>
    /// <param name="mzda">mzda zaměstnance</param>
    /// <param name="pracovniPozice">pracovní pozice zaměstnance</param>
    class Zamestnanec(string jmeno, string prijmeni, DateOnly datumNarozeni,
                       int mzda, string pracovniPozice)
    {
        /// <summary>
        /// Jméno zaměstnance
        /// </summary>
        public string Jmeno { get; set; } = Vstupy.ToTitleCase(jmeno);

        /// <summary>
        /// Příjmení zaměstnance
        /// </summary>
        public string Prijmeni { get; set; } = Vstupy.ToTitleCase(prijmeni);

        /// <summary>
        /// Datum narození zamstnance
        /// </summary>
        public DateOnly DatumNarozeni { get; set; } = datumNarozeni;

        /// <summary>
        /// Mzda zaměstnance
        /// </summary>
        public int Mzda { get; set; } = mzda;

        /// <summary>
        /// Pracovní pozice zaměstnance
        /// </summary>
        public string PracovniPozice { get; set; } = Vstupy.ToTitleCase(pracovniPozice);

        /// <summary>
        /// Metoda pro výpis zaměstnance
        /// </summary>
        public void VypisZamestnance()
        {
            Console.WriteLine("Jméno zaměstnance: {0}", Jmeno);

            Console.WriteLine("\tPříjmení zaměstnance: {0}", Prijmeni);

            Console.WriteLine("\tDatum narození zaměstnance: {0}", DatumNarozeni);

            Console.WriteLine("\tMzda zaměstnance: {0}", Mzda);

            Console.WriteLine("\tPracovní pozice zaměstnance: {0}", PracovniPozice);
        }                
    }
}
