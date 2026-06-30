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
        public static T ZeptejSeAUprav<T>(
            T aktualni,
            string popis,
            Func<T, string> formatter,   // jak zobrazit hodnotu
            Func<string, T> parser,      // jak zpracovat vstup
            bool jeNove = false)
        {
            try
            {
                if (!jeNove)
                {
                    Console.WriteLine($"Aktuální {popis}: {formatter(aktualni)}\nChcete upravit tuto položku? A/N");
                    if (!Console.ReadLine()!.Equals("A", StringComparison.CurrentCultureIgnoreCase))
                        return aktualni;
                }

                Console.Write($"Nový/á {popis}: ");

                while (true)
                {
                    try
                    {
                        string vstup = Console.ReadLine()!.Trim();
                        T novaHodnota = parser(vstup);
                        return novaHodnota;
                    }
                    catch
                    {
                        Console.WriteLine("Neplatné zadání, zkuste znovu:");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při úpravě {popis}: {ex.Message}");
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