using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Zaloha;

namespace EvidenceZOOCviceniUpraveno2.Menu
{
    internal class HlavniMenu(
        Zoo zoo,
        MenuZamestnanci menuZamestnanci,
        MenuZvirata menuZvirata,
        MenuSklad menuSklad,
        MenuPokladna menuPokladna,
        MenuStatistiky menuStatistiky)
    {
        private readonly Zoo zoo = zoo;
        private readonly MenuZamestnanci menuZamestnanci = menuZamestnanci;
        private readonly MenuZvirata menuZvirata = menuZvirata;
        private readonly MenuSklad menuSklad = menuSklad;
        private readonly MenuPokladna menuPokladna = menuPokladna;
        private readonly MenuStatistiky menuStatistiky = menuStatistiky;

        public void Zobraz()
        {
            char volbaMenu;
            do
            {
                VypisyDoKonzole.VypisHlavickuMenu("HLAVNÍ MENU");
                VypisyDoKonzole.VypisTeloMenu(
                "1. Zvířata",
                "2. Zaměstnanci",
                "3. Sklad",
                "4. Správce pokladny",
                "5. Statistiky",
                "6. Zálohovat teď",
                "7. Nastavit cestu k externí záloze",
                "k. Konec programu"
                );
                VypisyDoKonzole.VypisVyzvuKZadani();

                volbaMenu = char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();

                switch (volbaMenu)
                {
                    case '1':
                        menuZvirata.Zobraz();
                        break;

                    case '2':
                        menuZamestnanci.Zobraz();
                        break;

                    case '3':
                        menuSklad.Zobraz();
                        break;

                    case '4':
                        menuPokladna.Zobraz();
                        break;

                    case '5':
                        menuStatistiky.Zobraz();
                        break;

                    case '6':
                        new ZalohovaciSluzba(zoo).ProvedRucniZalohu();
                        break;

                    case '7':
                        Console.Write("Zadej cestu k externí záloze (prázdné = vypnout): ");
                        string cesta = Console.ReadLine()!.Trim();
                        zoo.Konfigurace.NastavitExterniZalohuCestu(cesta);
                        VypisyDoKonzole.VypisUspech("Nastavení uloženo.");
                        break;

                    case 'k':
                        break;

                    default:
                        VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");
                        break;
                }
            }
            while (volbaMenu != 'k');
        }
    }
}