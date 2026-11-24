using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.KonzoleVstupy;
using EvidenceZOOCviceniUpraveno2.Logika;

namespace EvidenceZOOCviceniUpraveno2.Menu;

class MenuZvirata(SpravceZvirat spravceZvirat)
{
    private readonly SpravceZvirat spravceZvirat = spravceZvirat;

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
                    ZvirataKonzole.Pridat(spravceZvirat);
                    break;
                case '2':
                    ZvirataKonzole.Vypis(spravceZvirat);
                    break;
                case '3':
                    ZvirataKonzole.Smazat(spravceZvirat);
                    break;
                case '4':
                    ZvirataKonzole.Upravit(spravceZvirat);
                    break;
                case '5':
                    ZvirataKonzole.Vyhledat(spravceZvirat);
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