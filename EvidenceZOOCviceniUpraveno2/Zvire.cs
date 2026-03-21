using System.Globalization;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Vytvoření instance Zvíře
    /// </summary>
    /// <param name="nazev">Název zvířete</param>
    /// <param name="vek">Věk zvířete</param>
    /// <param name="vaha">Váha zvířete</param>
    class Zvire(string nazev, int vek, double vaha)
    {
        /// <summary>
        /// Název zvířete
        /// </summary>
        public string Nazev { get; private set; } = Vstupy.ToTitleCase(nazev);

        /// <summary>
        /// Věk zvířete
        /// </summary>
        public int Vek { get; private set; } = vek;

        /// <summary>
        /// Váha zvířete
        /// </summary>
        public double Vaha { get; private set; } = vaha;

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

        /// <summary>
        /// Podmenu pro práci se zvířaty
        /// </summary>
        public static void MenuZvirata(ZOO zoo)
        {
            char volba;
            do
            {
                Console.WriteLine("\n=== MENU ZVÍŘATA ===");
                Console.WriteLine("\t1. Přidat zvíře");
                Console.WriteLine("\t2. Vypsat zvířata");
                Console.WriteLine("\t3. Smazat zvíře");
                Console.WriteLine("\t4. Upravit zvíře");
                Console.WriteLine("\t5. Vyhledat zvíře");
                Console.WriteLine("\t6. Návrat do hlavního menu");

                volba = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (volba)
                {
                    case '1':
                        PridatZvire(zoo);
                        break;

                    case '2':
                        VypisZvirat(zoo);
                        break;

                    case '3':
                        SmazatZvire(zoo);
                        break;

                    case '4':
                        UpravitZvire(zoo);
                        break;

                    case '5':
                        VyhledatZvire(zoo);
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
        /// Metoda pro přidání zvířat
        /// </summary>
        public static void PridatZvire(ZOO zoo)
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZVÍŘETE");

            string nazev = Vstupy.ZeptejSeAUpravString("", "název", "Nový", true);
            int vek = Vstupy.ZeptejSeAUpravInt(0, "věk", true);
            double vaha = Vstupy.ZeptejSeAUpravDouble(0, "váha", true);


            Zvire zvire = new(nazev, vek, vaha);
            zoo.Zvirata.Add(zvire);

            Console.WriteLine();
            Console.WriteLine("Zvíře bylo úspěšně přidáno.");
        }


        /// <summary>
        /// Metoda pro výpis všech zvířat
        /// </summary>
        public static void VypisZvirat(ZOO zoo)
        {
            Console.WriteLine("VÝPIS ZVÍŘAT");
            foreach (Zvire zvire in zoo.Zvirata) // iterace kolekce
            {
                zvire.VypisZvire(); // vypsání všech zvířat nacházejících se v kolekci
            }
        }

        /// <summary>
        /// Metoda pro smazání zvířete
        /// </summary>
        public static void SmazatZvire(ZOO zoo)
        {
            Console.WriteLine("SMAZÁNÍ ZVÍŘETE");
            int index = Vstupy.VybratIndexZvirete(zoo); // vyhledání zvířete dle indexu
            if (index >= 0)
            {
                Console.WriteLine("Zvíře: {0} bylo smazáno", zoo.Zvirata[index].Nazev);
                zoo.Zvirata.RemoveAt(index); // smazání zvířete dle jeho indexu
            }
        }

        /// <summary>
        /// Metoda pro úpravu zvířete
        /// </summary>
        public static void UpravitZvire(ZOO zoo)
        {
            Console.WriteLine("ÚPRAVA ZVÍŘETE");
            int index = Vstupy.VybratIndexZvirete(zoo);

            if (index >= 0)
            {
                Zvire zvire = zoo.Zvirata[index];

                zvire.Nazev = Vstupy.ZeptejSeAUpravString(zvire.Nazev, "název", "Nový");
                zvire.Vek = Vstupy.ZeptejSeAUpravInt(zvire.Vek, "věk");
                zvire.Vaha = Vstupy.ZeptejSeAUpravDouble(zvire.Vaha, "váha");


                Console.WriteLine();                 // oddělí výpis
                Console.WriteLine("Úprava dokončena");
            }
        }


        /// <summary>
        /// Vyhledání zvířat podle konkrétního výrazu
        /// </summary>
        public static void VyhledatZvire(ZOO zoo)
        {
            Console.Write("Zadejte hledaný výraz: ");
            string hledany = Console.ReadLine()!.Trim().ToLower();

            bool necoNalezeno = false;

            foreach (var zvire in zoo.Zvirata)
            {
                if (zvire.Nazev.Contains(hledany, StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine(
                        $"Nalezeno:\t{Vstupy.ToTitleCase(zvire.Nazev)}" +
                        $"\tVěk: {zvire.Vek}" +
                        $"\tVáha: {zvire.Vaha}"
                    );

                    necoNalezeno = true;
                }
            }

            if (!necoNalezeno)
            {
                Console.WriteLine("Zadané zvíře se v databázi nenachází.");
            }
        }               
                
    }
}
