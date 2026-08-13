using EvidenceZOOCviceniUpraveno2.Logika;
using EvidenceZOOCviceniUpraveno2.Data;
using EvidenceZOOCviceniUpraveno2.Zaloha;

string korenovaSlozka = KonfiguraceRepository.NactiNeboSeZeptejNaCesty();
Zoo zoo = new(korenovaSlozka);

// Zkontroluje a případně provede roční archivaci – musí proběhnout
// hned po vytvoření zoo, ještě před prvním použitím dat.
new ArchivacniSluzba(zoo).ZkontrolujRocniArchivaci();
zoo.Konfigurace.NactiExterniZalohuCestu();

zoo.Zamestnanci.Nacti();
zoo.Zvirata.Nacti();
zoo.Sklad.Nacti();
zoo.Pokladna.Nacti();
zoo.Logy.Nacti();

SpravceZamestnancu spravceZamestnancu = new(zoo);
SpravceZvirat spravceZvirat = new(zoo);
SpravceSkladu spravceSkladu = new(zoo);
SpravcePokladny spravcePokladny = new(zoo);
Statistiky statistiky = new(zoo);

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
    Console.WriteLine("\t6. Zálohovat teď");
    Console.WriteLine("\t7. Nastavit cestu k externí záloze");
    Console.WriteLine("\tk. Konec programu");
    Console.ResetColor();
    Console.Write("Vyber možnost: ");

    volbaMenu = char.ToLower(Console.ReadKey().KeyChar);
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
            statistiky.MenuStatistiky();
            break;

        case '6':
            new ZalohovaciSluzba(zoo).ProvedRucniZalohu();
            break;

        case '7':
            Console.Write("Zadej cestu k externí záloze (prázdné = vypnout): ");
            string cesta = Console.ReadLine()!.Trim();
            zoo.Konfigurace.NastavitExterniZalohuCestu(cesta);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Nastavení uloženo.");
            Console.ResetColor();
            break;

        case 'k':
            break;

        default:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Neplatná volba, opakujte zadání:");
            Console.ResetColor();
            break;
    }
}
while (volbaMenu != 'k');