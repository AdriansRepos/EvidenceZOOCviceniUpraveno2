using EvidenceZOOCviceniUpraveno2.Logika;
using EvidenceZOOCviceniUpraveno2.Zaloha;
using EvidenceZOOCviceniUpraveno2.Data.Repozitare;
using EvidenceZOOCviceniUpraveno2.Entity;

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

// není ukončený a doladěný - dokončení a odladění proběhne na konci refaktoringu,
// kdy budu kontrolovat i jednotlivá provázání namespace detailněji