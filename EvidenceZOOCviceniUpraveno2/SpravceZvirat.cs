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
        /// <summary>
        /// Odkaz na hlavní datový objekt ZOO, který obsahuje seznam zvířat.
        /// </summary>
        private readonly ZOO zoo = zoo;

        /// <summary>
        /// Zobrazí hlavní menu pro práci se zvířaty a zpracovává volby uživatele.
        /// </summary>
        public void Menu()
        {
            zoo.ZajistiData("Zvířata");
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

                    default: Console.WriteLine("Neplatná volba."); 
                        break;
                }

            } while (volba != '6');
        }

        /// <summary>
        /// Přidá nové zvíře na základě vstupů od uživatele.
        /// Vstupy jsou validovány a zakázané znaky nejsou povoleny.
        /// </summary>
        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZVÍŘETE");
            string nazev = UpravaVstupu.ZeptejSeAUprav(
                "", "název",
                v => v,
                s => s,
                jeNove: true);

            int vek = UpravaVstupu.ZeptejSeAUprav(
                0, "věk",
                v => v.ToString(),
                s => int.Parse(s),
                jeNove: true);

            double vaha = UpravaVstupu.ZeptejSeAUprav(
                0.0, "váha",
                v => v.ToString(),
                s => double.Parse(s),
                jeNove: true);

            zoo.Zvirata.Add(new Zvire(nazev, vek, vaha));
            zoo.UlozZvirata();
            Console.WriteLine("Zvíře bylo úspěšně přidáno.");
        }


        /// <summary>
        /// Vypíše všechna zvířata uložená v systému.
        /// </summary>
        public void Vypis()
        {
            Console.WriteLine("VÝPIS ZVÍŘAT");
            Console.WriteLine();

            Console.WriteLine(
                $"{"Název",-20} {"Věk",-12} {"Váha",-10}"
            );

            Console.WriteLine(new string('-', 45));
            foreach (var zvire in zoo.Zvirata)
                zvire.VypisZvire();
        }

        /// <summary>
        /// Smaže zvíře vybrané uživatelem.
        /// </summary>
        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZVÍŘETE");

            var zvire = SelectHelp.VybratPolozku(zoo.Zvirata, z => z.Nazev, "zvířete");
            if (zvire != null)
            {
                Console.WriteLine($"Zvíře {zvire.Nazev} bylo smazáno.");
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

                zvire.Vek = UpravaVstupu.ZeptejSeAUprav(
                    zvire.Vek, "věk",
                    v => v.ToString(),
                    s => int.Parse(s));

                zvire.Vaha = UpravaVstupu.ZeptejSeAUprav(
                    zvire.Vaha, "váha",
                    v => v.ToString(),
                    s => double.Parse(s));

                zoo.UlozZvirata();
                Console.WriteLine("Úprava dokončena.");
            }
        }

        /// <summary>
        /// Vyhledá zvíře podle názvu. Podporuje částečnou shodu.
        /// </summary>
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
                Console.WriteLine("Zvíře nenalezeno.");
        }
    }
}