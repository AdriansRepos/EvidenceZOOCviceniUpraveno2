
namespace EvidenceZOOCviceniUpraveno2
{
    
    class Zvire
    {
        /// <summary>
        /// Název zvířete
        /// </summary>
        public string Nazev { get; private set; }

        /// <summary>
        /// Věk zvířete
        /// </summary>
        public int Vek { get; private set; }

        /// <summary>
        /// Váha zvířete
        /// </summary>
        public double Vaha { get; private set; }

        /// <summary>
        /// Vytvoření instance Zvíře
        /// </summary>
        /// <param name="nazev">Název zvířete</param>
        /// <param name="vek">Věk zvířete</param>
        /// <param name="vaha">Váha zvířete</param>
        public Zvire(string nazev, int vek, double vaha)
        {
            NastavNazev(nazev);
            NastavVek(vek);
            NastavVahu(vaha);
        }

        internal void NastavNazev(string novyNazev)
        {
            Nazev = Vstupy.ToTitleCase(novyNazev);
        }

        internal void NastavVek(int novyVek)
        {
            Vek = novyVek; 
        }

        internal void NastavVahu(double novaVaha)
        {
            Vaha = novaVaha; 
        }

        /// <summary>
        /// Metoda pro výpis zvířat
        /// </summary>
        public void VypisZvire()
        {
            Console.WriteLine("Název zvířete: {0}", Nazev);

            if (Vek == 1)
            {
                Console.WriteLine("\tVěk zvířete: {0} rok", Vek);
            }
            else if (Vek >= 2 && Vek <= 4)
            {
                Console.WriteLine("\tVěk zvířete: {0} roky", Vek);
            }
            else
            {
                Console.WriteLine("\tVěk zvířete: {0} let", Vek);
            }

            Console.WriteLine("\tVáha: {0} kg", Vaha);
        }   
    }
}
