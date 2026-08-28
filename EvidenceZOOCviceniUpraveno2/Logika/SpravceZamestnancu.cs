using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.Data;
using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Enumy;
using EvidenceZOOCviceniUpraveno2.Vypisy.Zamestnanci;
using InputHelper;
using PohybHelper;
using SelectHelper;
using TextHelper;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    /// <summary>
    /// Zajišťuje správu evidovaných zaměstnanců, vytváření personalistiky a nastavení číselníku.
    /// </summary>
    class SpravceZamestnancu
    {
        private readonly Zoo zoo;

        public SpravceZamestnancu(Zoo zoo)
        {
            this.zoo = zoo;
            this.zoo.Zamestnanci.Nacti(); // Načtení dat zaměstnanců
        }

        /// <summary>
        /// Zadá nového zaměstnance včetně jeho rodinných příslušníků (děti, partner) a vytvoří mu osobní složku.
        /// </summary>
        public void Pridat()
        {
            Console.WriteLine("ZADÁNÍ NOVÉHO ZAMĚSTNANCE");

            // Vygenerování nového osobního čísla dle konfigurace číselníku
            string osobniCislo = zoo.Zamestnanci.CisloKonfigurace.DalsiCislo();
            zoo.Zamestnanci.UlozCisloKonfiguraci();

            // Načtení osobních a kontaktních údajů z konzole
            string jmeno = UpravaVstupu.ZeptejSeAUprav("", "jméno", v => v, s => s, jeNove: true);
            string prijmeni = UpravaVstupu.ZeptejSeAUprav("", "příjmení", v => v, s => s, jeNove: true);
            string pracovniPozice = UpravaVstupu.ZeptejSeAUprav("", "pracovní pozice", v => v, s => s, jeNove: true);

            DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                DateOnly.MinValue, "datum narození", v => v.ToString(), s => DateOnly.Parse(s), jeNove: true);

            string rodneCislo = UpravaVstupu.ZeptejSeAUprav(
                "", "rodné číslo (např. 900101/1234)", v => v, s => s, jeNove: true);

            int mzda = UpravaVstupu.ZeptejSeAUprav(
                0, "mzda", v => v.ToString(), s => int.Parse(s), jeNove: true);

            string mesto = UpravaVstupu.ZeptejSeAUprav("", "město", v => v, s => s, jeNove: true);
            string ulice = UpravaVstupu.ZeptejSeAUprav("", "ulice a číslo popisné", v => v, s => s, jeNove: true);
            string psc = UpravaVstupu.ZeptejSeAUprav("", "PSČ", v => v, s => s, jeNove: true);
            string telefon = UpravaVstupu.ZeptejSeAUprav("", "telefonní číslo", v => v, s => s, jeNove: true);

            // Načtení e-mailu s ověřením platného formátu
            string email = UpravaVstupu.ZeptejSeAUprav(
                "", "e-mail", v => v,
                s =>
                {
                    if (!Zamestnanec.JePlatnyEmail(s))
                        throw new FormatException("E-mail nemá platný formát (očekává se např. jmeno@domena.cz).");
                    return s;
                },
                jeNove: true);

            Console.WriteLine("Rodinný stav:");
            RodinnyStav rodinnyStav = VyberEnum<RodinnyStav>();

            Console.WriteLine("Zdravotní stav:");
            ZdravotniStav zdravotniStav = VyberEnum<ZdravotniStav>();

            string typDokladu = UpravaVstupu.ZeptejSeAUprav("", "typ dokladu (OP, pas...)", v => v, s => s, jeNove: true);
            string cisloDokladu = UpravaVstupu.ZeptejSeAUprav("", "číslo dokladu", v => v, s => s, jeNove: true);

            // Zadaní vedlejších entit (děti, partner) pro daňové účely
            List<Dite> deti = ZadatDeti();
            Partner? partner = ZadatPartnera();

            Zamestnanec novy = new(osobniCislo, jmeno, prijmeni, datumNarozeni, rodneCislo, mzda, pracovniPozice,
                mesto, ulice, psc, telefon, email, rodinnyStav, zdravotniStav, typDokladu, cisloDokladu, deti, partner);

            // Transakční uložení záznamu
            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Zamestnanci.Zamestnanci.Add(novy),
                rollback: () => zoo.Zamestnanci.Zamestnanci.Remove(novy),
                ulozeni: zoo.Zamestnanci.Uloz,
                popisOperace: "přidání zaměstnance");

            if (uspech)
            {
                // Vytvoření podřízené fyzické složky na disku pro dokumenty zaměstnance
                zoo.Zamestnanci.VytvorSlozkuZamestnance(novy);

                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Zaměstnanci", TypAkce.Pridano, $"{jmeno} {prijmeni} ({osobniCislo})",
                    novaHodnota: $"mzda {mzda} Kč"));

                VypisyDoKonzole.VypisUspech("Zaměstnanec byl úspěšně přidán.");
            }
        }

        /// <summary>
        /// Generická pomocná metoda pro výběr hodnoty výčtového typu ze seznamu v konzoli.
        /// </summary>
        private static TEnum VyberEnum<TEnum>() where TEnum : struct, Enum
        {
            var hodnoty = Enum.GetValues<TEnum>();

            for (int i = 0; i < hodnoty.Length; i++)
                Console.WriteLine($"\t{i + 1}. {PopiskyHelper.ZiskejPopisek(hodnoty[i])}");

            while (true)
            {
                Console.Write("Vyber možnost: ");
                if (int.TryParse(Console.ReadLine(), out int volba) && volba >= 1 && volba <= hodnoty.Length)
                    return hodnoty[volba - 1];

                VypisyDoKonzole.VypisInformaci("Neplatná volba, zkus to znovu.");
            }
        }

        /// <summary>
        /// Pomocný dialog pro zadání seznamu dětí daného zaměstnance.
        /// </summary>
        private static List<Dite> ZadatDeti()
        {
            List<Dite> deti = [];

            Console.Write("Počet dětí: ");
            int pocetDeti = int.TryParse(Console.ReadLine(), out int p) ? p : 0;

            for (int i = 0; i < pocetDeti; i++)
            {
                Console.WriteLine($"--- Dítě {i + 1} ---");
                string jmenoDite = UpravaVstupu.ZeptejSeAUprav("", "jméno dítěte", v => v, s => s, jeNove: true);
                string prijmeniDite = UpravaVstupu.ZeptejSeAUprav("", "příjmení dítěte", v => v, s => s, jeNove: true);

                DateOnly datumNarozeniDite = UpravaVstupu.ZeptejSeAUprav(
                    DateOnly.MinValue, "datum narození dítěte", v => v.ToString(), s => DateOnly.Parse(s), jeNove: true);

                string rodneCisloDite = UpravaVstupu.ZeptejSeAUprav(
                    "", "rodné číslo dítěte (např. 1101011234)", v => v, s => s, jeNove: true);

                bool jeDrzitelZtpP = UpravaVstupu.ZeptejSeAUprav(
                    false, "držitel průkazu ZTP/P (ano/ne)", v => v ? "ano" : "ne",
                    s => s.Trim().Equals("ano", StringComparison.OrdinalIgnoreCase), jeNove: true);

                bool uplatnenBonus = UpravaVstupu.ZeptejSeAUprav(
                    false, "uplatnit daňový bonus na dítě (ano/ne)", v => v ? "ano" : "ne",
                    s => s.Trim().Equals("ano", StringComparison.OrdinalIgnoreCase), jeNove: true);

                deti.Add(new Dite(jmenoDite, prijmeniDite, datumNarozeniDite, rodneCisloDite, jeDrzitelZtpP, uplatnenBonus));
            }
            return deti;
        }

        /// <summary>
        /// Pomocný dialog pro zadání údajů o partnerovi a daňových slevách.
        /// </summary>
        private static Partner? ZadatPartnera()
        {
            Console.WriteLine("Má zaměstnanec partnera/manžela/manželku pro uplatnění slevy? A/N");
            if (!Console.ReadLine()!.Equals("A", StringComparison.OrdinalIgnoreCase))
                return null;
        
            string jmeno = UpravaVstupu.ZeptejSeAUprav("", "jméno partnera", v => v, s => s, jeNove: true);
            string prijmeni = UpravaVstupu.ZeptejSeAUprav("", "příjmení partnera", v => v, s => s, jeNove: true);
            string rc = UpravaVstupu.ZeptejSeAUprav("", "rodné číslo partnera", v => v, s => InputHelper.RodneCislo.Zkontroluj(s), jeNove: true);
            
            Console.WriteLine("Je partner držitelem průkazu ZTP/P? A/N");
            bool ztp = Console.ReadLine()!.Equals("A", StringComparison.OrdinalIgnoreCase);
        
            Console.WriteLine("Pečuje partner o společné dítě do 3 let? A/N");
            bool diteDo3 = Console.ReadLine()!.Equals("A", StringComparison.OrdinalIgnoreCase);
        
            Console.WriteLine("Jsou vlastní příjmy partnera do 68 000 Kč / rok? A/N");
            bool prijemDo68 = Console.ReadLine()!.Equals("A", StringComparison.OrdinalIgnoreCase);
        
            Console.WriteLine("Uplatnit slevu na partnera v ročním zúčtování? A/N");
            bool uplatnit = Console.ReadLine()!.Equals("A", StringComparison.OrdinalIgnoreCase);
        
            return new Partner(jmeno, prijmeni, rc, ztp, diteDo3, prijemDo68, uplatnit);
        }

        /// <summary>
        /// Vypíše seznam všech zaměstnanců včetně kontaktních údajů.
        /// </summary>
        public void Vypis()
        {
            ZamestnanciVypisy.VypisHlavicku();

            foreach (var zam in zoo.Zamestnanci.Zamestnanci)
            {
                ZamestnanciVypisy.VypisZamestnance(zam);
                ZamestnanciVypisy.VypisKontaktniUdaje(zam);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Umožňuje modifikaci libovolných osobních a mzdových údajů vybraného zaměstnance.
        /// </summary>
        public void Upravit()
        {
            Console.WriteLine("ÚPRAVA ZAMĚSTNANCE");
            var zam = SelectHelp.VybratPolozku(zoo.Zamestnanci.Zamestnanci, z => z.Prijmeni, "zaměstnance");
            if (zam == null) 
                return;

            int puvodniMzda = zam.Mzda;
            string puvodniPrijmeni = zam.Prijmeni;

            // Úprava vlastností
            zam.Jmeno = UpravaVstupu.ZeptejSeAUprav(zam.Jmeno, "jméno", v => v, s => s);
            zam.Prijmeni = UpravaVstupu.ZeptejSeAUprav(zam.Prijmeni, "příjmení", v => v, s => s);
            zam.PracovniPozice = UpravaVstupu.ZeptejSeAUprav(zam.PracovniPozice, "pracovní pozice", v => v, s => s);

            zam.DatumNarozeni = UpravaVstupu.ZeptejSeAUprav(
                zam.DatumNarozeni, "datum narození", v => v.ToString(), s => DateOnly.Parse(s));

            zam.RodneCislo = UpravaVstupu.ZeptejSeAUprav(
                zam.RodneCislo, "rodné číslo (např. 900101/1234)", v => v, s => RodneCislo.Zkontroluj(s));

            zam.Mzda = UpravaVstupu.ZeptejSeAUprav(zam.Mzda, "mzda", v => v.ToString(), s => int.Parse(s));

            zam.Mesto = UpravaVstupu.ZeptejSeAUprav(zam.Mesto, "město", v => v, s => s);
            zam.Ulice = UpravaVstupu.ZeptejSeAUprav(zam.Ulice, "ulice a číslo popisné", v => v, s => s);
            zam.PSC = UpravaVstupu.ZeptejSeAUprav(zam.PSC, "PSČ", v => v, s => s);
            zam.Telefon = UpravaVstupu.ZeptejSeAUprav(zam.Telefon, "telefonní číslo", v => v, s => s);

            zam.Email = UpravaVstupu.ZeptejSeAUprav(
                zam.Email, "e-mail", v => v,
                s =>
                {
                    if (!Zamestnanec.JePlatnyEmail(s))
                        throw new FormatException("E-mail nemá platný formát (očekává se např. jmeno@domena.cz).");
                    return s;
                });

            Console.WriteLine("Upravit rodinný stav? A/N");
            if (Console.ReadLine()!.Equals("A", StringComparison.OrdinalIgnoreCase))
                zam.RodinnyStav = VyberEnum<RodinnyStav>();

            Console.WriteLine("Upravit zdravotní stav? A/N");
            if (Console.ReadLine()!.Equals("A", StringComparison.OrdinalIgnoreCase))
                zam.ZdravotniStav = VyberEnum<ZdravotniStav>();

            zam.TypDokladu = UpravaVstupu.ZeptejSeAUprav(zam.TypDokladu, "typ dokladu", v => v, s => s);
            zam.CisloDokladu = UpravaVstupu.ZeptejSeAUprav(zam.CisloDokladu, "číslo dokladu", v => v, s => s);

            Console.WriteLine("Upravit údaje o dětech (kompletně přepsat seznam)? A/N");
            if (Console.ReadLine()!.Equals("A", StringComparison.OrdinalIgnoreCase))
                zam.Deti = ZadatDeti();

            Console.WriteLine("Upravit údaje o partnerovi/manželovi? A/N");
            if (Console.ReadLine()!.Equals("A", StringComparison.OrdinalIgnoreCase))
                zam.Partner = ZadatPartnera();

            zoo.Zamestnanci.Uloz();

            // Při změně příjmení přejmenuje i jeho fyzickou složku na disku
            if (zam.Prijmeni != puvodniPrijmeni)
                zoo.Zamestnanci.PrejmenovatSlozkuZamestnance(zam, puvodniPrijmeni);

            // Auditování změn platu
            if (zam.Mzda != puvodniMzda)
            {
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Zaměstnanci", TypAkce.Upraveno, $"{zam.Jmeno} {zam.Prijmeni} – mzda",
                    puvodniMzda.ToString(), zam.Mzda.ToString()));
            }

            VypisyDoKonzole.VypisUspech("Úprava dokončena.");
        }

        /// <summary>
        /// Odstraní zaměstnance z evidence v paměti i z disku.
        /// </summary>
        public void Smazat()
        {
            Console.WriteLine("SMAZÁNÍ ZAMĚSTNANCE");
            var zam = SelectHelp.VybratPolozku(zoo.Zamestnanci.Zamestnanci, z => z.Prijmeni, "zaměstnance");
            if (zam == null) 
                return;

            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Zamestnanci.Zamestnanci.Remove(zam),
                rollback: () => zoo.Zamestnanci.Zamestnanci.Add(zam),
                ulozeni: zoo.Zamestnanci.Uloz,
                popisOperace: "smazání zaměstnance");

            if (uspech)
            {
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Zaměstnanci", TypAkce.Smazano, $"{zam.Jmeno} {zam.Prijmeni}",
                    puvodniHodnota: $"mzda {zam.Mzda} Kč"));

                VypisyDoKonzole.VypisInformaci($"Zaměstnanec {zam.Prijmeni} byl smazán.");
            }
        }

        /// <summary>
        /// Vyhledává zaměstnance podle shody v příjmení bez ohledu na velikost písmen.
        /// </summary>
        public void Vyhledat()
        {
            Console.Write("Zadejte hledané příjmení: ");
            string hledany = Console.ReadLine()!.Trim().ToLower();

            bool nalezeno = false;

            foreach (var zam in zoo.Zamestnanci.Zamestnanci)
            {
                if (zam.Prijmeni.Contains(hledany, StringComparison.CurrentCultureIgnoreCase))
                {
                    ZamestnanciVypisy.VypisZamestnance(zam);
                    nalezeno = true;
                }
            }
            if (!nalezeno)
            {
                VypisyDoKonzole.VypisVarovani("Zaměstnanec nenalezen.");
            }
        }

        /// <summary>
        /// Umožňuje nastavit formát osobního čísla (prefix, počáteční číslo, počet číslic).
        /// </summary>
        public void NastaveniCislovaniZamestnancu()
        {
            Console.WriteLine("NASTAVENÍ ČÍSLOVÁNÍ ZAMĚSTNANCŮ");

            var konfigurace = zoo.Zamestnanci.CisloKonfigurace;

            Console.WriteLine($"Aktuální ukázka dalšího čísla: {konfigurace.Prefix}{(konfigurace.PosledniCislo + 1).ToString().PadLeft(konfigurace.PocetCislic, '0')}");

            konfigurace.Prefix = UpravaVstupu.ZeptejSeAUprav(
                konfigurace.Prefix, "prefix (může být prázdný)", v => v, s => s);

            konfigurace.PocetCislic = UpravaVstupu.ZeptejSeAUprav(
                konfigurace.PocetCislic, "počet číslic", v => v.ToString(), s => int.Parse(s));

            konfigurace.PosledniCislo = UpravaVstupu.ZeptejSeAUprav(
                konfigurace.PosledniCislo, "poslední vydané číslo (další zaměstnanec dostane o 1 vyšší)",
                v => v.ToString(), s => long.Parse(s));

            zoo.Zamestnanci.UlozCisloKonfiguraci();

            VypisyDoKonzole.VypisUspech($"Nastavení uloženo. Další vydané číslo bude: {konfigurace.Prefix}{(konfigurace.PosledniCislo + 1).ToString().PadLeft(konfigurace.PocetCislic, '0')}");
        }
    }
}