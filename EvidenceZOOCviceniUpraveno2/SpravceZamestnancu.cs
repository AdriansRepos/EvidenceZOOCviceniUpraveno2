using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceZOOCviceniUpraveno2
{
    class SpravceZamestnancu(ZOO zoo)
    {
        private readonly ZOO zoo = zoo;

        public void Menu()
        {
            char volba;
            do
            {
                Console.WriteLine("\n=== MENU ZAMĚSTNANCI ===");
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
                        Pridat(); 
                        break;

                    case '2': 
                        Vypis(); 
                        break;

                    case '3': 
                        Smazat(); 
                        break;

                    case '4': 
                        Upravit(); 
                        break;

                    case '5': 
                        Vyhledat(); 
                        break;

                    case '6': 
                        Console.WriteLine(); 
                        break;

                    default: 
                        Console.WriteLine("Neplatná volba."); 
                        break;
                }

            } while (volba != '6');
        }

        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZAMĚSTNANCE");

            string jmeno = Vstupy.ZeptejSeAUpravString("", "jméno", "Nové", true);
            string prijmeni = Vstupy.ZeptejSeAUpravString("", "příjmení", "Nové", true);
            string pracovniPozice = Vstupy.ZeptejSeAUpravString("", "pracovní pozice", "Nová", true);
            int mzda = Vstupy.ZeptejSeAUpravInt(0, "mzda", true);

            DateOnly datumNarozeni;

            while (true)
            {
                Console.Write("Datum narození (formát d.M.rrrr): ");
                string vstup = Console.ReadLine()!;

                if (DateOnly.TryParse(vstup, out datumNarozeni))
                    break;

                Console.WriteLine("Neplatný formát data. Zkus to znovu.");
            }

            zoo.Zamestnanci.Add(new Zamestnanec(jmeno, prijmeni, datumNarozeni, mzda, pracovniPozice));

            Console.WriteLine("Zaměstnanec byl úspěšně přidán.");
        }

        public void Vypis()
        {
            Console.WriteLine("VÝPIS ZAMĚSTNANCŮ");
            foreach (var zam in zoo.Zamestnanci)
                zam.VypisZamestnance();
        }

        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZAMĚSTNANCE");
            int index = Vstupy.VybratIndexZamestnance(zoo);

            if (index >= 0)
            {
                Console.WriteLine($"Zaměstnanec {zoo.Zamestnanci[index].Prijmeni} byl smazán.");
                zoo.Zamestnanci.RemoveAt(index);
            }
        }

        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZAMĚSTNANCE");
            int index = Vstupy.VybratIndexZamestnance(zoo);

            if (index >= 0)
            {
                var zam = zoo.Zamestnanci[index];

                zam.NastavJmeno(Vstupy.ZeptejSeAUpravString(zam.Jmeno, "jméno", "Nové"));
                zam.NastavPrijmeni(Vstupy.ZeptejSeAUpravString(zam.Prijmeni, "příjmení", "Nové"));
                zam.NastavPracovniPozici(Vstupy.ZeptejSeAUpravString
                    (zam.PracovniPozice, "pracovní pozice", "Nová"));

                Console.WriteLine($"Aktuální datum narození: {zam.DatumNarozeni}\nChcete upravit tuto položku? A/N");
                if (Console.ReadLine()!.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.Write("Nové datum narození (formát d.M.rrrr): ");
                    DateOnly noveDatum;

                    while (!DateOnly.TryParse(Console.ReadLine(), out noveDatum))
                    {
                        Console.WriteLine("Neplatné zadání, zkuste znovu!");
                        Console.Write("Nové datum narození: ");
                    }

                    zam.NastavDatumNarozeni(noveDatum);
                }

                zam.NastavMzdu(Vstupy.ZeptejSeAUpravInt(zam.Mzda, "mzda"));

                Console.WriteLine("Úprava dokončena.");
            }
        }

        public void Vyhledat()
        {
            Console.Write("Zadejte hledané příjmení: ");
            string hledany = Console.ReadLine()!.Trim().ToLower();

            bool nalezeno = false;

            foreach (var zam in zoo.Zamestnanci)
            {
                if (zam.Prijmeni.Contains(hledany, StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine(
                        $"Nalezeno:\t{Vstupy.ToTitleCase(zam.Prijmeni)}" +
                        $"\t{Vstupy.ToTitleCase(zam.Jmeno)}" +
                        $"\t{zam.DatumNarozeni}" +
                        $"\t{zam.Mzda}" +
                        $"\t{Vstupy.ToTitleCase(zam.PracovniPozice)}"
                    );
                    nalezeno = true;
                }
            }

            if (!nalezeno)
                Console.WriteLine("Zaměstnanec nenalezen.");
        }

    }
}
