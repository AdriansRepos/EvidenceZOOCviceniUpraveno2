using EvidenceZOOCviceniUpraveno2;


// vytvoření instance ZOO
ZOO zoo = new();
SpravceZvirat spravceZvirat = new(zoo);
SpravceZamestnancu spravceZamestnancu = new(zoo);


char volbaMenu;
do
{
    Console.WriteLine("=== HLAVNÍ MENU ===");
    Console.WriteLine("\t1. Zvířata");
    Console.WriteLine("\t2. Zaměstnanci");
    Console.WriteLine("\t3. Statistiky");
    Console.WriteLine("\t4. Konec programu");
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
            zoo.MenuStatistiky();
            break;

        case '4':
            break;

        default:
            Console.WriteLine("Neplatná volba, opakujte zadání.");
            break;
    }

} 
while (volbaMenu != '4');
