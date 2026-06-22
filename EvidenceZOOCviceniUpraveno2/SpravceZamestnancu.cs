
namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Třída zodpovědná za správu zaměstnanců – přidávání, mazání, úpravy,
    /// výpisy a vyhledávání. Pracuje s daty uloženými v instanci <see cref="ZOO"/>.
    /// </summary>
    /// <param name="zoo">Instance třídy ZOO obsahující seznam zaměstnanců.</param>
    class SpravceZamestnancu(ZOO zoo)
    {
        /// <summary>
        /// Odkaz na hlavní datový objekt ZOO, který obsahuje seznam zaměstnanců.
        /// </summary>
        private readonly ZOO zoo = zoo;

        /// <summary>
        /// Zobrazí hlavní menu pro práci se zaměstnanci a zpracovává volby uživatele.
        /// </summary>
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
        /// Přidá nového zaměstnance na základě vstupů od uživatele.
        /// Vstupy jsou validovány a zakázané znaky nejsou povoleny.
        /// </summary>
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


        /// <summary>
        /// Vypíše všechny zaměstnance uložené v systému.
        /// </summary>
        public void Vypis()
        {
            Console.WriteLine("VÝPIS ZAMĚSTNANCŮ");
            foreach (var zam in zoo.Zamestnanci)
                zam.VypisZamestnance();
        }

        /// <summary>
        /// Smaže zaměstnance vybraného uživatelem.
        /// </summary>      
        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZAMĚSTNANCE");
            var vybranyZamestnanec = Vstupy.VybratPolozku(zoo.Zamestnanci, z => z.Prijmeni, "zaměstnance");
            int index = vybranyZamestnanec != null ? zoo.Zamestnanci.IndexOf(vybranyZamestnanec) : -1;
            if (index >= 0)
            {
                Console.WriteLine($"Zaměstnanec {zoo.Zamestnanci[index].Prijmeni} byl smazán.");
                zoo.Zamestnanci.RemoveAt(index);
                zoo.UlozZamestnance();
            }
        }

        /// <summary>
        /// Upraví údaje vybraného zaměstnance.
        /// </summary>
        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZAMĚSTNANCE");
            var vybranyZamestnanec = Vstupy.VybratPolozku(zoo.Zamestnanci, z => z.Prijmeni, "zaměstnance");
            int index = vybranyZamestnanec != null ? zoo.Zamestnanci.IndexOf(vybranyZamestnanec) : -1;
            if (index >= 0)
            {
                var zam = zoo.Zamestnanci[index];
                zam.NastavJmeno(Vstupy.NactiBezZakazanychZnaku($"Nové jméno ({zam.Jmeno}): ", '|'));
                zam.NastavPrijmeni(Vstupy.NactiBezZakazanychZnaku($"Nové příjmení ({zam.Prijmeni}): ", '|'));
                zam.NastavPracovniPozici(Vstupy.NactiBezZakazanychZnaku($"Nová pracovní pozice ({zam.PracovniPozice}): ", '|'));
                zam.NastavDatumNarozeni(Vstupy.ZeptejSeAUpravDateOnly(zam.DatumNarozeni, "datum narození", "Nové"));
                zam.NastavMzdu(Vstupy.ZeptejSeAUpravInt(zam.Mzda, "mzda", "Nová"));
                zoo.UlozZamestnance();
                Console.WriteLine("Úprava dokončena.");
            }
        }

        /// <summary>
        /// Vyhledá zaměstnance podle příjmení. Podporuje částečnou shodu.
        /// </summary>
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
