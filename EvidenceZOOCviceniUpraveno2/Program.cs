using EvidenceZOOCviceniUpraveno2;

// Cesty k datovým souborům (relativní vůči projektu)
string souborZam = @"..\..\..\zamestnanci.txt";
string souborZvir = @"..\..\..\zvirata.txt";
string logSoubor = @"..\..\..\chyby.log";

// Vytvoření hlavního objektu ZOO – načte data ze souborů do paměti
ZOO zoo = new(souborZam, souborZvir, logSoubor);

// Vytvoření správců, kteří pracují s daty v ZOO
SpravceZamestnancu spravceZamestnancu = new(zoo);
SpravceZvirat spravceZvirat = new(zoo);


// Hlavní menu aplikace – umožňuje přepínat mezi správou zvířat,
// správou zaměstnanců a statistikami.
char volbaMenu;
do
{
    Console.WriteLine("=== HLAVNÍ MENU ===");
    Console.WriteLine("\t1. Zvířata");
    Console.WriteLine("\t2. Zaměstnanci");
    Console.WriteLine("\t3. Statistiky");
    Console.WriteLine("\t4. Konec programu");
    Console.Write("Vyber možnost: ");

    // Načtení volby uživatele
    volbaMenu = Console.ReadKey().KeyChar;
    Console.WriteLine();

    // Zpracování volby
    switch (volbaMenu)
    {
        case '1':
            spravceZvirat.Menu();          // otevře menu pro práci se zvířaty
            break;

        case '2':
            spravceZamestnancu.Menu();     // otevře menu pro práci se zaměstnanci
            break;

        case '3':
            zoo.MenuStatistiky();          // zobrazí statistiky
            break;

        case '4':
            break;                          // ukončí program

        default:
            Console.WriteLine("Neplatná volba, opakujte zadání.");
            break;
    }
}
while (volbaMenu != '4');                    // opakuje menu, dokud uživatel nezvolí konec
