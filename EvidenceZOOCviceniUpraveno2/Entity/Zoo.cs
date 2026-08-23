using EvidenceZOOCviceniUpraveno2.Data.Repozitare;

namespace EvidenceZOOCviceniUpraveno2.Entity
{
    /// <summary>
    /// Lehký kontejner držící instance jednotlivých repository tříd.
    /// Neobsahuje žádnou vlastní I/O logiku – jen skládá dohromady
    /// jednotlivé oblasti dat pro pohodlný přístup z komunikační vrstvy.
    /// </summary>
    internal class Zoo(string korenovaSlozka)
    {
        public KonfiguraceRepository Konfigurace { get; } = new KonfiguraceRepository(korenovaSlozka);
        public ZamestnanciRepository Zamestnanci { get; } = new ZamestnanciRepository(korenovaSlozka);
        public ZvirataRepository Zvirata { get; } = new ZvirataRepository(korenovaSlozka);
        public SkladRepository Sklad { get; } = new SkladRepository(korenovaSlozka);
        public PokladnaRepository Pokladna { get; } = new PokladnaRepository(korenovaSlozka);
        public LogyRepository Logy { get; } = new LogyRepository(korenovaSlozka);
    }
}