using EvidenceZOOCviceniUpraveno2.Data.Repozitare;
using EvidenceZOOCviceniUpraveno2.Entity;
using EvidenceZOOCviceniUpraveno2.Logika;
using EvidenceZOOCviceniUpraveno2.Menu;
using EvidenceZOOCviceniUpraveno2.Zaloha;

string korenovaSlozka = KonfiguraceRepository.NactiNeboSeZeptejNaCesty();
Zoo zoo = new(korenovaSlozka);

// Zkontroluje a případně provede roční archivaci – musí proběhnout
// hned po vytvoření zoo, ještě před prvním použitím dat.
new ArchivacniSluzba(zoo).ZkontrolujRocniArchivaci();
zoo.Konfigurace.NactiExterniZalohuCestu();

zoo.Zamestnanci.Nacti();
zoo.Zvirata.Nacti();
zoo.Sklad.Nacti();
zoo.Pokladna.Nacti();
zoo.Logy.Nacti();

SpravceZamestnancu spravceZamestnancu = new(zoo);
SpravceZvirat spravceZvirat = new(zoo);
SpravceSkladu spravceSkladu = new(zoo);
SpravcePokladny spravcePokladny = new(zoo);
Statistiky statistiky = new(zoo);

MenuZamestnanci menuZamestnanci = new(spravceZamestnancu);
MenuZvirata menuZvirata = new(spravceZvirat);
MenuSklad menuSklad = new(spravceSkladu);
MenuPokladna menuPokladna = new(spravcePokladny);
MenuStatistiky menuStatistiky = new(statistiky);

// === SPUŠTĚNÍ APLIKACE ===
HlavniMenu hlavniMenu = new(zoo, menuZamestnanci, menuZvirata, menuSklad, menuPokladna, menuStatistiky);
hlavniMenu.Zobraz();