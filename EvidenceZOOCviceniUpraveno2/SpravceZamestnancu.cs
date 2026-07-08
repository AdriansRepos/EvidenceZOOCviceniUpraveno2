using TextHelper;
using SelectHelper;

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
            zoo.ZajistiData("Zaměstnanci");
            char volba;
            do
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n=== MENU ZAMĚSTNANCI ===");
                Console.WriteLine("\t1. Přidat zaměstnance");
                Console.WriteLine("\t2. Vypsat zaměstnance");
                Console.WriteLine("\t3. Smazat zaměstnance");
                Console.WriteLine("\t4. Upravit zaměstnance");
                Console.WriteLine("\t5. Vyhledat zaměstnance");
                Console.WriteLine("\t6. Návrat do hlavního menu");
                Console.ResetColor();
                Console.Write("Vyber možnost: ");

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

                    default: 
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Neplatná volba, opakujte zadání:"); 
                        Console.ResetColor();
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
            string jmeno = UpravaVstupu.ZeptejSeAUprav(
                "", "jméno",
                v => v,
                s => s,
                jeNove: true);

            string prijmeni = UpravaVstupu.ZeptejSeAUprav(
                "", "příjmení",
                v => v,
                s => s,
                jeNove: true);

            string pracovniPozice = UpravaVstupu.ZeptejSeAUprav(
                "", "pracovní pozice",
                v => v,
                s => s,
                jeNove: true);

            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                DateOnly.MinValue, "datum narození",
                v => v.ToString(),
                s => DateOnly.Parse(s),
                jeNove: true);

            int mzda = UpravaVstupu.ZeptejSeAUprav(
                0, "mzda",
                v => v.ToString(),
                s => int.Parse(s),
                jeNove: true);

            zoo.Zamestnanci.Add(new Zamestnanec(jmeno, prijmeni, datumNarozeni, mzda, pracovniPozice));
            zoo.UlozZamestnance();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Zaměstnanec byl úspěšně přidán.");
            Console.ResetColor();
        }

        /// <summary>
        /// Vypíše všechny zaměstnance uložené v systému.
        /// </summary>
        public void Vypis()
        {
            Console.WriteLine("VÝPIS ZAMĚSTNANCŮ");
            Console.WriteLine();

            Console.WriteLine(
                $"{"Jméno",-15} {"Příjmení",-15} {"Datum narození",-15} {"Mzda",-10} {"Pozice",-20}"
            );

            Console.WriteLine(new string('-', 80));

            foreach (var zam in zoo.Zamestnanci)
                zam.VypisZamestnance();
        }

        /// <summary>
        /// Smaže zaměstnance vybraného uživatelem.
        /// </summary>      
        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZAMĚSTNANCE");
            var zam = SelectHelp.VybratPolozku(zoo.Zamestnanci, z => z.Prijmeni, "zaměstnance");
            if (zam != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Zaměstnanec {zam.Prijmeni} byl smazán.");
                Console.ResetColor();
                zoo.Zamestnanci.Remove(zam);
                zoo.UlozZamestnance();
            }
        }

        /// <summary>
        /// Upraví údaje vybraného zaměstnance.
        /// </summary>
        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZAMĚSTNANCE");
            var zam = SelectHelp.VybratPolozku(zoo.Zamestnanci, z => z.Prijmeni, "zaměstnance");
            if (zam != null)
            {
                zam.Jmeno = UpravaVstupu.ZeptejSeAUprav(
                    zam.Jmeno, "jméno",
                    v => v,
                    s => s);

                zam.Prijmeni = UpravaVstupu.ZeptejSeAUprav(
                    zam.Prijmeni, "příjmení",
                    v => v,
                    s => s);

                zam.PracovniPozice = UpravaVstupu.ZeptejSeAUprav(
                    zam.PracovniPozice, "pracovní pozice",
                    v => v,
                    s => s);

                zam.DatumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                    zam.DatumNarozeni, "datum narození",
                    v => v.ToString(),
                    s => DateOnly.Parse(s));

                zam.Mzda = UpravaVstupu.ZeptejSeAUprav(
                    zam.Mzda, "mzda",
                    v => v.ToString(),
                    s => int.Parse(s));

                zoo.UlozZamestnance();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Úprava dokončena.");
                Console.ResetColor();
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
                        $"Nalezeno:\t{zam.Prijmeni}" +
                        $"\t{zam.Jmeno}" +
                        $"\t{zam.DatumNarozeni}" +
                        $"\t{zam.Mzda}" +
                        $"\t{zam.PracovniPozice}"
                    );
                    nalezeno = true;
                }
            }
            if (!nalezeno)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Zaměstnanec nenalezen.");
                Console.ResetColor();
            }
        }
    }
}