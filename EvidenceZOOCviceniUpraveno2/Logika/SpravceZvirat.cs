using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Vypisy.Zvirata;
using SelectHelper;
using TextHelper;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    /// <summary>
    /// Třída zodpovědná za správu zvířat – přidávání, mazání, úpravy,
    /// výpisy a vyhledávání. Pracuje s daty uloženými v instanci <see cref="Zoo"/>.
    /// </summary>
    class SpravceZvirat
    {
        private readonly Zoo zoo;

        public SpravceZvirat(Zoo zoo)
        {
            this.zoo = zoo;
            this.zoo.Zvirata.Nacti(); // Načtení evidence zvířat
        }

        /// <summary>
        /// Zadá nové zvíře, vytvoří pro něj ID a uloží samostatný soubor zvířete.
        /// </summary>
        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZVÍŘETE");

            // Vygenerování nového ID zvířete podle konfigurace
            string id = zoo.Zvirata.CisloKonfigurace.DalsiId();
            zoo.Zvirata.UlozCisloKonfiguraci();

            string nazev = UpravaVstupu.ZeptejSeAUprav(
                "", "název", v => v, s => s, jeNove: true);

            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                DateOnly.MinValue, "datum narození",
                v => v.ToString(), s => DateOnly.Parse(s), jeNove: true);

            double vaha = UpravaVstupu.ZeptejSeAUprav(
                0.0, "váha", v => v.ToString(), s => double.Parse(s), jeNove: true);

            Console.Write("Bylo zvíře přijato z jiné zoo? (Enter = ne, narozeno zde) Datum přijetí: ");
            string vstupDatumPrijeti = Console.ReadLine()!.Trim();
            DateOnly? datumPrijetiZJineZoo = string.IsNullOrWhiteSpace(vstupDatumPrijeti)
                ? null
                : DateOnly.Parse(vstupDatumPrijeti);

            List<ZdravotniZaznam> zdravotniZaznamy = [];

            Zvire nove = new(nazev, datumNarozeni, vaha, datumPrijetiZJineZoo, zdravotniZaznamy, id);

            // Uložení objektu v paměti a vytvoření dedikovaného souboru
            zoo.Zvirata.Zvirata.Add(nove);
            zoo.Zvirata.UlozZvire(nove);

            VypisyDoKonzole.VypisUspech("Zvíře bylo úspěšně přidáno.");
        }

        /// <summary>
        /// Zobrazí tabulkový výpis všech zvířat.
        /// </summary>
        public void Vypis()
        {
            ZvirataVypisy.VypisHlavicku();

            foreach (var zvire in zoo.Zvirata.Zvirata)
                ZvirataVypisy.VypisZvire(zvire);
        }

        /// <summary>
        /// Odstraní zvíře z evidence a smaže jeho soubor na disku.
        /// </summary>
        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZVÍŘETE");

            var zvire = SelectHelp.VybratPolozku(zoo.Zvirata.Zvirata, z => z.Nazev, "zvířete");
            if (zvire != null)
            {
                zoo.Zvirata.Zvirata.Remove(zvire);
                zoo.Zvirata.SmazatZvireSoubor(zvire);

                VypisyDoKonzole.VypisInformaci($"Zvíře {zvire.Nazev} bylo smazáno.");
            }
        }

        /// <summary>
        /// Umožňuje úpravu základních údajů o zvířeti (název, narození, váha).
        /// </summary>
        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZVÍŘETE");
            var zvire = SelectHelp.VybratPolozku(zoo.Zvirata.Zvirata, z => z.Nazev, "zvířete");
            if (zvire != null)
            {
                // Uložení starého názvu souboru pro případ, že dojde k přejmenování zvířete
                string puvodniSoubor = zoo.Zvirata.SouborZvirete(zvire);

                zvire.Nazev = UpravaVstupu.ZeptejSeAUprav(
                    zvire.Nazev, "název", v => v, s => s);

                zvire.DatumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                    zvire.DatumNarozeni, "datum narození",
                    v => v.ToString(), s => DateOnly.Parse(s));

                zvire.Vaha = UpravaVstupu.ZeptejSeAUprav(
                    zvire.Vaha, "váha", v => v.ToString(), s => double.Parse(s));

                // Aktualizace souboru s případným přejmenováním
                zoo.Zvirata.UlozZvire(zvire, puvodniSoubor);

                VypisyDoKonzole.VypisUspech("Úprava dokončena.");
            }
        }

        /// <summary>
        /// Vyhledává zvíře dle zadání textového řetězce v názvu.
        /// </summary>
        public void Vyhledat()
        {
            Console.Write("Zadejte hledaný výraz: ");
            string hledany = Console.ReadLine()!.Trim().ToLower();

            bool nalezeno = false;

            foreach (var zvire in zoo.Zvirata.Zvirata)
            {
                if (zvire.Nazev.Contains(hledany, StringComparison.CurrentCultureIgnoreCase))
                {
                    ZvirataVypisy.VypisVyhledaneZvire(zvire);
                    nalezeno = true;
                }
            }

            if (!nalezeno)
            {
                VypisyDoKonzole.VypisVarovani("Zvíře nenalezeno.");
            }
        }
    }
}