using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceZOOCviceniUpraveno2
{
    /* Třída zodpovědná za práci se zaměstnanci – přidávání, mazání, úpravy, výpisy a hledání.
     * Přijímá instanci ZOO, kde jsou uložena data. */
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
        // Přidání nového zaměstnance – dotazy na vstupy + validace
        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZAMĚSTNANCE");            
             /* Používáme NactiBezZakazanychZnaku, aby se do systému nikdy nedostal
             * zakázaný znak '|' (oddělovač v souboru). 
             * Tím zabráníme rozbití formátu při ukládání a načítání. */
            string jmeno = Vstupy.NactiBezZakazanychZnaku("Zadejte jméno: ", '|');
            string prijmeni = Vstupy.NactiBezZakazanychZnaku("Zadejte příjmení: ", '|');
            string pracovniPozice = Vstupy.NactiBezZakazanychZnaku("Zadejte pracovní pozici: ", '|');
            DateOnly datumNarozeni = Vstupy.ZeptejSeAUpravDateOnly(DateOnly.MinValue, "Datum narození", "Zadejte", true);
            int mzda = Vstupy.ZeptejSeAUpravInt(0, "mzda", "Nová", true);            

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
                
                 /* Opět používáme NactiBezZakazanychZnaku, aby uživatel nemohl zadat
                 * zakázaný znak '|' a nerozbil formát uložených dat.
                 */
                zam.NastavJmeno(
                    Vstupy.NactiBezZakazanychZnaku($"Nové jméno ({zam.Jmeno}): ", '|')
                );

                zam.NastavPrijmeni(
                    Vstupy.NactiBezZakazanychZnaku($"Nové příjmení ({zam.Prijmeni}): ", '|')
                );

                zam.NastavPracovniPozici(
                    Vstupy.NactiBezZakazanychZnaku($"Nová pracovní pozice ({zam.PracovniPozice}): ", '|')
                );
                zam.NastavDatumNarozeni(Vstupy.ZeptejSeAUpravDateOnly(zam.DatumNarozeni, "datum narození", "Nové"));
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
