using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceZOOCviceniUpraveno2
{
    /* Třída zodpovědná za práci se zvířaty – přidávání, mazání, úpravy, výpisy a hledání.
     * Přijímá instanci ZOO, kde jsou uložena data. */
    class SpravceZvirat(ZOO zoo)
    {
        // Odkaz na hlavní datový objekt ZOO
        private readonly ZOO zoo = zoo;

        // Hlavní menu pro práci se zvířaty
        public void Menu()
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
        
        // Přidání nového zvířete – dotazy na vstupy + validace
        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZVÍŘETE");

            /*
             * Používáme NactiBezZakazanychZnaku, aby se do systému nikdy nedostal
             * zakázaný znak '|' (oddělovač v souboru).
             * Tím zabráníme rozbití formátu při ukládání a načítání. */
            string nazev = Vstupy.NactiBezZakazanychZnaku("Zadejte název zvířete: ", '|');

            int vek = Vstupy.ZeptejSeAUpravInt(0, "věk", "Nový", true);
            double vaha = Vstupy.ZeptejSeAUpravDouble(0, "váha", true);

            // Uložení do seznamu
            zoo.Zvirata.Add(new Zvire(nazev, vek, vaha));
            zoo.UlozZvirata(); // okamžité uložení změn

            Console.WriteLine("Zvíře bylo úspěšně přidáno.");
        }


        // Vypíše všechna zvířata
        public void Vypis()
        {
            Console.WriteLine("VÝPIS ZVÍŘAT");
            foreach (var zvire in zoo.Zvirata)
                zvire.VypisZvire();
        }

        // Smazání zvířete podle výběru uživatele
        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZVÍŘETE");
            int index = Vstupy.VybratIndexZvirete(zoo);

            if (index >= 0)
            {
                Console.WriteLine($"Zvíře {zoo.Zvirata[index].Nazev} bylo smazáno.");
                zoo.Zvirata.RemoveAt(index);
                zoo.UlozZvirata(); // uloží změny
            }
        }

        // Úprava existujícího zvířete
        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZVÍŘETE");
            int index = Vstupy.VybratIndexZvirete(zoo);

            if (index >= 0)
            {
                var zvire = zoo.Zvirata[index];

                /*
                 * Opět používáme NactiBezZakazanychZnaku, aby uživatel nemohl zadat
                 * zakázaný znak '|' a nerozbil formát uložených dat.
                 */
                zvire.NastavNazev(
                    Vstupy.NactiBezZakazanychZnaku($"Nový název ({zvire.Nazev}): ", '|')
                );

                zvire.NastavVek(Vstupy.ZeptejSeAUpravInt(zvire.Vek, "věk", "Nový"));
                zvire.NastavVahu(Vstupy.ZeptejSeAUpravDouble(zvire.Vaha, "váha"));

                zoo.UlozZvirata(); // uloží změny

                Console.WriteLine("Úprava dokončena.");
            }
        }

        // Vyhledání zvířete podle názvu (částečná shoda)
        public void Vyhledat()
        {
            Console.Write("Zadejte hledaný výraz: ");
            string hledany = Console.ReadLine()!.Trim().ToLower();

            bool nalezeno = false;

            foreach (var zvire in zoo.Zvirata)
            {
                if (zvire.Nazev.Contains(hledany, StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine(
                        $"Nalezeno:\t{Vstupy.ToTitleCase(zvire.Nazev)}" +
                        $"\tVěk: {zvire.Vek}" +
                        $"\tVáha: {zvire.Vaha}"
                    );
                    nalezeno = true;
                }
            }

            if (!nalezeno)
                Console.WriteLine("Zvíře nenalezeno.");
        }
    }
}
