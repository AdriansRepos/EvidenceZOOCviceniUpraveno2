using EvidenceZOOCviceniUpraveno2;

string korenovaSlozka = ZOO.NactiNeboSeZeptejNaCesty();
ZOO zoo = new(korenovaSlozka);

// Zkontroluje a případně provede roční archivaci – musí proběhnout
// hned po vytvoření zoo, ještě před prvním použitím dat.
zoo.ZkontrolujRocniArchivaci();

zoo.RegistrujNacitani("Zaměstnanci", zoo.NactiZamestnance);
zoo.RegistrujNacitani("Zvířata", zoo.NactiZvirata);
zoo.RegistrujNacitani("Sklad", zoo.NactiSklad);

SpravceZamestnancu spravceZamestnancu = new(zoo);
SpravceZvirat spravceZvirat = new(zoo);
SpravceSkladu spravceSkladu = new(zoo);

char volbaMenu;
do
{
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("=== HLAVNÍ MENU ===");
    Console.WriteLine("\t1. Zvířata");
    Console.WriteLine("\t2. Zaměstnanci");
    Console.WriteLine("\t3. Sklad");
    Console.WriteLine("\t4. Statistiky");
    Console.WriteLine("\t5. Konec programu");
    Console.ResetColor();
    Console.Write("Vyber možnost: ");

    volbaMenu = Console.ReadKey().KeyChar;
    Console.WriteLine();

    switch (volbaMenu)
    {
        case '1': 
            spravceZvirat.Menu(); 
            break;

        case '2': 
            spravceZamestnancu.Menu(); 
            break;

        case '3': 
            spravceSkladu.Menu(); 
            break;

        case '4': 
            zoo.MenuStatistiky(); 
            break;

        case '5': 
            break;

        default:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Neplatná volba, opakujte zadání:");
            Console.ResetColor();
            break;
    }
}
while (volbaMenu != '5');