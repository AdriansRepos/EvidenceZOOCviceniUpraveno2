
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
        public string Jmeno { get; private set; } = Vstupy.ToTitleCase(jmeno);

        /// <summary>
        /// Příjmení zaměstnance
        /// </summary>
        public string Prijmeni { get; private set; } = Vstupy.ToTitleCase(prijmeni);

        /// <summary>
        /// Datum narození zamstnance
        /// </summary>
        public DateOnly DatumNarozeni { get; private set; } = datumNarozeni;

        /// <summary>
        /// Mzda zaměstnance
        /// </summary>
        public int Mzda { get; private set; } = mzda;

        /// <summary>
        /// Pracovní pozice zaměstnance
        /// </summary>
        public string PracovniPozice { get; private set; } = Vstupy.ToTitleCase(pracovniPozice);

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

        /// <summary>
        /// Podmenu pro práci se zaměstnanci
        /// </summary>
        public static void MenuZamestnanci(ZOO zoo)
        {
            char volba;
            do
            {
                Console.WriteLine("\n=== MENU ZAMŠTNANCI ===");
                Console.WriteLine("\t1. Přidat zaměstnance");
                Console.WriteLine("\t2. Vypsat zaměstnance");
                Console.WriteLine("\t3. Smazat zaměstnance");
                Console.WriteLine("\t4. Upravit zaměstnance");
                Console.WriteLine("\t5. Vyhledat zaměstnance");
                Console.WriteLine("\t6. Návrat do hlavního menu");

                volba = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (volba)
                {
                    case '1':
                        PridatZamestnance(zoo);
                        break;

                    case '2':
                        VypisZamestnancu(zoo);
                        break;

                    case '3':
                        SmazatZamestnance(zoo);
                        break;

                    case '4':
                        UpravitZamestnance(zoo);
                        break;

                    case '5':
                        VyhledatZamestnance(zoo);
                        break;

                    case '6':
                        Console.WriteLine();
                        break;

                    default:
                        Console.WriteLine("Neplatná volba, opakujte zadání:");
                        break;
                }

            } 
            while (volba != '6');
        }

        /// <summary>
        /// Metoda pro přidání zaměstnanců
        /// </summary>
        public static void PridatZamestnance(ZOO zoo)
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZAMĚSTNANCE");

            string jmeno = Vstupy.ZeptejSeAUpravString("", "jméno", "Nové", true);
            string prijmeni = Vstupy.ZeptejSeAUpravString("", "příjmení", "Nové", true);
            string pracovniPozice = Vstupy.ZeptejSeAUpravString("", "pracovní pozice", "Nová", true);
            int mzda = Vstupy.ZeptejSeAUpravInt(0, "mzda", true);

            Console.Write("Datum narození (formát d.M.rrrr): ");
            DateOnly datumNarozeni = DateOnly.Parse(Console.ReadLine()!);

            // vytvoření nového záznamu
            Zamestnanec zamestnanec = new(jmeno, prijmeni, datumNarozeni, mzda, pracovniPozice);
            zoo.Zamestnanci.Add(zamestnanec); // přidání záznamu do kolekce
        }

        /// <summary>
        /// Metoda pro výpis všech zaměstnanců
        /// </summary>
        public static void VypisZamestnancu(ZOO zoo)
        {
            Console.WriteLine("VÝPIS ZAMĚSTNANCŮ");
            foreach (Zamestnanec zamestnanec in zoo.Zamestnanci) // iterace kolekce
            {
                zamestnanec.VypisZamestnance(); // vypsání všech zaměstnanců nacházejících se v kolekci
            }
        }

        /// <summary>
        /// Metoda pro smazání zaměstnance
        /// </summary>
        public static void SmazatZamestnance(ZOO zoo)
        {
            Console.WriteLine("SMAZÁNÍ ZAMĚSTNANCE");
            int index = Vstupy.VybratIndexZamestnance(zoo); // vyhledání zaměstnance dle indexu
            if (index >= 0)
            {
                Console.WriteLine("Zaměstnanec: {0} byl smazán", zoo.Zamestnanci[index].Prijmeni);
                zoo.Zamestnanci.RemoveAt(index); // smazání zaměstnance dle jeho indexu
            }
        }

        /// <summary>
        /// Metoda pro úpravu zaměstnance
        /// </summary>
        public static void UpravitZamestnance(ZOO zoo)
        {
            Console.WriteLine("ÚPRAVA ZAMĚSTNANCE");
            int index = Vstupy.VybratIndexZamestnance(zoo);
            // vyhledání zaměstnance dle jeho indexu

            if (index >= 0)
            {
                Zamestnanec zamestnanec = zoo.Zamestnanci[index];

                zamestnanec.Jmeno = Vstupy.ZeptejSeAUpravString(zamestnanec.Jmeno, "jméno", "Nové");
                zamestnanec.Prijmeni = Vstupy.ZeptejSeAUpravString(zamestnanec.Prijmeni, "příjmení", "Nové");
                zamestnanec.PracovniPozice = Vstupy.ZeptejSeAUpravString(zamestnanec.PracovniPozice, "pracovní pozice", "Nová");

                Console.WriteLine("Aktuální datum narození: {0}\nChcete upravit tuto položku? A/N", zamestnanec.DatumNarozeni);
                if (Console.ReadLine()!.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.Write("Nové datum narození (formát d.M.rrrr): ");
                    DateOnly noveDatumNarozeni;

                    while (!DateOnly.TryParse(Console.ReadLine(), out noveDatumNarozeni))
                    {
                        Console.WriteLine("Neplatné zadání, zadejte prosím číslo:");
                        Console.Write("Nové datum narození: ");
                    }

                    zamestnanec.DatumNarozeni = noveDatumNarozeni;
                }

                zamestnanec.Mzda = Vstupy.ZeptejSeAUpravInt(zamestnanec.Mzda, "mzda");

                Console.WriteLine();
                Console.WriteLine("Úprava dokončena");
            }
        }

        /// <summary>
        /// Vyhledání zaměstnanců podle konkrétního příjmení
        /// </summary>
        public static void VyhledatZamestnance(ZOO zoo)
        {
            Console.Write("Zadejte hledané příjmení: ");
            string hledany = Console.ReadLine()!.ToLower();

            bool necoNalezeno = false;

            foreach (var zamestnanec in zoo.Zamestnanci)
            {
                if (zamestnanec.Prijmeni.Contains(hledany, StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine(
                        $"Nalezeno:\t{Vstupy.ToTitleCase(zamestnanec.Prijmeni)}" +
                        $"\t{Vstupy.ToTitleCase(zamestnanec.Jmeno)}" +
                        $"\t{zamestnanec.DatumNarozeni}" +
                        $"\t{zamestnanec.Mzda}" +
                        $"\t{Vstupy.ToTitleCase(zamestnanec.PracovniPozice)}"
                    );

                    necoNalezeno = true;
                }
            }

            if (!necoNalezeno)
            {
                Console.WriteLine("Zadaný zaměstnanec se v databázi nenachází.");
            }
        }
               
                
    }
}
