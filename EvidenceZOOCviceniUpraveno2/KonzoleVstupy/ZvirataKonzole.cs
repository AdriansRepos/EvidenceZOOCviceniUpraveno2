using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.DTO.ZvirataDTO;
using EvidenceZOOCviceniUpraveno2.Logika;
using EvidenceZOOCviceniUpraveno2.Vypisy.Zvirata;
using SelectHelper;
using TextHelper;

namespace EvidenceZOOCviceniUpraveno2.KonzoleVstupy;

internal static class ZvirataKonzole
{
    public static void Pridat(SpravceZvirat spravceZvirat)
    {
        try
        {
            VypisyDoKonzole.VypisInformaci("ZADÁNÍ NOVÉHO ZVÍŘETE");

            string nazev = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "název",
                v => v,
                s => PovinnyText(s, "název"),
                jeNove: true);

            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                default,
                "datum narození (DD.MM.RRRR)",
                v => v.ToString("dd.MM.yyyy"),
                s => DateOnly.Parse(s),
                jeNove: true);

            double vaha = UpravaVstupu.ZeptejSeAUprav(
                0.0,
                "váha",
                v => v.ToString(),
                s => double.Parse(s),
                jeNove: true);

            Console.Write("Bylo zvíře přijato z jiné zoo? (Enter = ne, narozeno zde) Datum přijetí: ");
            string vstupDatumPrijeti = (Console.ReadLine() ?? string.Empty).Trim();
            DateOnly? datumPrijetiZJineZoo = string.IsNullOrWhiteSpace(vstupDatumPrijeti)
                ? null
                : DateOnly.Parse(vstupDatumPrijeti);

            NoveZvireDto dto = new()
            {
                Nazev = nazev,
                DatumNarozeni = datumNarozeni,
                Vaha = vaha,
                DatumPrijetiZJineZoo = datumPrijetiZJineZoo
            };

            spravceZvirat.Pridat(dto);
            VypisyDoKonzole.VypisUspech("Zvíře bylo úspěšně přidáno.");
        }
        catch (Exception ex)
        {
            VypisyDoKonzole.VypisVarovani($"Chyba při přidávání zvířete: {ex.Message}");
        }
    }

    public static void Vypis(SpravceZvirat spravceZvirat)
    {
        var prehled = spravceZvirat.ZiskejPrehled().ToList();
        if (prehled.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádná zvířata.");
            return;
        }

        ZvirataVypisy.VypisHlavicku();

        foreach (var zvire in prehled)
        {
            ZvirataVypisy.VypisZvire(zvire);
        }
    }

    public static void Smazat(SpravceZvirat spravceZvirat)
    {
        var zvirata = spravceZvirat.ZiskejZvirata().ToList();
        if (zvirata.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádná zvířata.");
            return;
        }

        VypisyDoKonzole.VypisInformaci("SMAZÁNÍ ZVÍŘETE");

        var zvire = SelectHelp.VybratPolozku(zvirata, z => z.Nazev, "zvířete");
        if (zvire == null)
            return;

        if (spravceZvirat.Smazat(zvire))
            VypisyDoKonzole.VypisUspech($"Zvíře {zvire.Nazev} bylo smazáno.");
        else
            VypisyDoKonzole.VypisVarovani("Zvíře se nepodařilo smazat.");
    }

    public static void Upravit(SpravceZvirat spravceZvirat)
    {
        var zvirata = spravceZvirat.ZiskejZvirata().ToList();
        if (zvirata.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádná zvířata.");
            return;
        }

        VypisyDoKonzole.VypisInformaci("ÚPRAVA ZVÍŘETE");

        var zvire = SelectHelp.VybratPolozku(zvirata, z => z.Nazev, "zvířete");
        if (zvire == null)
            return;

        try
        {
            string nazev = UpravaVstupu.ZeptejSeAUprav(
                zvire.Nazev,
                "název",
                v => v,
                s => PovinnyText(s, "název"));

            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                zvire.DatumNarozeni,
                "datum narození (DD.MM.RRRR)",
                v => v.ToString("dd.MM.yyyy"),
                s => DateOnly.Parse(s));

            double vaha = UpravaVstupu.ZeptejSeAUprav(
                zvire.Vaha,
                "váha",
                v => v.ToString(),
                s => double.Parse(s));

            UpravaZvireteDto dto = new()
            {
                Nazev = nazev,
                DatumNarozeni = datumNarozeni,
                Vaha = vaha
            };

            if (spravceZvirat.Upravit(zvire, dto))
                VypisyDoKonzole.VypisUspech("Úprava dokončena.");
            else
                VypisyDoKonzole.VypisVarovani("Zvíře se nepodařilo upravit.");
        }
        catch (Exception ex)
        {
            VypisyDoKonzole.VypisVarovani($"Chyba při úpravě zvířete: {ex.Message}");
        }
    }

    public static void Vyhledat(SpravceZvirat spravceZvirat)
    {
        Console.Write("Zadejte hledaný výraz: ");
        string hledany = (Console.ReadLine() ?? string.Empty).Trim();

        var nalezena = spravceZvirat.Vyhledat(hledany);

        if (nalezena.Count == 0)
        {
            VypisyDoKonzole.VypisVarovani("Zvíře nenalezeno.");
            return;
        }

        foreach (var zvire in nalezena)
        {
            ZvirataVypisy.VypisVyhledaneZvire(zvire);
        }
    }

    private static string PovinnyText(string vstup, string nazevPole)
    {
        if (string.IsNullOrWhiteSpace(vstup))
            throw new ArgumentException($"Zadejte platné pole: {nazevPole}.");

        return vstup.Trim();
    }
}
