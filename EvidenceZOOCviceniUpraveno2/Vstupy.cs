using System.Globalization;

namespace EvidenceZOOCviceniUpraveno2
{
    static class Vstupy
    {
        
        public static string ZeptejSeAUpravString(string aktualni, string popis, string prefix, bool jeNove = false)
        {
            if (!jeNove)
            {
                Console.WriteLine($"Aktuální {popis}: {aktualni}\nChcete upravit tuto položku? A/N");
                if (!Console.ReadLine()!.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                    return aktualni;
            }

            Console.Write($"{prefix} {popis}: ");
            return ToTitleCase(Console.ReadLine()!.Trim());
        }

        public static int ZeptejSeAUpravInt(int aktualni,  string popis, string prefix, bool jeNove = false)
        {
            if (!jeNove)
            {
                Console.WriteLine($"Aktuální {popis}: {aktualni}\nChcete upravit tuto položku? A/N");
                if (!Console.ReadLine()!.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                    return aktualni;
            }

            Console.Write($"{prefix} {popis}: ");

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

        public static double ZeptejSeAUpravDouble(double aktualni, string popis, bool jeNove = false)
        {
            if (!jeNove)
            {
                Console.WriteLine($"Aktuální {popis}: {aktualni}\nChcete upravit tuto položku? A/N");
                if (!Console.ReadLine()!.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                    return aktualni;
            }

            Console.Write($"Nová {popis}: ");

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

        public static string ToTitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var culture = new CultureInfo("cs-CZ");
            var ti = culture.TextInfo;

            return ti.ToTitleCase(text.ToLower());
        }

        
        public static int VybratIndexZamestnance(ZOO zoo)
        {
            for (int i = 0; i < zoo.Zamestnanci.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {zoo.Zamestnanci[i].Jmeno}");
            }
            Console.Write("Pořadové číslo pro úpravu: ");
            int cislo;
            while (!int.TryParse(Console.ReadLine(), out cislo))
                Console.WriteLine("Neplatné zadání, zadejte prosím číslo:");

            int index = cislo - 1;

            if (index >= 0 && index < zoo.Zamestnanci.Count)
                return index;

            Console.WriteLine("Nesprávné pořadové číslo!");
            return -1;
        }

        
        public static int VybratIndexZvirete(ZOO zoo)
        {
            for (int i = 0; i < zoo.Zvirata.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {zoo.Zvirata[i].Nazev}");
            }
            Console.Write("Pořadové číslo pro úpravu: ");
            int cislo;
            while (!int.TryParse(Console.ReadLine(), out cislo))
                Console.WriteLine("Neplatné zadání, zadejte prosím číslo:");

            int index = cislo - 1;

            if (index >= 0 && index < zoo.Zvirata.Count)
                return index;

            Console.WriteLine("Nesprávné pořadové číslo!");
            return -1;
        }
    }

}

