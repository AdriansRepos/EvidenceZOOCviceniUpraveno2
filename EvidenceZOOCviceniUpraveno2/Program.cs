using EvidenceZOOCviceniUpraveno2;

string korenovaSlozka = ZOO.NactiNeboSeZeptejNaCesty();
ZOO zoo = new(korenovaSlozka);
zoo.ZajistiData("Logy");

// Zkontroluje a případně provede roční archivaci – musí proběhnout
// hned po vytvoření zoo, ještě před prvním použitím dat.
zoo.ZkontrolujRocniArchivaci();

zoo.RegistrujNacitani("Zaměstnanci", zoo.NactiZamestnance);
zoo.RegistrujNacitani("Zvířata", zoo.NactiZvirata);
zoo.RegistrujNacitani("Sklad", zoo.NactiSklad);
zoo.RegistrujNacitani("Pokladna", zoo.NactiPokladnu);
zoo.RegistrujNacitani("Logy", zoo.NactiLogy);

SpravceZamestnancu spravceZamestnancu = new(zoo);
SpravceZvirat spravceZvirat = new(zoo);
SpravceSkladu spravceSkladu = new(zoo);
SpravcePokladny spravcePokladny = new(zoo);

char volbaMenu;
do
{
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("=== HLAVNÍ MENU ===");
    Console.WriteLine("\t1. Zvířata");
    Console.WriteLine("\t2. Zaměstnanci");
    Console.WriteLine("\t3. Sklad");
    Console.WriteLine("\t4. Správce pokladny");
    Console.WriteLine("\t5. Statistiky");
    Console.WriteLine("\t6. Konec");
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
            spravcePokladny.Menu();
            break;
        case '5':
            zoo.MenuStatistiky();
            break;
        case '6':
            break;
        default:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Neplatná volba, opakujte zadání:");
            Console.ResetColor();
            break;
    }
}
while (volbaMenu != '6');