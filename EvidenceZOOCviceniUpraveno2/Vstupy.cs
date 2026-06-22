using System.Globalization;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Statická pomocná třída poskytující metody pro načítání a validaci
    /// uživatelských vstupů z konzole. Obsahuje funkce pro práci s textem,
    /// čísly, daty a výběrem položek.
    /// </summary>
    static class Vstupy
    {
        /// <summary>
        /// Načte textový vstup od uživatele a ověří, že:
        /// <list type="bullet">
        /// <item>neobsahuje zakázaný znak (např. '|')</item>
        /// <item>není prázdný ani tvořený pouze mezerami</item>
        /// </list>
        /// </summary>
        /// <param name="prompt">Text zobrazený uživateli.</param>
        /// <param name="zakazanyZnak">Znak, který nesmí být ve vstupu obsažen.</param>
        /// <returns>Validovaný textový vstup.</returns>
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

        /// <summary>
        /// Zeptá se uživatele na textový vstup a umožní:
        /// <list type="bullet">
        /// <item>ponechat původní hodnotu</item>
        /// <item>upravit ji</item>
        /// </list>
        /// Text je automaticky převeden do TitleCase.
        /// </summary>
        /// <param name="aktualni">Původní hodnota.</param>
        /// <param name="popis">Popis položky (např. "jméno").</param>
        /// <param name="prefix">Text před dotazem (např. "Nové").</param>
        /// <param name="jeNove">Určuje, zda jde o nový záznam.</param>
        /// <returns>Nová nebo původní hodnota.</returns>
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

        /// <summary>
        /// Načte celé číslo s validací a možností úpravy existující hodnoty.
        /// </summary>
        /// <param name="aktualni">Původní hodnota.</param>
        /// <param name="popis">Popis položky.</param>
        /// <param name="prefix">Text před dotazem.</param>
        /// <param name="jeNove">Určuje, zda jde o nový záznam.</param>
        /// <returns>Nová nebo původní hodnota.</returns>
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

        /// <summary>
        /// Načte desetinné číslo (např. váhu) s validací a možností úpravy existující hodnoty.
        /// </summary>
        /// <param name="aktualni">Původní hodnota.</param>
        /// <param name="popis">Popis položky.</param>
        /// <param name="jeNove">Určuje, zda jde o nový záznam.</param>
        /// <returns>Nová nebo původní hodnota.</returns>
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

        /// <summary>
        /// Načte datum typu <see cref="DateOnly"/> s validací a možností úpravy existující hodnoty.
        /// </summary>
        /// <param name="aktualni">Původní datum.</param>
        /// <param name="popis">Popis položky.</param>
        /// <param name="prefix">Text před dotazem.</param>
        /// <param name="jeNove">Určuje, zda jde o nový záznam.</param>
        /// <returns>Nové nebo původní datum.</returns>
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


        /// <summary>
        /// Převede text do českého formátu TitleCase.
        /// </summary>
        /// <param name="text">Vstupní text.</param>
        /// <returns>Text převedený do TitleCase.</returns>
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

        /// <summary>
        /// Zobrazí očíslovaný seznam položek a umožní uživateli vybrat jednu z nich
        /// podle pořadového čísla. Výběr je validován a metoda vrací odpovídající
        /// objekt ze seznamu.
        /// </summary>
        /// <typeparam name="T">Typ položek v seznamu.</typeparam>
        /// <param name="seznam">Seznam položek, ze kterého se vybírá.</param>
        /// <param name="nazev">Funkce určující, jak se má položka zobrazit v seznamu.</param>
        /// <param name="chybaZprava">Text použitý v chybových hláškách.</param>
        /// <returns>Vybraná položka typu T nebo default hodnota při chybě.</returns>
        public static T VybratPolozku<T>(IList<T> seznam, Func<T, string> nazev, string chybaZprava)
        {
            try
            {
                // Vytvoří očíslovaný seznam položek:
                // Select((polozka, p) => ...) vrací text jako "1. Název", "2. Název", ...
                // p = index položky v seznamu
                var radky = seznam.Select((polozka, p) => $"{p + 1}. {nazev(polozka)}");
                foreach (var radek in radky)
                    // Vypíše všechny řádky na obrazovku
                    Console.WriteLine(radek);

                // Požádá uživatele o zadání pořadového čísla
                Console.Write("Pořadové číslo pro úpravu: ");
                int cislo;
                while (!int.TryParse(Console.ReadLine(), out cislo))
                    Console.WriteLine("Neplatné zadání, zadejte prosím číslo:");

                // Pokusí se získat položku na indexu (cislo - 1),
                // protože uživatel zadává 1 = první položka, ale seznam má indexy od 0.
                var vybrany = seznam.ElementAtOrDefault(cislo - 1);
                // Pokud je výsledek null, znamená to, že číslo bylo mimo rozsah seznamu
                if (vybrany == null)
                {
                    Console.WriteLine("Nesprávné pořadové číslo!");
                    throw new InvalidOperationException("Nesprávné pořadové číslo!");
                }
                // Vrací nalezenou položku
                return vybrany;
            }
            catch (Exception ex)
            {
                // Pokud nastane jakákoliv chyba, vypíše se zpráva
                Console.WriteLine($"Chyba při výběru {chybaZprava}: {ex.Message}");
                // Vrací default hodnotu typu T (např. null pro reference)
                return default!;
            }
        }

    }
}

