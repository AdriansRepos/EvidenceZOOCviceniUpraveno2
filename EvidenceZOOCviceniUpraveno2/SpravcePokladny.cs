using TextHelper;
using SelectHelper;
using PohybHelper;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Třída zodpovědná za správu pokladny – prodej a storno vstupenek,
    /// úpravu ceníku a denní uzávěrky.
    /// </summary>
    /// <param name="zoo">Instance třídy ZOO obsahující ceník a pokladní pohyby.</param>
    class SpravcePokladny(ZOO zoo)
    {
        private readonly ZOO zoo = zoo;

        public void Menu()
        {
            zoo.ZajistiData("Pokladna");
            char volba;
            do
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n=== MENU POKLADNA ===");
                Console.WriteLine("\t1. Prodat vstupenku");
                Console.WriteLine("\t2. Stornovat vstupenku");
                Console.WriteLine("\t3. Upravit ceník");
                Console.WriteLine("\t4. Denní uzávěrka");
                Console.WriteLine("\t5. Vypsat pohyby (dle data)");
                Console.WriteLine("\t6. Návrat do hlavního menu");
                Console.ResetColor();
                Console.Write("Vyber možnost: ");

                volba = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (volba)
                {
                    case '1': ProdatVstupenku(); break;
                    case '2': StornovatVstupenku(); break;
                    case '3': UpravitCenik(); break;
                    case '4': DenniUzaverka(); break;
                    case '5': VypisPohybyDlePeriody(); break;
                    case '6': break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Neplatná volba, opakujte zadání:");
                        Console.ResetColor();
                        break;
                }
            } while (volba != '6');
        }

        // Konkrétní implementace jednotlivých metod (ProdatVstupenku,
        // StornovatVstupenku, UpravitCenik, DenniUzaverka, VypisPohybyDlePeriody)
        
        public void ProdatVstupenku()
        {
            Console.WriteLine("PRODEJ VSTUPENKY");
        
            Console.WriteLine("Vyber kategorii vstupenky:");
            var typVstupenky = SelectHelp.VybratTypNeboVse<TypVstupenky>();
        
            if (typVstupenky == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Musíš vybrat konkrétní typ vstupenky.");
                Console.ResetColor();
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
        
            decimal jednotkovaCena = VypocetCenyVstupenky.Vypocitej(typ, zoo.Cenik, pocetDospelych, pocetDeti);
            decimal celkovaCastka = typ is TypVstupenky.Detska or TypVstupenky.Dospela or TypVstupenky.ZTP or TypVstupenky.Duchodce
                ? jednotkovaCena * pocetKusu
                : jednotkovaCena;
        
            var pohyb = new PokladniPohyb(
                PokladniTypPohybu.Prodej, typ, pocetKusu, pocetDospelych, pocetDeti,
                celkovaCastka, DateTime.Now);
        
            zoo.PokladniPohyby.Add(pohyb);
            zoo.UlozPokladnu();
        
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Prodáno. Celková částka: {celkovaCastka:0.##} Kč");
            Console.ResetColor();
        }

        public void StornovatVstupenku()
        {
            Console.WriteLine("STORNO VSTUPENKY");
        
            var prodeje = zoo.PokladniPohyby
                .Where(p => p.TypPohybu == PokladniTypPohybu.Prodej)
                .OrderByDescending(p => p.DatumCas)
                .ToList();
        
            if (prodeje.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Žádné prodeje ke stornování.");
                Console.ResetColor();
                return;
            }
        
            var vybrany = SelectHelp.VybratPolozku(prodeje, p => p.Popis(), "prodeje ke stornování");
            if (vybrany == null) return;
        
            var storno = new PokladniPohyb(
                PokladniTypPohybu.Storno, vybrany.TypVstupenky,
                vybrany.PocetKusu, vybrany.PocetDospelych, vybrany.PocetDeti,
                vybrany.Castka, DateTime.Now);
        
            zoo.PokladniPohyby.Add(storno);
            zoo.UlozPokladnu();
        
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Stornováno: {storno.Popis()}");
            Console.ResetColor();
        }

        public void UpravitCenik()
        {
            Console.WriteLine("ÚPRAVA CENÍKU");
        
            var cenik = zoo.Cenik;
        
            cenik.CenaDetska = UpravaVstupu.ZeptejSeAUprav(
                cenik.CenaDetska, "cena dětské vstupenky", v => v.ToString(), s => decimal.Parse(s));
        
            cenik.CenaDospela = UpravaVstupu.ZeptejSeAUprav(
                cenik.CenaDospela, "cena dospělé vstupenky", v => v.ToString(), s => decimal.Parse(s));
        
            cenik.CenaZTP = UpravaVstupu.ZeptejSeAUprav(
                cenik.CenaZTP, "cena ZTP vstupenky", v => v.ToString(), s => decimal.Parse(s));
        
            cenik.CenaDuchodce = UpravaVstupu.ZeptejSeAUprav(
                cenik.CenaDuchodce, "cena vstupenky pro důchodce", v => v.ToString(), s => decimal.Parse(s));
        
            cenik.SlevaRodinaProcenta = UpravaVstupu.ZeptejSeAUprav(
                cenik.SlevaRodinaProcenta, "sleva na rodinnou vstupenku (%)", v => v.ToString(), s => decimal.Parse(s));
        
            cenik.SlevaSkupinaProcenta = UpravaVstupu.ZeptejSeAUprav(
                cenik.SlevaSkupinaProcenta, "sleva na skupinovou vstupenku (%)", v => v.ToString(), s => decimal.Parse(s));
        
            zoo.UlozCenik();
        
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ceník byl upraven.");
            Console.ResetColor();
        }

        public void DenniUzaverka()
        {
            Console.WriteLine("DENNÍ UZÁVĚRKA");
        
            var dnesniPohyby = zoo.PokladniPohyby
                .Where(p => p.DatumCas.Date == DateTime.Today)
                .ToList();
        
            if (dnesniPohyby.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Dnes zatím neproběhly žádné pokladní pohyby.");
                Console.ResetColor();
                return;
            }
        
            VypisUzaverku(dnesniPohyby, $"Uzávěrka za {DateTime.Today:d.M.yyyy}");
        }

        /// <summary>
        /// Vypíše souhrn pokladních pohybů – celkové počty a tržbu i rozpad
        /// podle jednotlivých kategorií vstupenek, zvlášť pro prodeje a storna.
        /// </summary>
        private static void VypisUzaverku(List<PokladniPohyb> pohyby, string nadpis)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"=== {nadpis} ===");
            Console.ResetColor();
        
            var prodeje = pohyby.Where(p => p.TypPohybu == PokladniTypPohybu.Prodej).ToList();
            var storna = pohyby.Where(p => p.TypPohybu == PokladniTypPohybu.Storno).ToList();
        
            Console.WriteLine();
            Console.WriteLine("PRODEJE PODLE KATEGORIÍ:");
            VypisRozpadPodleKategorie(prodeje);
        
            Console.WriteLine();
            Console.WriteLine("STORNA PODLE KATEGORIÍ:");
            VypisRozpadPodleKategorie(storna);
        
            decimal celkovyPrijemZProdeju = prodeje.Sum(p => p.Castka);
            decimal celkoveStorno = storna.Sum(p => p.Castka);
            decimal cistyPrijem = celkovyPrijemZProdeju - celkoveStorno;
        
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Celkový příjem z prodejů: {celkovyPrijemZProdeju:0.##} Kč");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Celkové storno:           {celkoveStorno:0.##} Kč");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Čistý příjem:             {cistyPrijem:0.##} Kč");
            Console.ResetColor();
        }
        
        private static void VypisRozpadPodleKategorie(List<PokladniPohyb> pohyby)
        {
            if (pohyby.Count == 0)
            {
                Console.WriteLine("  (žádné)");
                return;
            }
        
            var skupiny = pohyby.GroupBy(p => p.TypVstupenky);
        
            foreach (var skupina in skupiny)
            {
                string nazevKategorie = PopiskyHelper.ZiskejPopisek(skupina.Key);
                int pocetOsob = skupina.Sum(p =>
                    p.TypVstupenky is TypVstupenky.Detska or TypVstupenky.Dospela or TypVstupenky.ZTP or TypVstupenky.Duchodce
                        ? p.PocetKusu
                        : p.PocetDospelych + p.PocetDeti);
                decimal castka = skupina.Sum(p => p.Castka);
        
                Console.WriteLine($"  {nazevKategorie,-45} {pocetOsob,4} osob   {castka,10:0.##} Kč");
            }
        }

        public void VypisPohybyDlePeriody()
        {
            Console.WriteLine("VÝPIS POKLADNÍCH POHYBŮ");
        
            var vysledek = SelectHelp.VybratRozmeziData(zoo.PokladniPohyby);
        
            if (vysledek.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Žádné pohyby neodpovídají zadanému rozmezí.");
                Console.ResetColor();
                return;
            }
        
            VypisUzaverku(vysledek, "Souhrn za zvolené období");
        
            Console.WriteLine();
            Console.WriteLine("PODROBNÝ VÝPIS:");
            foreach (var pohyb in vysledek.OrderByDescending(p => p.DatumCas))
                pohyb.VypisPohyb();
        }
    }
}