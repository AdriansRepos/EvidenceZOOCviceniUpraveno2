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
        /// <summary> Přístup k datům konfigurací ceníků a číselníků. </summary>
        public KonfiguraceRepository Konfigurace { get; } = new KonfiguraceRepository(korenovaSlozka);

        /// <summary> Přístup k evidenci zaměstnanců a mzdové agendě. </summary>
        public ZamestnanciRepository Zamestnanci { get; } = new ZamestnanciRepository(korenovaSlozka);

        /// <summary> Přístup k evidenci zvířat a kartám zdravotního stavu. </summary>
        public ZvirataRepository Zvirata { get; } = new ZvirataRepository(korenovaSlozka);

        /// <summary> Přístup k evidenční kartě skladu a logům skladových pohybů. </summary>
        public SkladRepository Sklad { get; } = new SkladRepository(korenovaSlozka);

        /// <summary> Přístup k pokladním pohybům a tržbám za vstupné. </summary>
        public PokladnaRepository Pokladna { get; } = new PokladnaRepository(korenovaSlozka);

        /// <summary> Přístup k auditním a technickým systémovým logům. </summary>
        public LogyRepository Logy { get; } = new LogyRepository(korenovaSlozka);
    }
}