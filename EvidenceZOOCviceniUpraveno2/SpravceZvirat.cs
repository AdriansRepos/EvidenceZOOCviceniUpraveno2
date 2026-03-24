using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceZOOCviceniUpraveno2
{
    class SpravceZvirat(ZOO zoo)
    {
        private readonly ZOO zoo = zoo;

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

                switch (volba)
                {
                    case '1': Pridat(); break;
                    case '2': Vypis(); break;
                    case '3': Smazat(); break;
                    case '4': Upravit(); break;
                    case '5': Vyhledat(); break;
                    case '6': Console.WriteLine(); break;
                    default: Console.WriteLine("Neplatná volba."); break;
                }

            } while (volba != '6');
        }

        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZVÍŘETE");

            string nazev = Vstupy.ZeptejSeAUpravString("", "název", "Nový", true);
            int vek = Vstupy.ZeptejSeAUpravInt(0, "věk", true);
            double vaha = Vstupy.ZeptejSeAUpravDouble(0, "váha", true);

            zoo.Zvirata.Add(new Zvire(nazev, vek, vaha));
            Console.WriteLine("Zvíře bylo úspěšně přidáno.");
        }

        public void Vypis()
        {
            Console.WriteLine("VÝPIS ZVÍŘAT");
            foreach (var zvire in zoo.Zvirata)
                zvire.VypisZvire();
        }

        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZVÍŘETE");
            int index = Vstupy.VybratIndexZvirete(zoo);
            if (index >= 0)
            {
                Console.WriteLine($"Zvíře {zoo.Zvirata[index].Nazev} bylo smazáno.");
                zoo.Zvirata.RemoveAt(index);
            }
        }

        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZVÍŘETE");
            int index = Vstupy.VybratIndexZvirete(zoo);

            if (index >= 0)
            {
                var zvire = zoo.Zvirata[index];

                zvire.Nazev = Vstupy.ZeptejSeAUpravString(zvire.Nazev, "název", "Nový");
                zvire.Vek = Vstupy.ZeptejSeAUpravInt(zvire.Vek, "věk");
                zvire.Vaha = Vstupy.ZeptejSeAUpravDouble(zvire.Vaha, "váha");

                Console.WriteLine("Úprava dokončena.");
            }
        }

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
