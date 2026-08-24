using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.Logika;

namespace EvidenceZOOCviceniUpraveno2.Menu
{
    class MenuSklad(SpravceSkladu spravceSkladu)
    {
        private readonly SpravceSkladu spravceSkladu = spravceSkladu;

        public void Zobraz()
        {
            char volba;
            do
            {               
                VypisyDoKonzole.VypisHlavickuMenu("MENU SKLAD");
                VypisyDoKonzole.VypisTeloMenu(
                "\t1. Přidat novou položku",
                "\t2. Vypsat sklad",
                "\t3. Naskladnit (přidat množství)",
                "\t4. Vyskladnit (odebrat množství)",
                "\t5. Smazat položku",
                "\t6. Vypsat docházející položky",
                "\t7. Vypsat pohyby (inventura)",
                "\tn. Návrat do hlavního menu"
                );
                
                VypisyDoKonzole.VypisVyzvuKZadani();

                volba = char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();

                switch (volba)
                {
                    case '1': 
                        spravceSkladu.PridatPolozku(); 
                        break;
                    case '2': 
                        spravceSkladu.Vypis(); 
                        break;
                    case '3': 
                        spravceSkladu.Naskladnit(); 
                        break;
                    case '4': 
                        spravceSkladu.Vyskladnit(); 
                        break;
                    case '5': 
                        spravceSkladu.Smazat(); 
                        break;
                    case '6': 
                        spravceSkladu.VypisDochazejici(); 
                        break;
                    case '7': 
                        spravceSkladu.VypisPohybyInventura(); 
                        break;
                    case 'n': 
                        break;
                    default:                        
                        VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání: ");                        
                        break;
                }
            } while (volba != 'n');
        }
    }
}
