using PohybHelper;
using EvidenceZOOCviceniUpraveno2.Enumy;
using EvidenceZOOCviceniUpraveno2.Entity;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    internal class SpravcePokladny(Zoo zoo)
    {
        private readonly Zoo zoo = zoo;

        public void NactiData()
        {
            zoo.Pokladna.Nacti();
        }

        public Cenik ZiskejCenik() => zoo.Pokladna.Cenik;

        public List<PokladniPohyb> ZiskejVsechnyPohyby() => zoo.Pokladna.PokladniPohyby;

        public List<PokladniPohyb> ZiskejProdejeProStorno()
        {
            return zoo.Pokladna.PokladniPohyby
                .Where(p => p.TypPohybu == PokladniTypPohybu.Prodej)
                .OrderByDescending(p => p.DatumCas)
                .ToList();
        }

        public void ProdatVstupenku(TypVstupenky typ, int pocetKusu, int pocetDospelych, int pocetDeti)
        {
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

        public void StornovatVstupenku(PokladniPohyb vybrany)
        {
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
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Pokladna", TypAkce.Pridano, $"Storno: {storno.Popis()}",
                    puvodniHodnota: $"{storno.Castka:0.##} Kč"));

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Stornováno: {storno.Popis()}");
                Console.ResetColor();
            }
        }

        public void UpravCenik(decimal detska, decimal dospela, decimal ztp, decimal duchodce, decimal slevaRodina, decimal slevaSkupina)
        {
            var cenik = zoo.Pokladna.Cenik;

            decimal puvodniDetska = cenik.CenaDetska;
            decimal puvodniDospela = cenik.CenaDospela;
            decimal puvodniZTP = cenik.CenaZTP;
            decimal puvodniDuchodce = cenik.CenaDuchodce;
            decimal puvodniSlevaRodina = cenik.SlevaRodinaProcenta;
            decimal puvodniSlevaSkupina = cenik.SlevaSkupinaProcenta;

            cenik.CenaDetska = detska;
            cenik.CenaDospela = dospela;
            cenik.CenaZTP = ztp;
            cenik.CenaDuchodce = duchodce;
            cenik.SlevaRodinaProcenta = slevaRodina;
            cenik.SlevaSkupinaProcenta = slevaSkupina;

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

        public void ZobrazPohybyDlePeriody(List<PokladniPohyb> vysledek, PokladniTypPohybu? typFiltr)
        {
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
    }
}