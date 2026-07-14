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
        /// Přidá nového zaměstnance na základě vstupů od uživatele
        /// přidání zaměstnance přes transakci (rollback, pokud selže uložení).
        /// Vstupy jsou validovány.
        /// </summary>
        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZAMĚSTNANCE");
            string jmeno = UpravaVstupu.ZeptejSeAUprav("", "jméno", v => v, s => s, jeNove: true);
            string prijmeni = UpravaVstupu.ZeptejSeAUprav("", "příjmení", v => v, s => s, jeNove: true);
            string pracovniPozice = UpravaVstupu.ZeptejSeAUprav("", "pracovní pozice", v => v, s => s, jeNove: true);
        
            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                DateOnly.MinValue, "datum narození", v => v.ToString(), s => DateOnly.Parse(s), jeNove: true);
        
            int mzda = UpravaVstupu.ZeptejSeAUprav(
                0, "mzda", v => v.ToString(), s => int.Parse(s), jeNove: true);
        
            string mesto = UpravaVstupu.ZeptejSeAUprav("", "město", v => v, s => s, jeNove: true);
            string ulice = UpravaVstupu.ZeptejSeAUprav("", "ulice a číslo popisné", v => v, s => s, jeNove: true);
            string psc = UpravaVstupu.ZeptejSeAUprav("", "PSČ", v => v, s => s, jeNove: true);
            string telefon = UpravaVstupu.ZeptejSeAUprav("", "telefonní číslo", v => v, s => s, jeNove: true);
        
            string email = UpravaVstupu.ZeptejSeAUprav(
                "", "e-mail", v => v,
                s =>
                {
                    if (!Zamestnanec.JePlatnyEmail(s))
                        throw new FormatException("E-mail nemá platný formát (očekává se např. jmeno@domena.cz).");
                    return s;
                },
                jeNove: true);
        
            Zamestnanec novy = new(jmeno, prijmeni, datumNarozeni, mzda, pracovniPozice, mesto, ulice, psc, telefon, email);
        
            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Zamestnanci.Add(novy),
                rollback: () => zoo.Zamestnanci.Remove(novy),
                ulozeni: zoo.UlozZamestnance,
                popisOperace: "přidání zaměstnance");
        
            if (uspech)
            {
                zoo.ZapisAudit(AuditZaznam.Vytvor(
                    "Zaměstnanci", TypAkce.Pridano, $"{jmeno} {prijmeni}",
                    novaHodnota: $"mzda {mzda} Kč"));
        
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Zaměstnanec byl úspěšně přidán.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Vypíše všechny zaměstnance uložené v systému, včetně kontaktních
        /// a adresních údajů.
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
            {
                zam.VypisZamestnance();
                zam.VypisKontaktniUdaje();
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Smaže zaměstnance vybraného uživatelem – audit vždy (bez ohledu na to, co se mění)
        /// </summary>      
        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZAMĚSTNANCE");
            var zam = SelectHelp.VybratPolozku(zoo.Zamestnanci, z => z.Prijmeni, "zaměstnance");
            if (zam == null) return;
        
            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Zamestnanci.Remove(zam),
                rollback: () => zoo.Zamestnanci.Add(zam),
                ulozeni: zoo.UlozZamestnance,
                popisOperace: "smazání zaměstnance");
        
            if (uspech)
            {
                zoo.ZapisAudit(AuditZaznam.Vytvor(
                    "Zaměstnanci", TypAkce.Smazano, $"{zam.Jmeno} {zam.Prijmeni}",
                    puvodniHodnota: $"mzda {zam.Mzda} Kč"));
        
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Zaměstnanec {zam.Prijmeni} byl smazán.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Upraví údaje vybraného zaměstnance, včetně kontaktních
        /// a adresních údajů - audit jen na mzdu, transakce na celé uložení
        /// </summary>
        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZAMĚSTNANCE");
            var zam = SelectHelp.VybratPolozku(zoo.Zamestnanci, z => z.Prijmeni, "zaměstnance");
            if (zam == null) return;
        
            int puvodniMzda = zam.Mzda;
        
            zam.Jmeno = UpravaVstupu.ZeptejSeAUprav(zam.Jmeno, "jméno", v => v, s => s);
            zam.Prijmeni = UpravaVstupu.ZeptejSeAUprav(zam.Prijmeni, "příjmení", v => v, s => s);
            zam.PracovniPozice = UpravaVstupu.ZeptejSeAUprav(zam.PracovniPozice, "pracovní pozice", v => v, s => s);
        
            zam.DatumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                zam.DatumNarozeni, "datum narození", v => v.ToString(), s => DateOnly.Parse(s));
        
            zam.Mzda = UpravaVstupu.ZeptejSeAUprav(
                zam.Mzda, "mzda", v => v.ToString(), s => int.Parse(s));
        
            zam.Mesto = UpravaVstupu.ZeptejSeAUprav(zam.Mesto, "město", v => v, s => s);
            zam.Ulice = UpravaVstupu.ZeptejSeAUprav(zam.Ulice, "ulice a číslo popisné", v => v, s => s);
            zam.PSC = UpravaVstupu.ZeptejSeAUprav(zam.PSC, "PSČ", v => v, s => s);
            zam.Telefon = UpravaVstupu.ZeptejSeAUprav(zam.Telefon, "telefonní číslo", v => v, s => s);
        
            zam.Email = UpravaVstupu.ZeptejSeAUprav(
                zam.Email, "e-mail", v => v,
                s =>
                {
                    if (!Zamestnanec.JePlatnyEmail(s))
                        throw new FormatException("E-mail nemá platný formát (očekává se např. jmeno@domena.cz).");
                    return s;
                });
        
            zoo.UlozZamestnance();
        
            if (zam.Mzda != puvodniMzda)
            {
                zoo.ZapisAudit(AuditZaznam.Vytvor(
                    "Zaměstnanci", TypAkce.Upraveno, $"{zam.Jmeno} {zam.Prijmeni} – mzda",
                    puvodniMzda.ToString(), zam.Mzda.ToString()));
            }
        
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Úprava dokončena.");
            Console.ResetColor();
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