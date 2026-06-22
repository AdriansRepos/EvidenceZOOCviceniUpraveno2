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


        /// <summary>
        /// Vypíše všechna zvířata uložená v systému.
        /// </summary>
        public void Vypis()
        {
            Console.WriteLine("VÝPIS ZVÍŘAT");
            foreach (var zvire in zoo.Zvirata)
                zvire.VypisZvire();
        }

        /// <summary>
        /// Smaže zvíře vybrané uživatelem.
        /// </summary>
        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZVÍŘETE");

            var zvire = Vstupy.VybratPolozku(zoo.Zvirata, z => z.Nazev, "zvířete");
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

            var zvire = Vstupy.VybratPolozku(zoo.Zvirata, z => z.Nazev, "zvířete");
            if (zvire != null)
            {
                zvire.NastavNazev(
                    Vstupy.NactiBezZakazanychZnaku($"Nový název ({zvire.Nazev}): ", '|')
                );
                zvire.NastavVek(Vstupy.ZeptejSeAUpravInt(zvire.Vek, "věk", "Nový"));
                zvire.NastavVahu(Vstupy.ZeptejSeAUpravDouble(zvire.Vaha, "váha"));
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
