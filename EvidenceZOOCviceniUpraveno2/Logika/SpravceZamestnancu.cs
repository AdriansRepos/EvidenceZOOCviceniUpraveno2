using EvidenceZOOCviceniUpraveno2.Data;
using EvidenceZOOCviceniUpraveno2.DTO.ZamestnanciDTO;
using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Enumy;

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
        /// Zadá nového zaměstnance a vytvoří mu osobní složku.
        /// </summary>
        public void Pridat(ZamestnanecVytvoreniDto dto)
        {
            string osobniCislo = zoo.Zamestnanci.CisloKonfigurace.DalsiCislo();
            zoo.Zamestnanci.UlozCisloKonfiguraci();
        
            Zamestnanec novy = new(
                osobniCislo,
                dto.Jmeno,
                dto.Prijmeni,
                dto.DatumNarozeni,
                dto.RodneCislo,
                dto.Mzda,
                dto.PracovniPozice,
                dto.Mesto,
                dto.Ulice,
                dto.PSC,
                dto.Telefon,
                dto.Email,
                dto.RodinnyStav,
                dto.ZdravotniStav,
                dto.TypDokladu,
                dto.CisloDokladu,
                deti: [],
                partner: null);
        
            bool uspech = Transakce.ProvedSUlozenim(
                akce: () => zoo.Zamestnanci.Zamestnanci.Add(novy),
                rollback: () => zoo.Zamestnanci.Zamestnanci.Remove(novy),
                ulozeni: zoo.Zamestnanci.Uloz,
                popisOperace: "přidání zaměstnance");
        
            if (uspech)
            {
                zoo.Zamestnanci.VytvorSlozkuZamestnance(novy);
        
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Zaměstnanci", TypAkce.Pridano, $"{novy.Jmeno} {novy.Prijmeni} ({novy.OsobniCislo})",
                    novaHodnota: $"mzda {novy.Mzda} Kč"));
            }
        }

        /// <summary>
        /// Metody pro práci s dítětem tak, aby se nenačítali hned, pokud není dítě zadáno.
        /// </summary>
        public bool PridatDite(DiteVytvoreniDto dto)
        {
            var zam = zoo.Zamestnanci.Zamestnanci
                .FirstOrDefault(z => z.OsobniCislo == dto.OsobniCisloZamestnance);
        
            if (zam == null)
                return false;
        
            Dite noveDite = new(
                dto.Jmeno,
                dto.Prijmeni,
                dto.DatumNarozeni,
                dto.RodneCislo,
                dto.JeDrzitelZtpP,
                dto.UplatnenBonus);
        
            zam.Deti.Add(noveDite);
            zoo.Zamestnanci.Uloz();
        
            return true;
        }

        public bool UpravitDite(DiteUpravaDto dto)
        {
            var zam = zoo.Zamestnanci.Zamestnanci
                .FirstOrDefault(z => z.OsobniCislo == dto.OsobniCisloZamestnance);
        
            if (zam == null)
                return false;
        
            var dite = zam.Deti
                .FirstOrDefault(d => d.RodneCislo == dto.PuvodniRodneCislo);
        
            if (dite == null)
                return false;
        
            dite.Jmeno = dto.Jmeno;
            dite.Prijmeni = dto.Prijmeni;
            dite.DatumNarozeni = dto.DatumNarozeni;
            dite.RodneCislo = dto.RodneCislo;
            dite.JeDrzitelZtpP = dto.JeDrzitelZtpP;
            dite.UplatnenBonus = dto.UplatnenBonus;
        
            zoo.Zamestnanci.Uloz();
        
            return true;
        }

        public bool OdebratDite(string osobniCisloZamestnance, string rodneCisloDitete)
        {
            var zam = zoo.Zamestnanci.Zamestnanci
                .FirstOrDefault(z => z.OsobniCislo == osobniCisloZamestnance);
        
            if (zam == null)
                return false;
        
            var dite = zam.Deti
                .FirstOrDefault(d => d.RodneCislo == rodneCisloDitete);
        
            if (dite == null)
                return false;
        
            zam.Deti.Remove(dite);
            zoo.Zamestnanci.Uloz();
        
            return true;
        }

        /// <summary>
        /// Metody pro práci s údaji o partnerovi a daňových slevách.
        /// </summary>
        public bool UlozitPartnera(PartnerUlozeniDto dto)
        {
            var zam = zoo.Zamestnanci.Zamestnanci
                .FirstOrDefault(z => z.OsobniCislo == dto.OsobniCisloZamestnance);
        
            if (zam == null)
                return false;
        
            zam.Partner = new Partner(
                dto.Jmeno,
                dto.Prijmeni,
                dto.RodneCislo,
                dto.JeDrzitelZtpP,
                dto.PecujeODiteDo3Let,
                dto.PrijemDo68Tisic,
                dto.UplatnitSlevu);
        
            zoo.Zamestnanci.Uloz();
            return true;
        }
        
        public bool OdebratPartnera(string osobniCisloZamestnance)
        {
            var zam = zoo.Zamestnanci.Zamestnanci
                .FirstOrDefault(z => z.OsobniCislo == osobniCisloZamestnance);
        
            if (zam == null || zam.Partner == null)
                return false;
        
            zam.Partner = null;
            zoo.Zamestnanci.Uloz();
            return true;
        }
                
        /// <summary>
        /// Umožňuje modifikaci libovolných osobních a mzdových údajů vybraného zaměstnance.
        /// </summary>
        public bool Upravit(ZamestnanecUpravaDto dto)
        {
            var zam = zoo.Zamestnanci.Zamestnanci
                .FirstOrDefault(z => z.OsobniCislo == dto.OsobniCislo);
        
            if (zam == null)
                return false;
        
            int puvodniMzda = zam.Mzda;
            string puvodniPrijmeni = zam.Prijmeni;
        
            zam.Jmeno = dto.Jmeno;
            zam.Prijmeni = dto.Prijmeni;
            zam.PracovniPozice = dto.PracovniPozice;
            zam.DatumNarozeni = dto.DatumNarozeni;
            zam.RodneCislo = dto.RodneCislo;
            zam.Mzda = dto.Mzda;
            zam.Mesto = dto.Mesto;
            zam.Ulice = dto.Ulice;
            zam.PSC = dto.PSC;
            zam.Telefon = dto.Telefon;
            zam.Email = dto.Email;
            zam.RodinnyStav = dto.RodinnyStav;
            zam.ZdravotniStav = dto.ZdravotniStav;
            zam.TypDokladu = dto.TypDokladu;
            zam.CisloDokladu = dto.CisloDokladu;
        
            zoo.Zamestnanci.Uloz();
        
            if (zam.Prijmeni != puvodniPrijmeni)
                zoo.Zamestnanci.PrejmenovatSlozkuZamestnance(zam, puvodniPrijmeni);
        
            if (zam.Mzda != puvodniMzda)
            {
                zoo.Logy.ZapisAudit(AuditZaznam.Vytvor(
                    "Zaměstnanci", TypAkce.Upraveno, $"{zam.Jmeno} {zam.Prijmeni} – mzda",
                    puvodniMzda.ToString(), zam.Mzda.ToString()));
            }
        
            return true;
        }

        private static ZamestnanecPrehledDto MapujNaPrehledDto(Zamestnanec zam)
        {
            return new ZamestnanecPrehledDto
            {
                OsobniCislo = zam.OsobniCislo,
                Jmeno = zam.Jmeno,
                Prijmeni = zam.Prijmeni,
                DatumNarozeni = zam.DatumNarozeni,
                RodneCislo = zam.RodneCislo,
                Mzda = zam.Mzda,
                PracovniPozice = zam.PracovniPozice,
                Mesto = zam.Mesto,
                Ulice = zam.Ulice,
                PSC = zam.PSC,
                Telefon = zam.Telefon,
                Email = zam.Email,
                RodinnyStav = zam.RodinnyStav,
                ZdravotniStav = zam.ZdravotniStav,
                TypDokladu = zam.TypDokladu,
                CisloDokladu = zam.CisloDokladu,
                Deti = [.. zam.Deti.Select(d => new DitePrehledDto
                {
                    Jmeno = d.Jmeno,
                    Prijmeni = d.Prijmeni,
                    DatumNarozeni = d.DatumNarozeni,
                    RodneCislo = d.RodneCislo,
                    JeDrzitelZtpP = d.JeDrzitelZtpP,
                    UplatnenBonus = d.UplatnenBonus
                })],
                Partner = zam.Partner == null
                    ? null
                    : new PartnerPrehledDto
                    {
                        Jmeno = zam.Partner.Jmeno,
                        Prijmeni = zam.Partner.Prijmeni,
                        RodneCislo = zam.Partner.RodneCislo,
                        JeDrzitelZtpP = zam.Partner.JeDrzitelZtpP,
                        PecujeODiteDo3Let = zam.Partner.PecujeODiteDo3Let,
                        PrijemDo68Tisic = zam.Partner.PrijemDo68Tisic,
                        UplatnitSlevu = zam.Partner.UplatnitSlevu
                    }
            };
        }

        /// <summary>
        /// Vrátí přehled všech evidovaných zaměstnanců.
        /// </summary>
        public List<ZamestnanecPrehledDto> VratPrehled()
        {
            return [.. zoo.Zamestnanci.Zamestnanci.Select(MapujNaPrehledDto)];
        }
        
        /// <summary>
        /// Vyhledá zaměstnance podle shody v příjmení bez ohledu na velikost písmen.
        /// </summary>
        public List<ZamestnanecPrehledDto> Vyhledat(string hledanePrijmeni)
        {
            if (string.IsNullOrWhiteSpace(hledanePrijmeni))
                return [];
        
            string hledany = hledanePrijmeni.Trim();
        
            return [.. zoo.Zamestnanci.Zamestnanci
                .Where(z => z.Prijmeni.Contains(hledany, StringComparison.CurrentCultureIgnoreCase))
                .Select(MapujNaPrehledDto)];
        }
        
        /// <summary>
        /// Odstraní zaměstnance podle osobního čísla z evidence v paměti i z disku.
        /// </summary>
        public bool Smazat(string osobniCislo)
        {
            var zam = zoo.Zamestnanci.Zamestnanci
                .FirstOrDefault(z => z.OsobniCislo == osobniCislo);
        
            if (zam == null)
                return false;
        
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
            }
        
            return uspech;
        }

        /// <summary>
        /// Vrátí aktuální nastavení číslování zaměstnanců.
        /// </summary>
        public NastaveniCislovaniZamestnancuDto VratNastaveniCislovani()
        {
            var konfigurace = zoo.Zamestnanci.CisloKonfigurace;

            return new NastaveniCislovaniZamestnancuDto
            {
                Prefix = konfigurace.Prefix,
                PocetCislic = konfigurace.PocetCislic,
                PosledniCislo = konfigurace.PosledniCislo
            };
        }

        /// <summary>
        /// Uloží nový formát osobního čísla a vrátí ukázku dalšího vydaného čísla.
        /// </summary>
        public string UlozNastaveniCislovani(NastaveniCislovaniZamestnancuDto dto)
        {
            if (dto.PocetCislic is < 1 or > 20)
                throw new ArgumentOutOfRangeException(nameof(dto), "Počet číslic musí být v rozsahu 1 až 20.");

            if (dto.PosledniCislo < 0)
                throw new ArgumentOutOfRangeException(nameof(dto), "Poslední vydané číslo nemůže být záporné.");

            var konfigurace = zoo.Zamestnanci.CisloKonfigurace;
            konfigurace.Prefix = dto.Prefix ?? string.Empty;
            konfigurace.PocetCislic = dto.PocetCislic;
            konfigurace.PosledniCislo = dto.PosledniCislo;

            zoo.Zamestnanci.UlozCisloKonfiguraci();

            return FormatujDalsiCislo(konfigurace);
        }

        private static string FormatujDalsiCislo(CisloZamestnanceKonfigurace konfigurace)
        {
            return $"{konfigurace.Prefix}{(konfigurace.PosledniCislo + 1).ToString().PadLeft(konfigurace.PocetCislic, '0')}";
        }
    }
}    