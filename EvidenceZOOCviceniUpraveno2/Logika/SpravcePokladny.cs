using PohybHelper;
using EvidenceZOOCviceniUpraveno2.Enumy;
using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Vypisy.Pokladna;
using BarevneVypisyHelper;

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
            return [.. zoo.Pokladna.PokladniPohyby
                .Where(p => p.TypPohybu == PokladniTypPohybu.Prodej)
                .OrderByDescending(p => p.DatumCas)];
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

                VypisyDoKonzole.VypisUspech($"Prodáno. Celková částka: {celkovaCastka:0.##} Kč");                
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

                VypisyDoKonzole.VypisInformaci($"Stornováno: {storno.Popis()}");                
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

            VypisyDoKonzole.VypisUspech("Ceník byl upraven.");            
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
                VypisyDoKonzole.VypisInformaci("Dnes zatím neproběhly žádné pokladní pohyby.");                
                return;
            }

            PokladnaVypisy.VypisUzaverku(dnesniPohyby, $"Uzávěrka za {DateTime.Today:d.M.yyyy}");
        }

        public static void ZobrazPohybyDlePeriody(List<PokladniPohyb> vysledek, PokladniTypPohybu? typFiltr)
        {
            if (typFiltr != null)
                vysledek = [.. vysledek.Where(p => p.TypPohybu == typFiltr)];

            if (vysledek.Count == 0)
            {                
                VypisyDoKonzole.VypisInformaci("Žádné pohyby neodpovídají zadaným kritériím.");                
                return;
            }

            PokladnaVypisy.VypisUzaverku(vysledek, "Souhrn za zvolené období");

            Console.WriteLine();
            VypisyDoKonzole.VypisZvyrazneni("PODROBNÝ VÝPIS:");
            foreach (var pohyb in vysledek.OrderByDescending(p => p.DatumCas))
            {
                PokladnaVypisy.VypisJedenPohyb(pohyb);
            }
        }
    }
}