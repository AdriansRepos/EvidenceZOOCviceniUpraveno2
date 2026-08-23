using TextHelper;
using SelectHelper;
using PohybHelper;
using EvidenceZOOCviceniUpraveno2.Enumy;
using EvidenceZOOCviceniUpraveno2.Entity;

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
                s => (KategoriePolozky)(int.Parse(s) - 1),
                jeNove: true);

            double mnozstvi = UpravaVstupu.ZeptejSeAUprav(
                0.0, "počáteční množství", v => v.ToString(), s => double.Parse(s), jeNove: true);

            string jednotka = UpravaVstupu.ZeptejSeAUprav(
                "", "jednotka (kg, l, ks...)", v => v, s => s, jeNove: true);

            double minimalniStav = UpravaVstupu.ZeptejSeAUprav(
                0.0, "minimální stav pro upozornění", v => v.ToString(), s => double.Parse(s), jeNove: true);

            var novaPolozka = new SkladovaPolozka(nazev, kategorie, mnozstvi, jednotka, minimalniStav);

            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Sklad.Sklad.Add(novaPolozka),
                rollback: () => zoo.Sklad.Sklad.Remove(novaPolozka),
                ulozeni: zoo.Sklad.UlozSklad,
                popisOperace: "přidání skladové položky");

            if (uspech)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Položka byla úspěšně přidána.");
                Console.ResetColor();
            }
        }

        public void Vypis()
        {
            Console.WriteLine("STAV SKLADU");
            Console.WriteLine();
            Console.WriteLine($"{"Název",-20} {"Kategorie",-25} {"Množství",8} {"Jednotka",-8}");
            Console.WriteLine(new string('-', 70));

            foreach (var polozka in zoo.Sklad.Sklad)
                polozka.VypisPolozku();
        }

        public void Naskladnit()
        {
            var polozka = SelectHelp.VybratPolozku(zoo.Sklad.Sklad, p => p.Nazev, "položky k naskladnění");
            if (polozka == null) 
                return;

            double pridat = UpravaVstupu.ZeptejSeAUprav(
                0.0, "množství k naskladnění", v => v.ToString(), s => double.Parse(s), jeNove: true);

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

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Naskladněno {pridat} {polozka.Jednotka} položky {polozka.Nazev}.");
                Console.ResetColor();
            }
        }

        public void Vyskladnit()
        {
            var polozka = SelectHelp.VybratPolozku(zoo.Sklad.Sklad, p => p.Nazev, "položky k vyskladnění");
            if (polozka == null) 
                return;

            double odebrat = UpravaVstupu.ZeptejSeAUprav(
                0.0, "množství k vyskladnění", v => v.ToString(), s => double.Parse(s), jeNove: true);

            if (odebrat > polozka.Mnozstvi)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nelze vyskladnit více, než je aktuálně na skladě.");
                Console.ResetColor();
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

            if (polozka.JeDochazejici)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Pozor: položka {polozka.Nazev} dosáhla minimálního stavu.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Vyskladněno {odebrat} {polozka.Jednotka} položky {polozka.Nazev}.");
                Console.ResetColor();
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

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Položka {polozka.Nazev} byla smazána.");
                Console.ResetColor();
            }
        }

        public void VypisDochazejici()
        {
            var dochazejici = zoo.Sklad.Sklad.Where(p => p.JeDochazejici).ToList();

            if (dochazejici.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Žádné položky momentálně nedochází.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("DOCHÁZEJÍCÍ POLOŽKY:");
            Console.ResetColor();

            foreach (var polozka in dochazejici)
                polozka.VypisPolozku();
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
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Žádné pohyby neodpovídají zadaným kritériím.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"{"Datum a čas",-20} {"Popis",-45}");
            Console.WriteLine(new string('-', 65));

            foreach (var pohyb in vysledek.OrderByDescending(p => p.DatumCas))
                pohyb.VypisPohyb();
        }
    }
}