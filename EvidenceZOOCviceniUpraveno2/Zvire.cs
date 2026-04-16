
namespace EvidenceZOOCviceniUpraveno2
{
    
    class Zvire
    {        
        public string Nazev { get; private set; } = "";

        public int Vek { get; private set; }

        public double Vaha { get; private set; }

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

        public string ToFileString()
        {
            return $"{Nazev};{Vek};{Vaha}";
        }

        public static Zvire Parse(string line)
        {
            string[] parts = line.Split(';');

            if (parts.Length != 3)
                throw new FormatException("Řádek nemá správný formát pro Zvire.");

            if (string.IsNullOrWhiteSpace(parts[0]))
                throw new FormatException("Název nesmí být prázdný.");
            string nazev = parts[0];

            if (!int.TryParse(parts[1], out int vek))
                throw new FormatException("Věk není platné číslo.");

            if (!double.TryParse(parts[2], out double vaha))
                throw new FormatException("Váha není platné číslo.");

            return new Zvire(nazev, vek, vaha);
        }

    }
}
