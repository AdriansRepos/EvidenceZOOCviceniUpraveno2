using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceZOOCviceniUpraveno2
{
    // Třída zodpovědná za práci se zaměstnanci – přidávání, mazání, úpravy, výpisy a hledání.
    // Přijímá instanci ZOO, kde jsou uložena data.
    class SpravceZamestnancu(ZOO zoo)
    {
        // Odkaz na hlavní datový objekt ZOO
        private readonly ZOO zoo = zoo;

        // Hlavní menu pro práci se zaměstnanci
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

                // Zpracování volby uživatele
                switch (volba)
                {
                    case '1': Pridat(); break;
                    case '2': Vypis(); break;
                    case '3': Smazat(); break;
                    case '4': Upravit(); break;
                    case '5': Vyhledat(); break;
                    case '6': break;
                    default: Console.WriteLine("Neplatná volba."); break;
                }

            } while (volba != '6');
        }

        // Přidání nového zaměstnance – dotazy na vstupy + validace
        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZAMĚSTNANCE");

            // Získání vstupů od uživatele
            string jmeno = Vstupy.ZeptejSeAUpravString("", "jméno", "Nové", true);
            string prijmeni = Vstupy.ZeptejSeAUpravString("", "příjmení", "Nové", true);
            string pracovniPozice = Vstupy.ZeptejSeAUpravString("", "pracovní pozice", "Nová", true);
            int mzda = Vstupy.ZeptejSeAUpravInt(0, "mzda", "Nová", true);

            // Zadání data narození s validací
            DateOnly datumNarozeni;
            while (true)
            {
                Console.Write("Datum narození (formát d.M.rrrr): ");
                string vstup = Console.ReadLine()!;

                if (DateOnly.TryParse(vstup, out datumNarozeni))
                    break;

                Console.WriteLine("Neplatný formát data. Zkus to znovu.");
            }

            // Uložení do seznamu
            zoo.Zamestnanci.Add(new Zamestnanec(jmeno, prijmeni, datumNarozeni, mzda, pracovniPozice));
            zoo.UlozZamestnance(); // okamžité uložení změn

            Console.WriteLine("Zaměstnanec byl úspěšně přidán.");
        }

        // Vypíše všechny zaměstnance
        public void Vypis()
        {
            Console.WriteLine("VÝPIS ZAMĚSTNANCŮ");
            foreach (var zam in zoo.Zamestnanci)
                zam.VypisZamestnance();
        }

        // Smazání zaměstnance podle výběru uživatele
        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZAMĚSTNANCE");
            int index = Vstupy.VybratIndexZamestnance(zoo);

            if (index >= 0)
            {
                Console.WriteLine($"Zaměstnanec {zoo.Zamestnanci[index].Prijmeni} byl smazán.");
                zoo.Zamestnanci.RemoveAt(index);
                zoo.UlozZamestnance(); // uloží změny
            }
        }

        // Úprava existujícího zaměstnance
        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZAMĚSTNANCE");
            int index = Vstupy.VybratIndexZamestnance(zoo);

            if (index >= 0)
            {
                var zam = zoo.Zamestnanci[index];

                // Úprava jednotlivých položek
                zam.NastavJmeno(Vstupy.ZeptejSeAUpravString(zam.Jmeno, "jméno", "Nové"));
                zam.NastavPrijmeni(Vstupy.ZeptejSeAUpravString(zam.Prijmeni, "příjmení", "Nové"));
                zam.NastavPracovniPozici(Vstupy.ZeptejSeAUpravString(zam.PracovniPozice, "pracovní pozice", "Nová"));

                // Úprava data narození – volitelná
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

                // Úprava mzdy
                zam.NastavMzdu(Vstupy.ZeptejSeAUpravInt(zam.Mzda, "mzda", "Nová"));

                zoo.UlozZamestnance(); // uloží změny

                Console.WriteLine("Úprava dokončena.");
            }
        }

        // Vyhledání zaměstnance podle příjmení (částečná shoda)
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
