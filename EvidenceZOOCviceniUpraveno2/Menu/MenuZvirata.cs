using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.KonzoleVstupy;

namespace EvidenceZOOCviceniUpraveno2.Menu;

class MenuZvirata(ZvirataKonzole zvirataKonzole)
{
    private readonly ZvirataKonzole zvirataKonzole = zvirataKonzole;

    public void Zobraz()
    {
        char volba;
        do
        {
            VypisyDoKonzole.VypisHlavickuMenu("MENU ZVÍŘATA");
            VypisyDoKonzole.VypisTeloMenu(
                "1. Přidat zvíře",
                "2. Vypsat zvířata",
                "3. Smazat zvíře",
                "4. Upravit zvíře",
                "5. Vyhledat zvíře",
                "n. Návrat do hlavního menu"
            );

            VypisyDoKonzole.VypisVyzvuKZadani();

            volba = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            switch (volba)
            {
                case '1':
                    zvirataKonzole.Pridat();
                    break;
                case '2':
                    zvirataKonzole.Vypis();
                    break;
                case '3':
                    zvirataKonzole.Smazat();
                    break;
                case '4':
                    zvirataKonzole.Upravit();
                    break;
                case '5':
                    zvirataKonzole.Vyhledat();
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