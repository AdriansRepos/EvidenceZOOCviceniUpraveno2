using System.Globalization;

namespace EvidenceZOOCviceniUpraveno2
{
    static class Vstupy
    {
        // Zeptá se na textový vstup, případně umožní úpravu existující hodnoty.
        // Používá TitleCase pro hezký formát jména/pozice.
        public static string ZeptejSeAUpravString(string aktualni, string popis, string prefix, bool jeNove = false)
        {
            // Pokud nejde o nový záznam, nabídne možnost úpravy
            if (!jeNove)
            {
                Console.WriteLine($"Aktuální {popis}: {aktualni}\nChcete upravit tuto položku? A/N");
                if (!Console.ReadLine()!.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                    return aktualni; // uživatel nechce měnit
            }

            // Zadání nové hodnoty
            Console.Write($"{prefix} {popis}: ");
            return ToTitleCase(Console.ReadLine()!.Trim());
        }

        // Zeptá se na celé číslo, s validací a možností úpravy existující hodnoty
        public static int ZeptejSeAUpravInt(int aktualni, string popis, string prefix, bool jeNove = false)
        {
            // Nabídka úpravy u existující hodnoty
            if (!jeNove)
            {
                Console.WriteLine($"Aktuální {popis}: {aktualni}\nChcete upravit tuto položku? A/N");
                if (!Console.ReadLine()!.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                    return aktualni;
            }

            Console.Write($"{prefix} {popis}: ");

            // Validace vstupu
            while (true)
            {
                if (!int.TryParse(Console.ReadLine(), out int novaHodnota))
                {
                    Console.WriteLine("Neplatné zadání, zadejte prosím číslo:");
                    continue;
                }

                if (novaHodnota < 0)
                {
                    Console.WriteLine($"{popis} nemůže být záporný, zkuste to znovu:");
                    continue;
                }

                return novaHodnota;
            }
        }

        // Zeptá se na desetinné číslo (např. váhu), s validací
        public static double ZeptejSeAUpravDouble(double aktualni, string popis, bool jeNove = false)
        {
            // Nabídka úpravy u existující hodnoty
            if (!jeNove)
            {
                Console.WriteLine($"Aktuální {popis}: {aktualni}\nChcete upravit tuto položku? A/N");
                if (!Console.ReadLine()!.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                    return aktualni;
            }

            Console.Write($"Nová {popis}: ");

            // Validace vstupu
            while (true)
            {
                if (!double.TryParse(Console.ReadLine(), out double novaHodnota))
                {
                    Console.WriteLine("Neplatné zadání, zadejte prosím číslo:");
                    continue;
                }

                if (novaHodnota < 0)
                {
                    Console.WriteLine($"{popis} nemůže být záporný, zkuste to znovu:");
                    continue;
                }

                return novaHodnota;
            }
        }

        // Převede text do českého TitleCase (např. "jan novák" → "Jan Novák")
        public static string ToTitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var culture = new CultureInfo("cs-CZ");
            var ti = culture.TextInfo;

            return ti.ToTitleCase(text.ToLower());
        }

        // Umožní vybrat zaměstnance podle pořadového čísla
        public static int VybratIndexZamestnance(ZOO zoo)
        {
            // Výpis seznamu zaměstnanců
            for (int i = 0; i < zoo.Zamestnanci.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {zoo.Zamestnanci[i].Jmeno}");
            }

            Console.Write("Pořadové číslo pro úpravu: ");

            // Validace vstupu
            int cislo;
            while (!int.TryParse(Console.ReadLine(), out cislo))
                Console.WriteLine("Neplatné zadání, zadejte prosím číslo:");

            int index = cislo - 1;

            // Kontrola rozsahu
            if (index >= 0 && index < zoo.Zamestnanci.Count)
                return index;

            Console.WriteLine("Nesprávné pořadové číslo!");
            return -1;
        }

        // Umožní vybrat zvíře podle pořadového čísla
        public static int VybratIndexZvirete(ZOO zoo)
        {
            // Výpis seznamu zvířat
            for (int i = 0; i < zoo.Zvirata.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {zoo.Zvirata[i].Nazev}");
            }

            Console.Write("Pořadové číslo pro úpravu: ");

            // Validace vstupu
            int cislo;
            while (!int.TryParse(Console.ReadLine(), out cislo))
                Console.WriteLine("Neplatné zadání, zadejte prosím číslo:");

            int index = cislo - 1;

            // Kontrola rozsahu
            if (index >= 0 && index < zoo.Zvirata.Count)
                return index;

            Console.WriteLine("Nesprávné pořadové číslo!");
            return -1;
        }
    }


}

