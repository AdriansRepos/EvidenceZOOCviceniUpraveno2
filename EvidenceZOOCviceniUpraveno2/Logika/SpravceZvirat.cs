using TextHelper;
using SelectHelper;
using EvidenceZOOCviceniUpraveno2.Entity;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    /// <summary>
    /// Třída zodpovědná za správu zvířat – přidávání, mazání, úpravy,
    /// výpisy a vyhledávání. Pracuje s daty uloženými v instanci <see cref="Zoo"/>.
    /// </summary>
    /// <param name="zoo">Instance třídy Zoo obsahující repository pro zvířata.</param>
    class SpravceZvirat
    {
        private readonly Zoo zoo;

        public SpravceZvirat(Zoo zoo)
        {
            this.zoo = zoo;
            this.zoo.Zvirata.Nacti();
        }

        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZVÍŘETE");

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

            zoo.Zvirata.Zvirata.Add(nove);
            zoo.Zvirata.UlozZvire(nove);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Zvíře bylo úspěšně přidáno.");
            Console.ResetColor();
        }

        public void Vypis()
        {
            Console.WriteLine("VÝPIS ZVÍŘAT");
            Console.WriteLine();

            Console.WriteLine(
                $"{"Název",-20} {"Věk",-15} {"Váha",-10}"
            );

            Console.WriteLine(new string('-', 45));
            foreach (var zvire in zoo.Zvirata.Zvirata)
                zvire.VypisZvire();
        }

        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZVÍŘETE");

            var zvire = SelectHelp.VybratPolozku(zoo.Zvirata.Zvirata, z => z.Nazev, "zvířete");
            if (zvire != null)
            {
                zoo.Zvirata.Zvirata.Remove(zvire);
                zoo.Zvirata.SmazatZvireSoubor(zvire);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Zvíře {zvire.Nazev} bylo smazáno.");
                Console.ResetColor();
            }
        }

        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZVÍŘETE");
            var zvire = SelectHelp.VybratPolozku(zoo.Zvirata.Zvirata, z => z.Nazev, "zvířete");
            if (zvire != null)
            {
                string puvodniSoubor = zoo.Zvirata.SouborZvirete(zvire);

                zvire.Nazev = UpravaVstupu.ZeptejSeAUprav(
                    zvire.Nazev, "název", v => v, s => s);

                zvire.DatumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                    zvire.DatumNarozeni, "datum narození",
                    v => v.ToString(), s => DateOnly.Parse(s));

                zvire.Vaha = UpravaVstupu.ZeptejSeAUprav(
                    zvire.Vaha, "váha", v => v.ToString(), s => double.Parse(s));

                zoo.Zvirata.UlozZvire(zvire, puvodniSoubor);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Úprava dokončena.");
                Console.ResetColor();
            }
        }

        public void Vyhledat()
        {
            Console.Write("Zadejte hledaný výraz: ");
            string hledany = Console.ReadLine()!.Trim().ToLower();

            bool nalezeno = false;

            foreach (var zvire in zoo.Zvirata.Zvirata)
            {
                if (zvire.Nazev.Contains(hledany, StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine(
                        $"Nalezeno:\t{zvire.Nazev}" +
                        $"\tVěk: {zvire.Vek}" +
                        $"\tVáha: {zvire.Vaha}"
                    );
                    nalezeno = true;
                }
            }

            if (!nalezeno)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Zvíře nenalezeno.");
                Console.ResetColor();
            }
        }
    }
}