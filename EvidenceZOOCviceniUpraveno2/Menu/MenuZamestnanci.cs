using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.KonzoleVstupy;
using EvidenceZOOCviceniUpraveno2.Logika;

namespace EvidenceZOOCviceniUpraveno2.Menu;

class MenuZamestnanci(SpravceZamestnancu spravceZamestnancu)
{
    private readonly SpravceZamestnancu spravceZamestnancu = spravceZamestnancu;

    public void Zobraz()
    {
        char volba;
        do
        {
            VypisyDoKonzole.VypisHlavickuMenu("MENU ZAMĚSTNANCI");
            VypisyDoKonzole.VypisTeloMenu(
            "t1. Přidat zaměstnance",
            "t2. Vypsat zaměstnance",
            "t3. Smazat zaměstnance",
            "t4. Upravit zaměstnance",
            "t5. Vyhledat zaměstnance",
            "t6. Nastavení číslování zaměstnanců",
            "t7. Děti a partner",
            "tn. Návrat do hlavního menu"
            );

            VypisyDoKonzole.VypisVyzvuKZadani();

            volba = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            switch (volba)
            {
                case '1':
                    ZamestnanciKonzole.PridatZamestnance(spravceZamestnancu);
                    break;
                case '2':
                    ZamestnanciKonzole.VypisZamestnance(spravceZamestnancu.VratPrehled());
                    break;
                case '3':
                    ZamestnanciKonzole.SmazatZamestnance(spravceZamestnancu);
                    break;
                case '4':
                    ZamestnanciKonzole.UpravitZamestnance(spravceZamestnancu);
                    break;
                case '5':
                    ZamestnanciKonzole.VyhledatZamestnance(spravceZamestnancu);
                    break;
                case '6':
                    ZamestnanciKonzole.NastavitCislovaniZamestnancu(spravceZamestnancu);
                    break;
                case '7':
                    ZobrazPodmenuRodina();
                    break;
                case 'n':
                    break;
                default:
                    VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");
                    break;
            }

        } while (volba != 'n');
    }

    private void ZobrazPodmenuRodina()
    {
        char volba;
        do
        {
            VypisyDoKonzole.VypisHlavickuMenu("DĚTI A PARTNER");
            VypisyDoKonzole.VypisTeloMenu(
            "t1. Přidat dítě",
            "t2. Upravit dítě",
            "t3. Uložit / upravit partnera",
            "t4. Odebrat dítě",
            "t5. Odebrat partnera",
            "tn. Návrat zpět"
            );

            VypisyDoKonzole.VypisVyzvuKZadani();

            volba = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            switch (volba)
            {
                case '1':
                    ZamestnanciKonzole.PridatDiteZamestnanci(spravceZamestnancu);
                    break;
                case '2':
                    ZamestnanciKonzole.UpravitDiteZamestnanci(spravceZamestnancu);
                    break;
                case '3':
                    ZamestnanciKonzole.UlozitPartneraZamestnanci(spravceZamestnancu);
                    break;
                case '4':
                    ZamestnanciKonzole.OdebratDiteZamestnanci(spravceZamestnancu);
                    break;
                case '5':
                    ZamestnanciKonzole.OdebratPartneraZamestnanci(spravceZamestnancu);
                    break;
                case 'n':
                    break;
                default:
                    VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");
                    break;
            }

        } while (volba != 'n');
    }
}