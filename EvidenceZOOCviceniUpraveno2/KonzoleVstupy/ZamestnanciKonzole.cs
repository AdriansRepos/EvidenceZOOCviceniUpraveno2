using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.DTO.ZamestnanciDTO;
using EvidenceZOOCviceniUpraveno2.Enumy;
using EvidenceZOOCviceniUpraveno2.Logika;
using EvidenceZOOCviceniUpraveno2.Vypisy.Zamestnanci;
using PohybHelper;
using SelectHelper;
using TextHelper;
using InputHelper;

namespace EvidenceZOOCviceniUpraveno2.KonzoleVstupy;

internal static class ZamestnanciKonzole
{
    public static void PridatZamestnance(SpravceZamestnancu spravceZamestnancu)
    {
        try
        {
            var (jmeno, prijmeni) = UpravaVstupu.ZeptejSeAUprav(
                (jmeno: string.Empty, prijmeni: string.Empty),
                "celé jméno (Jméno Příjmení)",
                v => $"{v.jmeno} {v.prijmeni}".Trim(),
                s => OsobniJmeno.Rozdel(s),
                jeNove: true);

            string pracovniPozice = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "pracovní pozici",
                v => v,
                s => PovinnyText(s, "pracovní pozici"),
                jeNove: true);

            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                default,
                "datum narození (DD.MM.RRRR)",
                v => v.ToString("dd.MM.yyyy"),
                s => DateOnly.Parse(s),
                jeNove: true);

