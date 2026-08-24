using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.Enumy;
using EvidenceZOOCviceniUpraveno2.Logika;
using PohybHelper;
using SelectHelper;
using TextHelper;

namespace EvidenceZOOCviceniUpraveno2.Menu
{
    internal class MenuPokladna(SpravcePokladny spravcePokladny)
    {
        private readonly SpravcePokladny spravcePokladny = spravcePokladny;

        public void Zobraz()
        {
            spravcePokladny.NactiData();
            char volba;
            do
            {
                VypisyDoKonzole.VypisHlavickuMenu("MENU POKLADNA");
                VypisyDoKonzole.VypisTeloMenu(
                    "1. Prodat vstupenku",
                    "2. Stornovat vstupenku",
                    "3. Upravit ceník",
                    "4. Denní uzávěrka",
                    "5. Vypsat pohyby (dle data)",
                    "n. Návrat do hlavního menu"
                );
                
                VypisyDoKonzole.VypisVyzvuKZadani();

                volba = char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();

                switch (volba)
                {
                    case '1':
                        ProdatVstupenku();
                        break;

                    case '2':
                        StornovatVstupenku();
                        break;

                    case '3':
                        UpravitCenik();
                        break;

                    case '4':
                        spravcePokladny.DenniUzaverka();
                        break;

                    case '5':
                        VypisPohybyDlePeriody();
                        break;

                    case 'n':
                        break;

                    default:                        
                        VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání: ");                        
                        break;
                }
            } while (volba != 'n');
        }

        private void ProdatVstupenku()
        {
            Console.WriteLine("PRODEJ VSTUPENKY");
            Console.WriteLine("Vyber kategorii vstupenky:");
            var typVstupenky = SelectHelp.VybratTypNeboVse<TypVstupenky>();

            if (typVstupenky == null)
            {                
                VypisyDoKonzole.VypisInformaci("Musíte vybrat konkrétní typ vstupenky.");                
                return;
            }

            TypVstupenky typ = typVstupenky.Value;
            int pocetKusu = 0;
            int pocetDospelych = 0;
            int pocetDeti = 0;

            switch (typ)
            {
                case TypVstupenky.Detska:
                case TypVstupenky.Dospela:
                case TypVstupenky.ZTP:
                case TypVstupenky.Duchodce:
                    pocetKusu = UpravaVstupu.ZeptejSeAUprav(
                        1, "počet kusů", v => v.ToString(), s => int.Parse(s), jeNove: true);
                    break;

                case TypVstupenky.RodinaUplna:
                case TypVstupenky.RodinaNeuplna:
                    pocetKusu = 1;
                    pocetDospelych = typ == TypVstupenky.RodinaUplna ? 2 : 1;
                    pocetDeti = UpravaVstupu.ZeptejSeAUprav(
                        0, "počet dětí", v => v.ToString(), s => int.Parse(s), jeNove: true);
                    break;

                case TypVstupenky.Skupina:
                    pocetKusu = 1;
                    pocetDospelych = UpravaVstupu.ZeptejSeAUprav(
                        0, "počet dospělých ve skupině", v => v.ToString(), s => int.Parse(s), jeNove: true);
                    pocetDeti = UpravaVstupu.ZeptejSeAUprav(
                        0, "počet dětí ve skupině", v => v.ToString(), s => int.Parse(s), jeNove: true);
                    break;
            }

            spravcePokladny.ProdatVstupenku(typ, pocetKusu, pocetDospelych, pocetDeti);
        }

        private void StornovatVstupenku()
        {
            Console.WriteLine("STORNO VSTUPENKY");

            var prodeje = spravcePokladny.ZiskejProdejeProStorno();
            if (prodeje.Count == 0)
            {                
                VypisyDoKonzole.VypisInformaci("Žádné prodeje ke stornování.");                
                return;
            }

            var vybrany = SelectHelp.VybratPolozku(prodeje, p => p.Popis(), "prodeje ke stornování");
            if (vybrany == null)
                return;

            spravcePokladny.StornovatVstupenku(vybrany);
        }

        private void UpravitCenik()
        {
            Console.WriteLine("ÚPRAVA CENÍKU");
            var cenik = spravcePokladny.ZiskejCenik();

            decimal novaDetska = UpravaVstupu.ZeptejSeAUprav(
                cenik.CenaDetska, "cena dětské vstupenky", v => v.ToString(), s => decimal.Parse(s));

            decimal novaDospela = UpravaVstupu.ZeptejSeAUprav(
                cenik.CenaDospela, "cena dospělé vstupenky", v => v.ToString(), s => decimal.Parse(s));

            decimal novaZTP = UpravaVstupu.ZeptejSeAUprav(
                cenik.CenaZTP, "cena ZTP vstupenky", v => v.ToString(), s => decimal.Parse(s));

            decimal novaDuchodce = UpravaVstupu.ZeptejSeAUprav(
                cenik.CenaDuchodce, "cena vstupenky pro důchodce", v => v.ToString(), s => decimal.Parse(s));

            decimal novaSlevaRodina = UpravaVstupu.ZeptejSeAUprav(
                cenik.SlevaRodinaProcenta, "sleva na rodinnou vstupenku (%)", v => v.ToString(), s => decimal.Parse(s));

            decimal novaSlevaSkupina = UpravaVstupu.ZeptejSeAUprav(
                cenik.SlevaSkupinaProcenta, "sleva na skupinovou vstupenku (%)", v => v.ToString(), s => decimal.Parse(s));

            spravcePokladny.UpravCenik(novaDetska, novaDospela, novaZTP, novaDuchodce, novaSlevaRodina, novaSlevaSkupina);
        }

        private void VypisPohybyDlePeriody()
        {
            Console.WriteLine("VÝPIS POKLADNÍCH POHYBŮ");

            var typFiltr = SelectHelp.VybratTypNeboVse<PokladniTypPohybu>();
            var vsechnyPohyby = spravcePokladny.ZiskejVsechnyPohyby();
            var vysledek = SelectHelp.VybratRozmeziData(vsechnyPohyby);

            SpravcePokladny.ZobrazPohybyDlePeriody(vysledek, typFiltr);
        }
    }
}