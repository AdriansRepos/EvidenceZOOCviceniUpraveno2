using EvidenceZOOCviceniUpraveno2.DTO.ZvirataDTO;
using EvidenceZOOCviceniUpraveno2.Entity;

namespace EvidenceZOOCviceniUpraveno2.Logika
{
    /// <summary>
    /// Třída zodpovědná za správu zvířat – přidávání, mazání, úpravy,
    /// čtení a vyhledávání. Pracuje s daty uloženými v instanci <see cref="Zoo"/>.
    /// </summary>
    class SpravceZvirat
    {
        private readonly Zoo zoo;

        public SpravceZvirat(Zoo zoo)
        {
            this.zoo = zoo;
            this.zoo.Zvirata.Nacti(); // Načtení evidence zvířat
        }

        /// <summary>
        /// Vrátí kolekci všech zvířat v evidenci.
        /// </summary>
        public IReadOnlyList<Zvire> ZiskejZvirata()
        {
            return zoo.Zvirata.Zvirata;
        }

        /// <summary>
        /// Vrátí přehled všech zvířat pro výpis.
        /// </summary>
        public List<ZvirePrehledDto> ZiskejPrehled()
        {
            return [.. zoo.Zvirata.Zvirata
                .Select(z => new ZvirePrehledDto
                {
                    Nazev = z.Nazev,
                    Vek = z.Vek,
                    Vaha = z.Vaha
                })];
        }

        /// <summary>
        /// Přidá nové zvíře, vygeneruje pro něj ID a uloží samostatný soubor zvířete v rámci transakce.
        /// </summary>
        public bool Pridat(NoveZvireDto dto)
        {
            // Vygenerování nového ID zvířete podle konfigurace
            string id = zoo.Zvirata.CisloKonfigurace.DalsiId();
            zoo.Zvirata.UlozCisloKonfiguraci();

            List<ZdravotniZaznam> zdravotniZaznamy = [];
            Zvire nove = new(
                dto.Nazev,
                dto.DatumNarozeni,
                dto.Vaha,
                dto.DatumPrijetiZJineZoo,
                zdravotniZaznamy,
                id);

            return Transakce.ProvedSUlozenim(
                akce: () => zoo.Zvirata.Zvirata.Add(nove),
                rollback: () => zoo.Zvirata.Zvirata.Remove(nove),
                ulozeni: () => zoo.Zvirata.UlozZvire(nove),
                popisOperace: $"přidání zvířete '{nove.Nazev}'"
            );
        }

        /// <summary>
        /// Odstraní zvíře z evidence a smaže jeho soubor na disku v rámci transakce.
        /// </summary>
        public bool Smazat(Zvire? zvire)
        {
            if (zvire == null)
                return false;

            int index = zoo.Zvirata.Zvirata.IndexOf(zvire);
            if (index < 0)
                return false;

            return Transakce.ProvedSUlozenim(
                akce: () => zoo.Zvirata.Zvirata.RemoveAt(index),
                rollback: () => zoo.Zvirata.Zvirata.Insert(index, zvire),
                ulozeni: () => zoo.Zvirata.SmazatZvireSoubor(zvire),
                popisOperace: $"smazání zvířete '{zvire.Nazev}'"
            );
        }

        /// <summary>
        /// Upraví základní údaje o zvířeti (název, narození, váha) a uloží změny v rámci transakce.
        /// </summary>
        public bool Upravit(Zvire? zvire, UpravaZvireteDto dto)
        {
            if (zvire == null)
                return false;

            // Uložení původních hodnot a názvu souboru pro případný rollback i přejmenování
            string puvodniSoubor = zoo.Zvirata.SouborZvirete(zvire);
            string puvodniNazev = zvire.Nazev;
            DateOnly puvodniDatumNarozeni = zvire.DatumNarozeni;
            double puvodniVaha = zvire.Vaha;

            return Transakce.ProvedSUlozenim(
                akce: () =>
                {
                    zvire.Nazev = dto.Nazev;
                    zvire.DatumNarozeni = dto.DatumNarozeni;
                    zvire.Vaha = dto.Vaha;
                },
                rollback: () =>
                {
                    zvire.Nazev = puvodniNazev;
                    zvire.DatumNarozeni = puvodniDatumNarozeni;
                    zvire.Vaha = puvodniVaha;
                },
                ulozeni: () => zoo.Zvirata.UlozZvire(zvire, puvodniSoubor),
                popisOperace: $"úprava zvířete '{puvodniNazev}'"
            );
        }

        /// <summary>
        /// Vyhledá zvířata dle zadání textového řetězce v názvu.
        /// </summary>
        public List<ZvirePrehledDto> Vyhledat(string hledanyVyraz)
        {
            string hledany = hledanyVyraz.Trim().ToLower();

            return [.. zoo.Zvirata.Zvirata
                .Where(z => z.Nazev.Contains(hledany, StringComparison.CurrentCultureIgnoreCase))
                .Select(z => new ZvirePrehledDto
                {
                    Nazev = z.Nazev,
                    Vek = z.Vek,
                    Vaha = z.Vaha
                })];
        }
    }
}