            string rodneCislo = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "rodné číslo",
                v => v,
                s => InputHelper.RodneCislo.Zkontroluj(s),
                jeNove: true);

            int mzda = UpravaVstupu.ZeptejSeAUprav(
                0,
                "mzdu",
                v => v.ToString(),
                s => int.Parse(s),
                jeNove: true);

            string mesto = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "město",
                v => v,
                s => MestoTitleCase.ZpracujNazevMesta(s),
                jeNove: true);

            string ulice = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "ulici",
                v => v,
                s => PovinnyText(s, "ulici"),
                jeNove: true);

            string psc = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "PSČ",
                v => v,
                s => PovinnyText(s, "PSČ"),
                jeNove: true);

            string telefon = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "telefon",
                v => v,
                s => PovinnyText(s, "telefon"),
                jeNove: true);

            string email = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "e-mail",
                v => v,
                s => PovinnyText(s, "e-mail"),
                jeNove: true);

            RodinnyStav rodinnyStav = VyberEnum<RodinnyStav>("Rodinný stav");
            ZdravotniStav zdravotniStav = VyberEnum<ZdravotniStav>("Zdravotní stav");

            string typDokladu = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "typ dokladu",
                v => v,
                s => PovinnyText(s, "typ dokladu"),
                jeNove: true);

            string cisloDokladu = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "číslo dokladu",
                v => v,
                s => PovinnyText(s, "číslo dokladu"),
                jeNove: true);

            ZamestnanecVytvoreniDto dto = new()
            {
                Jmeno = jmeno,
                Prijmeni = prijmeni,
                PracovniPozice = pracovniPozice,
                DatumNarozeni = datumNarozeni,
                RodneCislo = rodneCislo,
                Mzda = mzda,
                Mesto = mesto,
                Ulice = ulice,
                PSC = psc,
                Telefon = telefon,
                Email = email,
                RodinnyStav = rodinnyStav,
                ZdravotniStav = zdravotniStav,
                TypDokladu = typDokladu,
                CisloDokladu = cisloDokladu
            };

            spravceZamestnancu.Pridat(dto);
            VypisyDoKonzole.VypisUspech("Zaměstnanec byl přidán.");
        }
        catch (Exception ex)
        {
            VypisyDoKonzole.VypisVarovani($"Chyba při přidávání zaměstnance: {ex.Message}");
        }
    }

    public static void VypisZamestnance(List<ZamestnanecPrehledDto> zamestnanci)
    {
        if (zamestnanci.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádní zaměstnanci.");
            return;
        }

        ZamestnanciVypisy.VypisHlavicku();

        foreach (var zam in zamestnanci)
        {
            ZamestnanciVypisy.VypisZamestnance(zam);
            ZamestnanciVypisy.VypisKontaktniUdaje(zam);
            Console.WriteLine();
        }
    }

    public static void SmazatZamestnance(SpravceZamestnancu spravceZamestnancu)
    {
        var prehled = spravceZamestnancu.VratPrehled();
        if (prehled.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádní zaměstnanci.");
            return;
        }

        Console.WriteLine("SMAZÁNÍ ZAMĚSTNANCE");
        var vybrany = SelectHelp.VybratPolozku(
            prehled,
            z => $"{z.OsobniCislo} - {z.Jmeno} {z.Prijmeni}",
            "zaměstnance");

        if (vybrany == null)
            return;

        bool smazano = spravceZamestnancu.Smazat(vybrany.OsobniCislo);
        if (smazano)
            VypisyDoKonzole.VypisUspech($"Zaměstnanec {vybrany.Prijmeni} byl smazán.");
        else
            VypisyDoKonzole.VypisVarovani("Zaměstnance se nepodařilo smazat.");
    }

    public static void UpravitZamestnance(SpravceZamestnancu spravceZamestnancu)
    {
        var prehled = spravceZamestnancu.VratPrehled();
        if (prehled.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádní zaměstnanci.");
            return;
        }

        Console.WriteLine("ÚPRAVA ZAMĚSTNANCE");
        var vybrany = SelectHelp.VybratPolozku(
            prehled,
            z => $"{z.OsobniCislo} - {z.Jmeno} {z.Prijmeni}",
            "zaměstnance");

        if (vybrany == null)
            return;

        try
        {
            string jmeno = UpravaVstupu.ZeptejSeAUprav(
                vybrany.Jmeno,
                "jméno",
                v => v,
                s => TitleCase.ToTitleCase(s));

            string prijmeni = UpravaVstupu.ZeptejSeAUprav(
                vybrany.Prijmeni,
                "příjmení",
                v => v,
                s => TitleCase.ToTitleCase(s));

            string pracovniPozice = UpravaVstupu.ZeptejSeAUprav(
                vybrany.PracovniPozice,
                "pracovní pozici",
                v => v,
                s => PovinnyText(s, "pracovní pozici"));

            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                vybrany.DatumNarozeni,
                "datum narození (DD.MM.RRRR)",
                v => v.ToString("dd.MM.yyyy"),
                s => DateOnly.Parse(s));

            string rodneCislo = UpravaVstupu.ZeptejSeAUprav(
                vybrany.RodneCislo,
                "rodné číslo",
                v => v,
                s => InputHelper.RodneCislo.Zkontroluj(s));

            int mzda = UpravaVstupu.ZeptejSeAUprav(
                vybrany.Mzda,
                "mzdu",
                v => v.ToString(),
                s => int.Parse(s));

            string mesto = UpravaVstupu.ZeptejSeAUprav(
                vybrany.Mesto,
                "město",
                v => v,
                s => MestoTitleCase.ZpracujNazevMesta(s));

            string ulice = UpravaVstupu.ZeptejSeAUprav(
                vybrany.Ulice,
                "ulici",
                v => v,
                s => PovinnyText(s, "ulici"));

            string psc = UpravaVstupu.ZeptejSeAUprav(
                vybrany.PSC,
                "PSČ",
                v => v,
                s => PovinnyText(s, "PSČ"));

            string telefon = UpravaVstupu.ZeptejSeAUprav(
                vybrany.Telefon,
                "telefon",
                v => v,
                s => PovinnyText(s, "telefon"));

            string email = UpravaVstupu.ZeptejSeAUprav(
                vybrany.Email,
                "e-mail",
                v => v,
                s => PovinnyText(s, "e-mail"));

            RodinnyStav rodinnyStav = VyberEnum<RodinnyStav>(
                $"Rodinný stav (aktuálně: {PopiskyHelper.ZiskejPopisek(vybrany.RodinnyStav)})");

            ZdravotniStav zdravotniStav = VyberEnum<ZdravotniStav>(
                $"Zdravotní stav (aktuálně: {PopiskyHelper.ZiskejPopisek(vybrany.ZdravotniStav)})");

            string typDokladu = UpravaVstupu.ZeptejSeAUprav(
                vybrany.TypDokladu,
                "typ dokladu",
                v => v,
                s => PovinnyText(s, "typ dokladu"));

            string cisloDokladu = UpravaVstupu.ZeptejSeAUprav(
                vybrany.CisloDokladu,
                "číslo dokladu",
                v => v,
                s => PovinnyText(s, "číslo dokladu"));

            ZamestnanecUpravaDto dto = new()
            {
                OsobniCislo = vybrany.OsobniCislo,
                Jmeno = jmeno,
                Prijmeni = prijmeni,
                PracovniPozice = pracovniPozice,
                DatumNarozeni = datumNarozeni,
                RodneCislo = rodneCislo,
                Mzda = mzda,
                Mesto = mesto,
                Ulice = ulice,
                PSC = psc,
                Telefon = telefon,
                Email = email,
                RodinnyStav = rodinnyStav,
                ZdravotniStav = zdravotniStav,
                TypDokladu = typDokladu,
                CisloDokladu = cisloDokladu
            };

            bool upraveno = spravceZamestnancu.Upravit(dto);
            if (upraveno)
                VypisyDoKonzole.VypisUspech("Údaje zaměstnance byly upraveny.");
            else
                VypisyDoKonzole.VypisVarovani("Zaměstnance se nepodařilo upravit.");
        }
        catch (Exception ex)
        {
            VypisyDoKonzole.VypisVarovani($"Chyba při úpravě zaměstnance: {ex.Message}");
        }
    }

    public static void VyhledatZamestnance(SpravceZamestnancu spravceZamestnancu)
    {
        Console.Write("Zadejte hledané příjmení: ");
        string hledany = Console.ReadLine() ?? string.Empty;

        var nalezeni = spravceZamestnancu.Vyhledat(hledany);

        if (nalezeni.Count == 0)
        {
            VypisyDoKonzole.VypisVarovani("Zaměstnanec nenalezen.");
            return;
        }

        ZamestnanciVypisy.VypisHlavicku();
        foreach (var zam in nalezeni)
        {
            ZamestnanciVypisy.VypisZamestnance(zam);
            ZamestnanciVypisy.VypisKontaktniUdaje(zam);
            Console.WriteLine();
        }
    }

    private static string PovinnyText(string vstup, string nazevPole)
    {
        if (string.IsNullOrWhiteSpace(vstup))
            throw new ArgumentException($"Zadejte platné pole: {nazevPole}.");

        return vstup.Trim();
    }

    private static TEnum VyberEnum<TEnum>(string nadpis) where TEnum : struct, Enum
    {
        var hodnoty = Enum.GetValues<TEnum>();
        Console.WriteLine($"{nadpis}:");
        for (int i = 0; i < hodnoty.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {PopiskyHelper.ZiskejPopisek(hodnoty[i])}");
        }

        while (true)
        {
            Console.Write("Vyber číslo: ");
            if (int.TryParse(Console.ReadLine(), out int volba) && volba >= 1 && volba <= hodnoty.Length)
                return hodnoty[volba - 1];

            VypisyDoKonzole.VypisInformaci("Neplatná volba, zkuste znovu.");
        }
    }

    public static void PridatDiteZamestnanci(SpravceZamestnancu spravceZamestnancu)
    {
        var prehled = spravceZamestnancu.VratPrehled();
        if (prehled.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádní zaměstnanci.");
            return;
        }
    
        var vybrany = SelectHelp.VybratPolozku(
            prehled,
            z => $"{z.OsobniCislo} - {z.Jmeno} {z.Prijmeni}",
            "zaměstnance");
    
        if (vybrany == null)
            return;
    
        try
        {
            string jmeno = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "jméno dítěte",
                v => v,
                s => TitleCase.ToTitleCase(s),
                jeNove: true);
    
            string prijmeni = UpravaVstupu.ZeptejSeAUprav(
                vybrany.Prijmeni,
                "příjmení dítěte",
                v => v,
                s => TitleCase.ToTitleCase(s),
                jeNove: true);
    
            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                default,
                "datum narození dítěte (DD.MM.RRRR)",
                v => v.ToString("dd.MM.yyyy"),
                s => DateOnly.Parse(s),
                jeNove: true);
    
            string rodneCislo = UpravaVstupu.ZeptejSeAUprav(
                string.Empty,
                "rodné číslo dítěte",
                v => v,
                s => InputHelper.RodneCislo.Zkontroluj(s),
                jeNove: true);
    
            bool jeDrzitelZtpP = NactiAnoNe("Je dítě držitelem ZTP/P?");
            bool uplatnenBonus = NactiAnoNe("Je uplatněn daňový bonus na dítě?");
    
            DiteVytvoreniDto dto = new()
            {
                OsobniCisloZamestnance = vybrany.OsobniCislo,
                Jmeno = jmeno,
                Prijmeni = prijmeni,
                DatumNarozeni = datumNarozeni,
                RodneCislo = rodneCislo,
                JeDrzitelZtpP = jeDrzitelZtpP,
                UplatnenBonus = uplatnenBonus
            };
    
            if (spravceZamestnancu.PridatDite(dto))
                VypisyDoKonzole.VypisUspech("Dítě bylo přidáno.");
            else
                VypisyDoKonzole.VypisVarovani("Dítě se nepodařilo přidat.");
        }
        catch (Exception ex)
        {
            VypisyDoKonzole.VypisVarovani($"Chyba při přidávání dítěte: {ex.Message}");
        }
    }
    
    public static void UpravitDiteZamestnanci(SpravceZamestnancu spravceZamestnancu)
    {
        var prehled = spravceZamestnancu.VratPrehled();
        if (prehled.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádní zaměstnanci.");
            return;
        }
    
        var vybranyZam = SelectHelp.VybratPolozku(
            prehled,
            z => $"{z.OsobniCislo} - {z.Jmeno} {z.Prijmeni}",
            "zaměstnance");
    
        if (vybranyZam == null)
            return;
    
        if (vybranyZam.Deti.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("Zaměstnanec nemá evidované žádné děti.");
            return;
        }
    
        var vybraneDite = SelectHelp.VybratPolozku(
            vybranyZam.Deti,
            d => $"{d.Jmeno} {d.Prijmeni} (RČ: {d.RodneCislo})",
            "dítě");
    
        if (vybraneDite == null)
            return;
    
        try
        {
            string jmeno = UpravaVstupu.ZeptejSeAUprav(
                vybraneDite.Jmeno,
                "jméno dítěte",
                v => v,
                s => TitleCase.ToTitleCase(s));
    
            string prijmeni = UpravaVstupu.ZeptejSeAUprav(
                vybraneDite.Prijmeni,
                "příjmení dítěte",
                v => v,
                s => TitleCase.ToTitleCase(s));
    
            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                vybraneDite.DatumNarozeni,
                "datum narození dítěte (DD.MM.RRRR)",
                v => v.ToString("dd.MM.yyyy"),
                s => DateOnly.Parse(s));
    
            string rodneCislo = UpravaVstupu.ZeptejSeAUprav(
                vybraneDite.RodneCislo,
                "rodné číslo dítěte",
                v => v,
                s => InputHelper.RodneCislo.Zkontroluj(s));
    
            bool jeDrzitelZtpP = UpravaVstupu.ZeptejSeAUprav(
                vybraneDite.JeDrzitelZtpP,
                "držitel ZTP/P (A/N)",
                v => v ? "Ano" : "Ne",
                s => ParseAnoNe(s));
    
            bool uplatnenBonus = UpravaVstupu.ZeptejSeAUprav(
                vybraneDite.UplatnenBonus,
                "uplatněn daňový bonus (A/N)",
                v => v ? "Ano" : "Ne",
                s => ParseAnoNe(s));
    
            DiteUpravaDto dto = new()
            {
                OsobniCisloZamestnance = vybranyZam.OsobniCislo,
                PuvodniRodneCislo = vybraneDite.RodneCislo,
                Jmeno = jmeno,
                Prijmeni = prijmeni,
                DatumNarozeni = datumNarozeni,
                RodneCislo = rodneCislo,
                JeDrzitelZtpP = jeDrzitelZtpP,
                UplatnenBonus = uplatnenBonus
            };
    
            if (spravceZamestnancu.UpravitDite(dto))
                VypisyDoKonzole.VypisUspech("Údaje dítěte byly upraveny.");
            else
                VypisyDoKonzole.VypisVarovani("Údaje dítěte se nepodařilo upravit.");
        }
        catch (Exception ex)
        {
            VypisyDoKonzole.VypisVarovani($"Chyba při úpravě dítěte: {ex.Message}");
        }
    }
    
    public static void UlozitPartneraZamestnanci(SpravceZamestnancu spravceZamestnancu)
    {
        var prehled = spravceZamestnancu.VratPrehled();
        if (prehled.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádní zaměstnanci.");
            return;
        }
    
        var vybranyZam = SelectHelp.VybratPolozku(
            prehled,
            z => $"{z.OsobniCislo} - {z.Jmeno} {z.Prijmeni}",
            "zaměstnance");
    
        if (vybranyZam == null)
            return;
    
        var akt = vybranyZam.Partner;
        bool jeNovy = akt == null;
    
        try
        {
            string jmeno = UpravaVstupu.ZeptejSeAUprav(
                akt?.Jmeno ?? string.Empty,
                "jméno partnera",
                v => v,
                s => TitleCase.ToTitleCase(s),
                jeNove: jeNovy);
    
            string prijmeni = UpravaVstupu.ZeptejSeAUprav(
                akt?.Prijmeni ?? string.Empty,
                "příjmení partnera",
                v => v,
                s => TitleCase.ToTitleCase(s),
                jeNove: jeNovy);
    
            string rodneCislo = UpravaVstupu.ZeptejSeAUprav(
                akt?.RodneCislo ?? string.Empty,
                "rodné číslo partnera",
                v => v,
                s => InputHelper.RodneCislo.Zkontroluj(s),
                jeNove: jeNovy);
    
            bool jeDrzitelZtpP = jeNovy
                ? NactiAnoNe("Je partner držitelem ZTP/P?")
                : UpravaVstupu.ZeptejSeAUprav(
                    akt!.JeDrzitelZtpP,
                    "držitel ZTP/P (A/N)",
                    v => v ? "Ano" : "Ne",
                    s => ParseAnoNe(s));
    
            bool pecujeODiteDo3Let = jeNovy
                ? NactiAnoNe("Pečuje partner o dítě do 3 let?")
                : UpravaVstupu.ZeptejSeAUprav(
                    akt!.PecujeODiteDo3Let,
                    "péče o dítě do 3 let (A/N)",
                    v => v ? "Ano" : "Ne",
                    s => ParseAnoNe(s));
    
            bool prijemDo68Tisic = jeNovy
                ? NactiAnoNe("Má partner příjem do 68 000 Kč?")
                : UpravaVstupu.ZeptejSeAUprav(
                    akt!.PrijemDo68Tisic,
                    "příjem do 68 tis. (A/N)",
                    v => v ? "Ano" : "Ne",
                    s => ParseAnoNe(s));
    
            bool uplatnitSlevu = jeNovy
                ? NactiAnoNe("Uplatnit slevu na partnera?")
                : UpravaVstupu.ZeptejSeAUprav(
                    akt!.UplatnitSlevu,
                    "uplatnit slevu (A/N)",
                    v => v ? "Ano" : "Ne",
                    s => ParseAnoNe(s));
    
            PartnerUlozeniDto dto = new()
            {
                OsobniCisloZamestnance = vybranyZam.OsobniCislo,
                Jmeno = jmeno,
                Prijmeni = prijmeni,
                RodneCislo = rodneCislo,
                JeDrzitelZtpP = jeDrzitelZtpP,
                PecujeODiteDo3Let = pecujeODiteDo3Let,
                PrijemDo68Tisic = prijemDo68Tisic,
                UplatnitSlevu = uplatnitSlevu
            };
    
            if (spravceZamestnancu.UlozitPartnera(dto))
                VypisyDoKonzole.VypisUspech("Údaje partnera byly uloženy.");
            else
                VypisyDoKonzole.VypisVarovani("Údaje partnera se nepodařilo uložit.");
        }
        catch (Exception ex)
        {
            VypisyDoKonzole.VypisVarovani($"Chyba při ukládání partnera: {ex.Message}");
        }
    }
    
    private static bool NactiAnoNe(string otazka)
    {
        while (true)
        {
            Console.Write($"{otazka} (A/N): ");
            string vstup = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.Equals(vstup, "A", StringComparison.CurrentCultureIgnoreCase))
                return true;
            if (string.Equals(vstup, "N", StringComparison.CurrentCultureIgnoreCase))
                return false;
    
            VypisyDoKonzole.VypisInformaci("Zadejte prosím A nebo N.");
        }
    }
    
    private static bool ParseAnoNe(string vstup)
    {
        if (string.Equals(vstup, "A", StringComparison.CurrentCultureIgnoreCase))
            return true;
        if (string.Equals(vstup, "N", StringComparison.CurrentCultureIgnoreCase))
            return false;
    
        throw new ArgumentException("Zadejte A nebo N.");
    }

    public static void OdebratDiteZamestnanci(SpravceZamestnancu spravceZamestnancu)
        {
            var prehled = spravceZamestnancu.VratPrehled();
            if (prehled.Count == 0)
            {
                VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádní zaměstnanci.");
                return;
            }

            var vybranyZam = SelectHelp.VybratPolozku(
                prehled,
                z => $"{z.OsobniCislo} - {z.Jmeno} {z.Prijmeni}",
                "zaměstnance");

            if (vybranyZam == null)
                return;

            if (vybranyZam.Deti.Count == 0)
            {
                VypisyDoKonzole.VypisInformaci("Zaměstnanec nemá evidované žádné děti.");
                return;
            }

            var vybraneDite = SelectHelp.VybratPolozku(
                vybranyZam.Deti,
                d => $"{d.Jmeno} {d.Prijmeni} (RČ: {d.RodneCislo})",
                "dítě");

            if (vybraneDite == null)
                return;

            if (spravceZamestnancu.OdebratDite(vybranyZam.OsobniCislo, vybraneDite.RodneCislo))
                VypisyDoKonzole.VypisUspech("Dítě bylo odebráno.");
            else
                VypisyDoKonzole.VypisVarovani("Dítě se nepodařilo odebrat.");
    }

    public static void OdebratPartneraZamestnanci(SpravceZamestnancu spravceZamestnancu)
    {
        var prehled = spravceZamestnancu.VratPrehled();
        if (prehled.Count == 0)
        {
            VypisyDoKonzole.VypisInformaci("V evidenci nejsou žádní zaměstnanci.");
            return;
        }

        var vybranyZam = SelectHelp.VybratPolozku(
            prehled,
            z => $"{z.OsobniCislo} - {z.Jmeno} {z.Prijmeni}",
            "zaměstnance");

        if (vybranyZam == null)
            return;

        if (vybranyZam.Partner == null)
        {
            VypisyDoKonzole.VypisInformaci("Zaměstnanec nemá evidovaného žádného partnera.");
            return;
        }

        if (spravceZamestnancu.OdebratPartnera(vybranyZam.OsobniCislo))
            VypisyDoKonzole.VypisUspech("Partner byl odebrán.");
        else
            VypisyDoKonzole.VypisVarovani("Partnera se nepodařilo odebrat.");
    }

    public static void NastavitCislovaniZamestnancu(SpravceZamestnancu spravceZamestnancu)
    {
        Console.WriteLine("NASTAVENÍ ČÍSLOVÁNÍ ZAMĚSTNANCŮ");
    
        var aktualni = spravceZamestnancu.VratNastaveniCislovani();
        string aktualniUkazka = $"{aktualni.Prefix}{(aktualni.PosledniCislo + 1).ToString().PadLeft(aktualni.PocetCislic, '0')}";
        Console.WriteLine($"Aktuální ukázka dalšího čísla: {aktualniUkazka}");
    
        try
        {
            string prefix = UpravaVstupu.ZeptejSeAUprav(
                aktualni.Prefix,
                "prefix (může být prázdný)",
                v => v,
                s => s); 
    
            int pocetCislic = UpravaVstupu.ZeptejSeAUprav(
                aktualni.PocetCislic,
                "počet číslic",
                v => v.ToString(),
                s => int.Parse(s));
    
            long posledniCislo = UpravaVstupu.ZeptejSeAUprav(
                aktualni.PosledniCislo,
                "poslední vydané číslo (další zaměstnanec dostane o 1 vyšší)",
                v => v.ToString(),
                s => long.Parse(s));
    
            NastaveniCislovaniZamestnancuDto dto = new()
            {
                Prefix = prefix,
                PocetCislic = pocetCislic,
                PosledniCislo = posledniCislo
            };
    
            string dalsiCislo = spravceZamestnancu.UlozNastaveniCislovani(dto);
            VypisyDoKonzole.VypisUspech($"Nastavení uloženo. Další vydané číslo bude: {dalsiCislo}");
        }
        catch (Exception ex)
        {
            VypisyDoKonzole.VypisVarovani($"Chyba při ukládání nastavení číslování: {ex.Message}");
        }
    }
}