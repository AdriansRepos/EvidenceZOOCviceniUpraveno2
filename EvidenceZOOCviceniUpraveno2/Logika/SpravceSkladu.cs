using TextHelper;
using SelectHelper;
using PohybHelper;
using EvidenceZOOCviceniUpraveno2.Enumy;
using EvidenceZOOCviceniUpraveno2.Entity;
using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    class SpravceSkladu
    {
        private readonly Zoo zoo;

        public SpravceSkladu(Zoo zoo)
        {
            this.zoo = zoo;
            this.zoo.Sklad.Nacti();
        }

        public void PridatPolozku()
        {
            Console.WriteLine("PŘIDÁNÍ NOVÉ SKLADOVÉ POLOŽKY");

            string nazev = UpravaVstupu.ZeptejSeAUprav("", "název položky", v => v, s => s, jeNove: true);

            Console.WriteLine("Kategorie: 1 = Krmivo, 2 = Pomůcky, 3 = Léky/veterinární materiál");
            KategoriePolozky kategorie = UpravaVstupu.ZeptejSeAUprav(
                KategoriePolozky.Krmivo, "kategorie",
                v => ((int)v + 1).ToString(),
                s => int.TryParse(s, out int parsed) ? (KategoriePolozky)(parsed - 1) : KategoriePolozky.Krmivo,
                jeNove: true);

            double mnozstvi = UpravaVstupu.ZeptejSeAUprav(
                0.0, "počáteční množství", v => v.ToString(), s => double.TryParse(s, out double d) ? d : 0.0, jeNove: true);

            string jednotka = UpravaVstupu.ZeptejSeAUprav(
                "", "jednotka (kg, l, ks...)", v => v, s => s, jeNove: true);

            double minimalniStav = UpravaVstupu.ZeptejSeAUprav(
                0.0, "minimální stav pro upozornění", v => v.ToString(), s => double.TryParse(s, out double d) ? d : 0.0, jeNove: true);

            var novaPolozka = new SkladovaPolozka(nazev, kategorie, mnozstvi, jednotka, minimalniStav);

            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Sklad.Sklad.Add(novaPolozka),
                rollback: () => zoo.Sklad.Sklad.Remove(novaPolozka),
                ulozeni: zoo.Sklad.UlozSklad,
                popisOperace: "přidání skladové položky");

            if (uspech)
            {
                // Přidán chybějící audit log
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Sklad", TypAkce.Pridano, novaPolozka.Nazev,
                    novaHodnota: $"{novaPolozka.Mnozstvi:0.##} {novaPolozka.Jednotka}"));

                VypisyDoKonzole.VypisUspech("Položka byla úspěšně přidána.");                
            }
        }

        public void Vypis()
        {
            Console.WriteLine("STAV SKLADU");            
            Console.WriteLine($"\n{"Název",-20} {"Kategorie",-25} {"Množství",8} {"Jednotka",-8}");
            Console.WriteLine(new string('-', 70));

            foreach (var polozka in zoo.Sklad.Sklad)
                Console.WriteLine(polozka.VypisPolozku());
        }

        public void Naskladnit()
        {
            var polozka = SelectHelp.VybratPolozku(zoo.Sklad.Sklad, p => p.Nazev, "položky k naskladnění");
            if (polozka == null) 
                return;

            double pridat = UpravaVstupu.ZeptejSeAUprav(
                0.0, "množství k naskladnění", v => v.ToString(), s => double.TryParse(s, out double d) ? d : 0.0, jeNove: true);

            if (pridat <= 0)
            {                
                VypisyDoKonzole.VypisVarovani("Množství k naskladnění musí být větší než 0.");                
                return;
            }

            double puvodniMnozstvi = polozka.Mnozstvi;
            var pohyb = new SkladovyPohyb(polozka.Nazev, SkladovyTypPohybu.Naskladneni, pridat, DateTime.Now);

            bool uspech = Transakce.ProvedSUlozenim(
                akce: () =>
                {
                    polozka.Mnozstvi += pridat;
                    zoo.Sklad.SkladovaHistorie.Add(pohyb);
                },
                rollback: () =>
                {
                    polozka.Mnozstvi = puvodniMnozstvi;
                    zoo.Sklad.SkladovaHistorie.Remove(pohyb);
                },
                ulozeni: zoo.Sklad.UlozSkladSHistorii,
                popisOperace: "naskladnění");

            if (uspech)
            {
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Sklad", TypAkce.Upraveno, $"{polozka.Nazev} – naskladnění",
                    puvodniMnozstvi.ToString("0.##"), polozka.Mnozstvi.ToString("0.##")));
                                
                VypisyDoKonzole.VypisUspech($"Naskladněno {pridat} {polozka.Jednotka} položky {polozka.Nazev}.");                
            }
        }

        public void Vyskladnit()
        {
            var polozka = SelectHelp.VybratPolozku(zoo.Sklad.Sklad, p => p.Nazev, "položky k vyskladnění");
            if (polozka == null) 
                return;

            double odebrat = UpravaVstupu.ZeptejSeAUprav(
                0.0, "množství k vyskladnění", v => v.ToString(), s => double.TryParse(s, out double d) ? d : 0.0, jeNove: true);

            if (odebrat <= 0)
            {                
                VypisyDoKonzole.VypisInformaci("Množství k vyskladnění musí být větší než 0.");                
                return;
            }

            if (odebrat > polozka.Mnozstvi)
            {                
                VypisyDoKonzole.VypisVarovani("Nelze vyskladnit více, než je aktuálně na skladě.");                
                return;
            }

            double puvodniMnozstvi = polozka.Mnozstvi;
            var pohyb = new SkladovyPohyb(polozka.Nazev, SkladovyTypPohybu.Vyskladneni, odebrat, DateTime.Now);

            bool uspech = Transakce.ProvedSUlozenim(
                akce: () =>
                {
                    polozka.Mnozstvi -= odebrat;
                    zoo.Sklad.SkladovaHistorie.Add(pohyb);
                },
                rollback: () =>
                {
                    polozka.Mnozstvi = puvodniMnozstvi;
                    zoo.Sklad.SkladovaHistorie.Remove(pohyb);
                },
                ulozeni: zoo.Sklad.UlozSkladSHistorii,
                popisOperace: "vyskladnění");

            if (!uspech) 
                return;

            zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                "Sklad", TypAkce.Upraveno, $"{polozka.Nazev} – vyskladnění",
                puvodniMnozstvi.ToString("0.##"), polozka.Mnozstvi.ToString("0.##")));

            // Informujeme vždy o úspěšném vyskladnění            
            VypisyDoKonzole.VypisUspech($"Vyskladněno {odebrat} {polozka.Jednotka} položky {polozka.Nazev}.");
            
            // A navíc zobrazíme varování, pokud klesla pod minimum
            if (polozka.JeDochazejici)
            {                
                VypisyDoKonzole.VypisInformaci($"Pozor: položka {polozka.Nazev} dosáhla minimálního stavu ({polozka.Mnozstvi:0.##} / min. {polozka.MinimalniStav:0.##}).");                
            }
        }

        public void Smazat()
        {
            var polozka = SelectHelp.VybratPolozku(zoo.Sklad.Sklad, p => p.Nazev, "položky ke smazání");
            if (polozka == null) 
                return;

            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Sklad.Sklad.Remove(polozka),
                rollback: () => zoo.Sklad.Sklad.Add(polozka),
                ulozeni: zoo.Sklad.UlozSklad,
                popisOperace: "smazání skladové položky");

            if (uspech)
            {
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Sklad", TypAkce.Smazano, polozka.Nazev,
                    puvodniHodnota: $"{polozka.Mnozstvi:0.##} {polozka.Jednotka}"));

                VypisyDoKonzole.VypisInformaci($"Položka {polozka.Nazev} byla smazána.");                
            }
        }

        public void VypisDochazejici()
        {
            var dochazejici = zoo.Sklad.Sklad.Where(p => p.JeDochazejici).ToList();

            if (dochazejici.Count == 0)
            {                
                VypisyDoKonzole.VypisUspech("Žádné položky momentálně nedochází.");                
                return;
            }
                        
            VypisyDoKonzole.VypisVarovani("DOCHÁZEJÍCÍ POLOŽKY:");
            
            foreach (var polozka in dochazejici)
                Console.WriteLine(polozka.VypisPolozku());
        }

        public void VypisPohybyInventura()
        {
            Console.WriteLine("VÝPIS SKLADOVÝCH POHYBŮ (inventura)");

            var typFiltr = SelectHelp.VybratTypNeboVse<SkladovyTypPohybu>();
            var vysledek = SelectHelp.VybratRozmeziData(zoo.Sklad.SkladovaHistorie);

            if (typFiltr != null)
                vysledek = [.. vysledek.Where(p => p.TypPohybu == typFiltr)];

            if (vysledek.Count == 0)
            {                
                VypisyDoKonzole.VypisInformaci("Žádné pohyby neodpovídají zadaným kritériím.");                
                return;
            }

            Console.WriteLine($"{"Datum a čas",-20} {"Popis",-45}");
            Console.WriteLine(new string('-', 65));

            foreach (var pohyb in vysledek.OrderByDescending(p => p.DatumCas))
                Console.WriteLine(pohyb.VypisPohyb());
        }
    }
}