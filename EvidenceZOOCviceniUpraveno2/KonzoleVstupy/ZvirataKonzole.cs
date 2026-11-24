using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.DTO.ZvirataDTO;
using EvidenceZOOCviceniUpraveno2.Logika;
using EvidenceZOOCviceniUpraveno2.Vypisy.Zvirata;
using SelectHelper;
using TextHelper;

namespace EvidenceZOOCviceniUpraveno2.KonzoleVstupy;

internal sealed class ZvirataKonzole(SpravceZvirat spravceZvirat)
{
    private readonly SpravceZvirat spravceZvirat = spravceZvirat;

    public void Pridat()
    {
        VypisyDoKonzole.VypisInformaci("ZADÁNÍ NOVÉHO ZVÍŘETE");

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

    public void Vypis()
    {
        ZvirataVypisy.VypisHlavicku();

        foreach (var zvire in spravceZvirat.ZiskejPrehled())
            ZvirataVypisy.VypisZvire(zvire);
    }

    public void Smazat()
    {
        VypisyDoKonzole.VypisInformaci("SMAZÁNÍ ZVÍŘETE");

        var zvire = SelectHelp.VybratPolozku(spravceZvirat.ZiskejZvirata(), z => z.Nazev, "zvířete");
        if (zvire != null && spravceZvirat.Smazat(zvire))
        {
            VypisyDoKonzole.VypisInformaci($"Zvíře {zvire.Nazev} bylo smazáno.");
        }
    }

    public void Upravit()
    {
        VypisyDoKonzole.VypisInformaci("ÚPRAVA ZVÍŘETE");

        var zvire = SelectHelp.VybratPolozku(spravceZvirat.ZiskejZvirata(), z => z.Nazev, "zvířete");
        if (zvire != null)
        {
            string nazev = UpravaVstupu.ZeptejSeAUprav(
                zvire.Nazev, "název", v => v, s => s);

            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                zvire.DatumNarozeni, "datum narození",
                v => v.ToString(), s => DateOnly.Parse(s));

            double vaha = UpravaVstupu.ZeptejSeAUprav(
                zvire.Vaha, "váha", v => v.ToString(), s => double.Parse(s));

            UpravaZvireteDto dto = new()
            {
                Nazev = nazev,
                DatumNarozeni = datumNarozeni,
                Vaha = vaha
            };

            if (spravceZvirat.Upravit(zvire, dto))
            {
                VypisyDoKonzole.VypisUspech("Úprava dokončena.");
            }
        }
    }

    public void Vyhledat()
    {
        Console.Write("Zadejte hledaný výraz: ");
        string hledany = Console.ReadLine()!.Trim();

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
}