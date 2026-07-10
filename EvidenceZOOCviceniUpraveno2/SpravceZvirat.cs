using TextHelper;
using SelectHelper;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Třída zodpovědná za správu zvířat – přidávání, mazání, úpravy,
    /// výpisy a vyhledávání. Pracuje s daty uloženými v instanci <see cref="ZOO"/>.
    /// </summary>
    /// <param name="zoo">Instance třídy ZOO obsahující seznam zvířat.</param>
    class SpravceZvirat(ZOO zoo)
    {
        private readonly ZOO zoo = zoo;

        public void Menu()
        {
            zoo.ZajistiData("Zvířata");
            char volba;
            do
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n=== MENU ZVÍŘATA ===");
                Console.WriteLine("\t1. Přidat zvíře");
                Console.WriteLine("\t2. Vypsat zvířata");
                Console.WriteLine("\t3. Smazat zvíře");
                Console.WriteLine("\t4. Upravit zvíře");
                Console.WriteLine("\t5. Vyhledat zvíře");
                Console.WriteLine("\t6. Návrat do hlavního menu");
                Console.ResetColor();
                Console.Write("Vyber možnost: ");

                volba = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (volba)
                {
                    case '1': Pridat();
                        break;

                    case '2': Vypis();
                        break;

                    case '3': Smazat();
                        break;

                    case '4': Upravit();
                        break;

                    case '5': Vyhledat();
                        break;

                    case '6':
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Neplatná volba, opakujte zadání:");
                        Console.ResetColor();
                        break;
                }

            } while (volba != '6');
        }

        /// <summary>
        /// Přidá nové zvíře na základě vstupů od uživatele.
        /// Vstupy jsou validovány.
        /// </summary>
        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZVÍŘETE");
            string nazev = UpravaVstupu.ZeptejSeAUprav(
                "", "název",
                v => v,
                s => s,
                jeNove: true);

            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                DateOnly.MinValue, "datum narození",
                v => v.ToString(),
                s => DateOnly.Parse(s),
                jeNove: true);

            double vaha = UpravaVstupu.ZeptejSeAUprav(
                0.0, "váha",
                v => v.ToString(),
                s => double.Parse(s),
                jeNove: true);

            zoo.Zvirata.Add(new Zvire(nazev, datumNarozeni, vaha));
            zoo.UlozZvirata();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Zvíře bylo úspěšně přidáno.");
            Console.ResetColor();
        }

        public void Vypis()
        {
            Console.WriteLine("VÝPIS ZVÍŘAT");
            Console.WriteLine();

            Console.WriteLine(
                $"{"Název",-20} {"Věk",-15} {"Váha",-10}"
            );

            Console.WriteLine(new string('-', 45));
            foreach (var zvire in zoo.Zvirata)
                zvire.VypisZvire();
        }

        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZVÍŘETE");

            var zvire = SelectHelp.VybratPolozku(zoo.Zvirata, z => z.Nazev, "zvířete");
            if (zvire != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Zvíře {zvire.Nazev} bylo smazáno.");
                Console.ResetColor();
                zoo.Zvirata.Remove(zvire);
                zoo.UlozZvirata();
            }
        }

        /// <summary>
        /// Upraví údaje vybraného zvířete.
        /// </summary>
        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZVÍŘETE");
            var zvire = SelectHelp.VybratPolozku(zoo.Zvirata, z => z.Nazev, "zvířete");
            if (zvire != null)
            {
                zvire.Nazev = UpravaVstupu.ZeptejSeAUprav(
                    zvire.Nazev, "název",
                    v => v,
                    s => s);

                zvire.DatumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                    zvire.DatumNarozeni, "datum narození",
                    v => v.ToString(),
                    s => DateOnly.Parse(s));

                zvire.Vaha = UpravaVstupu.ZeptejSeAUprav(
                    zvire.Vaha, "váha",
                    v => v.ToString(),
                    s => double.Parse(s));

                zoo.UlozZvirata();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Úprava dokončena.");
                Console.ResetColor();
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
                        $"Nalezeno:\t{zvire.Nazev}" +
                        $"\tVěk: {zvire.Vek}" +
                        $"\tVáha: {zvire.Vaha}"
                    );
                    nalezeno = true;
                }
            }

            if (!nalezeno)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Zvíře nenalezeno.");
                Console.ResetColor();
            }
        }
    }
}