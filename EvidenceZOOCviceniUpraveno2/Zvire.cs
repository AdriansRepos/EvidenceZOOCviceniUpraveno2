
namespace EvidenceZOOCviceniUpraveno2
{

    class Zvire
    {
        // Název zvířete – text, nikdy nesmí být null, proto výchozí hodnota ""
        public string Nazev { get; private set; } = "";

        // Věk zvířete v letech
        public int Vek { get; private set; }

        // Váha zvířete v kilogramech
        public double Vaha { get; private set; }

        // Konstruktor – při vytvoření objektu nastaví všechny vlastnosti
        public Zvire(string nazev, int vek, double vaha)
        {
            NastavNazev(nazev);
            NastavVek(vek);
            NastavVahu(vaha);
        }

        // Nastaví název a převede ho do hezkého formátu (TitleCase)
        internal void NastavNazev(string novyNazev)
        {
            Nazev = Vstupy.ToTitleCase(novyNazev);
        }

        // Nastaví věk zvířete
        internal void NastavVek(int novyVek)
        {
            Vek = novyVek;
        }

        // Nastaví váhu zvířete
        internal void NastavVahu(double novaVaha)
        {
            Vaha = novaVaha;
        }

        // Vypíše informace o zvířeti včetně správného skloňování věku
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

        // Převede objekt na řetězec pro uložení do souboru
        public string ToFileString()
        {
            return $"{Nazev}|{Vek}|{Vaha}";
        }

        // Vytvoří objekt Zvire z jednoho řádku textu v souboru
        public static Zvire Parse(string line)
        {
            // Rozdělení řádku podle svislé čáry
            string[] parts = line.Split('|');

            // Kontrola správného počtu položek
            if (parts.Length != 3)
                throw new FormatException("Řádek nemá správný formát pro Zvire.");

            // Kontrola názvu
            if (string.IsNullOrWhiteSpace(parts[0]))
                throw new FormatException("Název nesmí být prázdný.");
            string nazev = parts[0];

            // Kontrola a převod věku
            if (!int.TryParse(parts[1], out int vek))
                throw new FormatException("Věk není platné číslo.");

            // Kontrola a převod váhy
            if (!double.TryParse(parts[2], out double vaha))
                throw new FormatException("Váha není platné číslo.");

            // Vytvoření nového objektu
            return new Zvire(nazev, vek, vaha);
        }
    }
}
