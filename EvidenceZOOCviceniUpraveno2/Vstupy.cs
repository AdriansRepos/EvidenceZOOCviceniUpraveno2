using System.Globalization;

namespace EvidenceZOOCviceniUpraveno2
{
    static class Vstupy
    {
        // Zeptá se na textový vstup a zkontroluje, že neobsahuje zakázaný znak (např. oddělovač) a není prázdný.
        public static string NactiBezZakazanychZnaku(string prompt, char zakazanyZnak)
        {
            while (true)
            {
                try
                {
                    Console.Write(prompt);
                    string text = Console.ReadLine() ?? "";

                    if (text.Contains(zakazanyZnak))
                    {
                        Console.WriteLine($"Text nesmí obsahovat znak '{zakazanyZnak}'. Zadejte prosím znovu.");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        Console.WriteLine("Text nesmí být prázdný. Zadejte prosím znovu.");
                        continue;
                    }

                    return text;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Chyba při načítání textu: {ex.Message}");
                }
            }
        }

        // Zeptá se na textový vstup, případně umožní úpravu existující hodnoty.
        public static string ZeptejSeAUpravString(string aktualni, string popis, string prefix, bool jeNove = false)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při úpravě textu: {ex.Message}");
                return aktualni;
            }
        }

        // Zeptá se na celé číslo, s validací a možností úpravy existující hodnoty
        public static int ZeptejSeAUpravInt(int aktualni, string popis, string prefix, bool jeNove = false)
        {
            try
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
                    try
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
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Chyba při načítání čísla: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při úpravě čísla: {ex.Message}");
                return aktualni;
            }
        }

        // Zeptá se na desetinné číslo (např. váhu), s validací
        public static double ZeptejSeAUpravDouble(double aktualni, string popis, bool jeNove = false)
        {
            try
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
                    try
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
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Chyba při načítání čísla: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při úpravě čísla: {ex.Message}");
                return aktualni;
            }
        }

        public static DateOnly ZeptejSeAUpravDateOnly(DateOnly aktualni, string popis, string prefix, bool jeNove = false)
        {
            try
            {
                // Pokud NEjde o nový záznam → nabídneme úpravu
                if (!jeNove)
                {
                    Console.WriteLine($"Aktuální {popis}: {aktualni}\nChcete upravit tuto položku? A/N");
                    string odpoved = Console.ReadLine()!.Trim();

                    // Pokud NEchce upravit → vracíme původní hodnotu
                    if (!odpoved.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                        return aktualni;
                }

                // Tady jsme buď v režimu "nový záznam", nebo uživatel zvolil "A"
                Console.Write($"{prefix} {popis}: ");

                while (true)
                {
                    try
                    {
                        string? vstup = Console.ReadLine();

                        if (!DateOnly.TryParse(vstup, out DateOnly noveDatum))
                        {
                            Console.WriteLine("Neplatné zadání, zkuste znovu!");
                            Console.Write($"{prefix} {popis}: ");
                            continue;
                        }

                        return noveDatum;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Chyba při načítání data: {ex.Message}");
                        Console.Write($"{prefix} {popis}: ");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při úpravě data: {ex.Message}");
                return aktualni;
            }
        }


        // Převede text do českého TitleCase
        public static string ToTitleCase(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return text;

                var culture = new CultureInfo("cs-CZ");
                var ti = culture.TextInfo;

                return ti.ToTitleCase(text.ToLower());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při převodu textu: {ex.Message}");
                return text;
            }
        }

        // Výběr zaměstnance podle indexu
        public static int VybratIndexZamestnance(ZOO zoo)
        {
            try
            {
                for (int i = 0; i < zoo.Zamestnanci.Count; i++)
                    Console.WriteLine($"{i + 1}. {zoo.Zamestnanci[i].Jmeno}");

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
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při výběru zaměstnance: {ex.Message}");
                return -1;
            }
        }

        // Výběr zvířete podle indexu
        public static int VybratIndexZvirete(ZOO zoo)
        {
            try
            {
                for (int i = 0; i < zoo.Zvirata.Count; i++)
                    Console.WriteLine($"{i + 1}. {zoo.Zvirata[i].Nazev}");

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
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při výběru zvířete: {ex.Message}");
                return -1;
            }
        }
    }

}

