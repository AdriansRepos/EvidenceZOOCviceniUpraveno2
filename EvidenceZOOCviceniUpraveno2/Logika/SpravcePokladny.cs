using PohybHelper;
using EvidenceZOOCviceniUpraveno2.Enumy;
using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Vypisy.Pokladna;
using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    /// <summary>
    /// Třída SpravcePokladny zajišťuje kompletní logiku prodeje, storna a úpravy ceníku.
    /// Využívá primární konstruktor C# pro předání instance zOO.
    /// </summary>
    internal class SpravcePokladny(Zoo zoo)
    {
        // Uložení odkazu na hlavní objekt Zoo pro přístup k datům pokladny a logům
        private readonly Zoo zoo = zoo;

        /// <summary>
        /// Načte data pokladny ze souboru do paměti.
        /// </summary>
        public void NactiData()
        {
            zoo.Pokladna.Nacti();
        }

        /// <summary>
        /// Vrací aktuálně platný ceník vstupenek.
        /// </summary>
        public Cenik ZiskejCenik() => zoo.Pokladna.Cenik;

        /// <summary>
        /// Vrací kompletní seznam všech pokladních pohybů (prodejů i storn).
        /// </summary>
        public List<PokladniPohyb> ZiskejVsechnyPohyby() => zoo.Pokladna.PokladniPohyby;

        /// <summary>
        /// Vyfiltruje a vrátí pouze prodejní pohyby seřazené od nejnovějšího pro potřeby stornování.
        /// </summary>
        public List<PokladniPohyb> ZiskejProdejeProStorno()
        {
            return [.. zoo.Pokladna.PokladniPohyby
                .Where(p => p.TypPohybu == PokladniTypPohybu.Prodej)
                .OrderByDescending(p => p.DatumCas)];
        }

        /// <summary>
        /// Provede výpočet ceny, vytvoří pohyb a bezpečně jej uloží přes transakci.
        /// </summary>
        public void ProdatVstupenku(TypVstupenky typ, int pocetKusu, int pocetDospelych, int pocetDeti)
        {
            // Výpočet jednotkové ceny podle ceníku a věkového složení
            decimal jednotkovaCena = VypocetCenyVstupenky.Vypocitej(typ, zoo.Pokladna.Cenik, pocetDospelych, pocetDeti);
            
            // U jednorázových typů násobíme počtem kusů, u rodinných/skupinových platí částka za celek
            decimal celkovaCastka = typ is TypVstupenky.Detska or TypVstupenky.Dospela or TypVstupenky.ZTP or TypVstupenky.Duchodce
                ? jednotkovaCena * pocetKusu
                : jednotkovaCena;

            // Vytvoření nové položky pokladního pohybu
            var pohyb = new PokladniPohyb(
                PokladniTypPohybu.Prodej, typ, pocetKusu, pocetDospelych, pocetDeti,
                celkovaCastka, DateTime.Now);

            // Provedení zápisu do paměti a uložení na disk s automatickým rollbackem při chybě
            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Pokladna.PokladniPohyby.Add(pohyb),
                rollback: () => zoo.Pokladna.PokladniPohyby.Remove(pohyb),
                ulozeni: zoo.Pokladna.UlozPokladnu,
                popisOperace: "prodej vstupenky");

            if (uspech)
            {
                // Zápis záznamu o prodeji do auditního logu
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Pokladna", TypAkce.Pridano, pohyb.Popis(),
                    novaHodnota: $"{celkovaCastka:0.##} Kč"));

                VypisyDoKonzole.VypisUspech($"Prodáno. Celková částka: {celkovaCastka:0.##} Kč");                
            }
        }

        /// <summary>
        /// Provede storno vybraného pokladního pohybu.
        /// </summary>
        public void StornovatVstupenku(PokladniPohyb vybrany)
        {
            // Vytvoření storno pohybu kopírujícího parametry původního prodeje
            var storno = new PokladniPohyb(
                PokladniTypPohybu.Storno, vybrany.TypVstupenky,
                vybrany.PocetKusu, vybrany.PocetDospelych, vybrany.PocetDeti,
                vybrany.Castka, DateTime.Now);

            // Transakční uložení storno záznamu
            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Pokladna.PokladniPohyby.Add(storno),
                rollback: () => zoo.Pokladna.PokladniPohyby.Remove(storno),
                ulozeni: zoo.Pokladna.UlozPokladnu,
                popisOperace: "storno vstupenky");

            if (uspech)
            {
                // Auditování stornované transakce
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Pokladna", TypAkce.Pridano, $"Storno: {storno.Popis()}",
                    puvodniHodnota: $"{storno.Castka:0.##} Kč"));

                VypisyDoKonzole.VypisInformaci($"Stornováno: {storno.Popis()}");                
            }
        }

        /// <summary>
        /// Aktualizuje nastavení ceníku a zaznamená případné změny cen do auditu.
        /// </summary>
        public void UpravCenik(decimal detska, decimal dospela, decimal ztp, decimal duchodce, decimal slevaRodina, decimal slevaSkupina)
        {
            var cenik = zoo.Pokladna.Cenik;

            // Uchování původních cen pro vyhodnocení změn v logu
            decimal puvodniDetska = cenik.CenaDetska;
            decimal puvodniDospela = cenik.CenaDospela;
            decimal puvodniZTP = cenik.CenaZTP;
            decimal puvodniDuchodce = cenik.CenaDuchodce;
            decimal puvodniSlevaRodina = cenik.SlevaRodinaProcenta;
            decimal puvodniSlevaSkupina = cenik.SlevaSkupinaProcenta;

            // Nastavení nových hodnot
            cenik.CenaDetska = detska;
            cenik.CenaDospela = dospela;
            cenik.CenaZTP = ztp;
            cenik.CenaDuchodce = duchodce;
            cenik.SlevaRodinaProcenta = slevaRodina;
            cenik.SlevaSkupinaProcenta = slevaSkupina;

            // Uložení upraveného ceníku na disk
            zoo.Pokladna.UlozCenik();

            // Kontrola a případný zápis změněných položek do auditu
            ZapisZmenuCeniku("cena dětské", puvodniDetska, cenik.CenaDetska);
            ZapisZmenuCeniku("cena dospělé", puvodniDospela, cenik.CenaDospela);
            ZapisZmenuCeniku("cena ZTP", puvodniZTP, cenik.CenaZTP);
            ZapisZmenuCeniku("cena důchodce", puvodniDuchodce, cenik.CenaDuchodce);
            ZapisZmenuCeniku("sleva rodina %", puvodniSlevaRodina, cenik.SlevaRodinaProcenta);
            ZapisZmenuCeniku("sleva skupina %", puvodniSlevaSkupina, cenik.SlevaSkupinaProcenta);

            VypisyDoKonzole.VypisUspech("Ceník byl upraven.");            
        }

        /// <summary>
        /// Pomocná metoda pro porovnání původní a nové ceny a zapsání do auditu.
        /// </summary>
        private void ZapisZmenuCeniku(string popisPolozky, decimal puvodni, decimal nova)
        {
            if (puvodni != nova)
            {
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Pokladna – Ceník", TypAkce.Upraveno, popisPolozky,
                    puvodni.ToString("0.##"), nova.ToString("0.##")));
            }
        }

        /// <summary>
        /// Zobrazí dnešní pokladní uzávěrku na základě prodejů za aktuální kalendářní den.
        /// </summary>
        public void DenniUzaverka()
        {
            Console.WriteLine("DENNÍ UZÁVĚRKA");

            // Filtr transakcí pro dnešní datum
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

        /// <summary>
        /// Statická metoda pro filtraci a výpis pohybů za zvolené období.
        /// </summary>
        public static void ZobrazPohybyDlePeriody(List<PokladniPohyb> vysledek, PokladniTypPohybu? typFiltr)
        {
            // Aplikace volitelného filtru dle typu pohybu (Prodej/Storno)
            if (typFiltr != null)
                vysledek = [.. vysledek.Where(p => p.TypPohybu == typFiltr)];

            if (vysledek.Count == 0)
            {                
                VypisyDoKonzole.VypisInformaci("Žádné pohyby neodpovídají zadaným kritériím.");                
                return;
            }

            // Výpis agregovaného souhrnu
            PokladnaVypisy.VypisUzaverku(vysledek, "Souhrn za zvolené období");

            // Detailní řádkový výpis jednotlivých pohybů
            Console.WriteLine();
            VypisyDoKonzole.VypisZvyrazneni("PODROBNÝ VÝPIS:");
            foreach (var pohyb in vysledek.OrderByDescending(p => p.DatumCas))
            {
                PokladnaVypisy.VypisJedenPohyb(pohyb);
            }
        }
    }
}