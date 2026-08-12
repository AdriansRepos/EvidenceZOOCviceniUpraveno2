using TextHelper;
using SelectHelper;
using PohybHelper;
using EvidenceZOOCviceniUpraveno2.Enumy;
using EvidenceZOOCviceniUpraveno2.Data;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    class SpravcePokladny(Zoo zoo)
    {
        private readonly Zoo zoo = zoo;

        public void Menu()
        {
            zoo.Pokladna.Nacti();
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
                        DenniUzaverka(); 
                        break;

                    case '5': 
                        VypisPohybyDlePeriody(); 
                        break;

                    case '6': 
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Neplatná volba, opakujte zadání:");
                        Console.ResetColor();
                        break;
                }
            } while (volba != '6');
        }

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

            decimal jednotkovaCena = VypocetCenyVstupenky.Vypocitej(typ, zoo.Pokladna.Cenik, pocetDospelych, pocetDeti);
            decimal celkovaCastka = typ is TypVstupenky.Detska or TypVstupenky.Dospela or TypVstupenky.ZTP or TypVstupenky.Duchodce
                ? jednotkovaCena * pocetKusu
                : jednotkovaCena;

            var pohyb = new PokladniPohyb(
                PokladniTypPohybu.Prodej, typ, pocetKusu, pocetDospelych, pocetDeti,
                celkovaCastka, DateTime.Now);

            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Pokladna.PokladniPohyby.Add(pohyb),
                rollback: () => zoo.Pokladna.PokladniPohyby.Remove(pohyb),
                ulozeni: zoo.Pokladna.UlozPokladnu,
                popisOperace: "prodej vstupenky");

            if (uspech)
            {
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Pokladna", TypAkce.Pridano, pohyb.Popis(),
                    novaHodnota: $"{celkovaCastka:0.##} Kč"));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Prodáno. Celková částka: {celkovaCastka:0.##} Kč");
                Console.ResetColor();
            }
        }

        public void StornovatVstupenku()
        {
            Console.WriteLine("STORNO VSTUPENKY");

            var prodeje = zoo.Pokladna.PokladniPohyby
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
            if (vybrany == null) 
                return;

            var storno = new PokladniPohyb(
                PokladniTypPohybu.Storno, vybrany.TypVstupenky,
                vybrany.PocetKusu, vybrany.PocetDospelych, vybrany.PocetDeti,
                vybrany.Castka, DateTime.Now);

            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Pokladna.PokladniPohyby.Add(storno),
                rollback: () => zoo.Pokladna.PokladniPohyby.Remove(storno),
                ulozeni: zoo.Pokladna.UlozPokladnu,
                popisOperace: "storno vstupenky");

            if (uspech)
            {
                // Storno je typický vektor podvodu, auditujeme vždy
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Pokladna", TypAkce.Pridano, $"Storno: {storno.Popis()}",
                    puvodniHodnota: $"{storno.Castka:0.##} Kč"));

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Stornováno: {storno.Popis()}");
                Console.ResetColor();
            }
        }

        public void UpravitCenik()
        {
            Console.WriteLine("ÚPRAVA CENÍKU");

            var cenik = zoo.Pokladna.Cenik;

            decimal puvodniDetska = cenik.CenaDetska;
            decimal puvodniDospela = cenik.CenaDospela;
            decimal puvodniZTP = cenik.CenaZTP;
            decimal puvodniDuchodce = cenik.CenaDuchodce;
            decimal puvodniSlevaRodina = cenik.SlevaRodinaProcenta;
            decimal puvodniSlevaSkupina = cenik.SlevaSkupinaProcenta;

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

            zoo.Pokladna.UlozCenik();

            ZapisZmenuCeniku("cena dětské", puvodniDetska, cenik.CenaDetska);
            ZapisZmenuCeniku("cena dospělé", puvodniDospela, cenik.CenaDospela);
            ZapisZmenuCeniku("cena ZTP", puvodniZTP, cenik.CenaZTP);
            ZapisZmenuCeniku("cena důchodce", puvodniDuchodce, cenik.CenaDuchodce);
            ZapisZmenuCeniku("sleva rodina %", puvodniSlevaRodina, cenik.SlevaRodinaProcenta);
            ZapisZmenuCeniku("sleva skupina %", puvodniSlevaSkupina, cenik.SlevaSkupinaProcenta);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ceník byl upraven.");
            Console.ResetColor();
        }

        private void ZapisZmenuCeniku(string popisPolozky, decimal puvodni, decimal nova)
        {
            if (puvodni != nova)
            {
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Pokladna – Ceník", TypAkce.Upraveno, popisPolozky,
                    puvodni.ToString("0.##"), nova.ToString("0.##")));
            }
        }

        public void DenniUzaverka()
        {
            Console.WriteLine("DENNÍ UZÁVĚRKA");

            var dnesniPohyby = zoo.Pokladna.PokladniPohyby
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

        private static void VypisUzaverku(List<PokladniPohyb> pohyby, string nadpis)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"=== {nadpis} ===");
            Console.ResetColor();

            var prodeje = pohyby.Where(p => p.TypPohybu == PokladniTypPohybu.Prodej).ToList();
            var storna = pohyby.Where(p => p.TypPohybu == PokladniTypPohybu.Storno).ToList();

            if (prodeje.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("PRODEJE PODLE KATEGORIÍ:");
                VypisRozpadPodleKategorie(prodeje);
            }

            if (storna.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("STORNA PODLE KATEGORIÍ:");
                VypisRozpadPodleKategorie(storna);
            }

            decimal celkovyPrijemZProdeju = prodeje.Sum(p => p.Castka);
            decimal celkoveStorno = storna.Sum(p => p.Castka);
            decimal cistyPrijem = celkovyPrijemZProdeju - celkoveStorno;

            Console.WriteLine();

            if (prodeje.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Celkový příjem z prodejů: {celkovyPrijemZProdeju:0.##} Kč");
                Console.ResetColor();
            }

            if (storna.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Celkové storno:           {celkoveStorno:0.##} Kč");
                Console.ResetColor();
            }

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

            var typFiltr = SelectHelp.VybratTypNeboVse<PokladniTypPohybu>();
            var vysledek = SelectHelp.VybratRozmeziData(zoo.Pokladna.PokladniPohyby);

            if (typFiltr != null)
                vysledek = [.. vysledek.Where(p => p.TypPohybu == typFiltr)];

            if (vysledek.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Žádné pohyby neodpovídají zadaným kritériím.");
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