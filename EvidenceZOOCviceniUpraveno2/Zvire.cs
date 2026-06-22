
namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje jedno zvíře v zoologické zahradě.
    /// Uchovává jeho název, věk a váhu.
    /// </summary>
    class Zvire
    {
        /// <summary>
        /// Název zvířete. Nikdy nesmí být null.
        /// Je automaticky převáděn do formátu TitleCase.
        /// </summary>
        public string Nazev { get; private set; } = "";

        /// <summary>
        /// Věk zvířete v letech.
        /// </summary>
        public int Vek { get; private set; }

        /// <summary>
        /// Váha zvířete v kilogramech.
        /// </summary>
        public double Vaha { get; private set; }

        /// <summary>
        /// Vytvoří nové zvíře a nastaví jeho název, věk a váhu.
        /// </summary>
        /// <param name="nazev">Název zvířete.</param>
        /// <param name="vek">Věk v letech.</param>
        /// <param name="vaha">Váha v kilogramech.</param>
        public Zvire(string nazev, int vek, double vaha)
        {
            NastavNazev(nazev);
            NastavVek(vek);
            NastavVahu(vaha);
        }

        /// <summary>
        /// Nastaví název zvířete a převede jej do formátu TitleCase.
        /// </summary>
        /// <param name="novyNazev">Nový název zvířete.</param>
        internal void NastavNazev(string novyNazev)
        {
            Nazev = Vstupy.ToTitleCase(novyNazev);
        }

        /// <summary>
        /// Nastaví věk zvířete.
        /// </summary>
        /// <param name="novyVek">Nový věk v letech.</param>
        internal void NastavVek(int novyVek)
        {
            Vek = novyVek;
        }

        /// <summary>
        /// Nastaví váhu zvířete.
        /// </summary>
        /// <param name="novaVaha">Nová váha v gramech (kilogramech).</param>
        internal void NastavVahu(double novaVaha)
        {
            Vaha = novaVaha;
        }

        /// <summary>
        /// Vypíše informace o zvířeti do konzole,
        /// včetně správného skloňování slova „rok“.
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

        /// <summary>
        /// Převede objekt zvířete na řetězec vhodný pro uložení do souboru.
        /// </summary>
        /// <returns>Řetězec ve formátu "Nazev|Vek|Vaha".</returns>
        public string ToFileString()
        {
            return $"{Nazev}|{Vek}|{Vaha}";
        }

        /// <summary>
        /// Vytvoří objekt <see cref="Zvire"/> z jednoho řádku textu v souboru.
        /// </summary>
        /// <param name="line">Řádek textu obsahující údaje o zvířeti.</param>
        /// <returns>Nově vytvořený objekt zvířete.</returns>
        /// <exception cref="FormatException">
        /// Vyvolána, pokud řádek nemá správný formát nebo obsahuje neplatná data.
        /// </exception>
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
