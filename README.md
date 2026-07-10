# ZOO – Konzolová aplikace v C#

Tento projekt je jednoduchá, ale plně funkční konzolová aplikace pro správu ZOO.  
Umožňuje evidovat **zvířata**, **zaměstnance**, **sklad**, provádět **statistiky**, ukládat data do souborů a pracovat s uživatelskými vstupy.

---

## Funkce aplikace

### Zvířata

- Přidání nového zvířete  
- Výpis všech zvířat  
- Úprava existujícího zvířete
- Smazání zvířete
- Vyhledávání podle názvu
- Automatický výpočet aktuálního věku z data narození (roky, měsíce)
- Statistiky (průměrná váha, počet druhů…)

### Zaměstnanci

- Přidání zaměstnance
- Úprava zaměstnance
- Smazání zaměstnance
- Výpis všech zaměstnanců
- Vyhledávání podle příjmení
- Ukládání do souboru + automatická záloha

### Sklad

- Evidence položek ve třech kategoriích: krmivo, pomůcky, léky/veterinární materiál
- Přidání nové položky (název, kategorie, množství, jednotka, minimální stav)
- Naskladnění a vyskladnění s kontrolou proti zápornému množství
- Upozornění na docházející položky (pokles na/pod minimální stav)
- Historie všech skladových pohybů (naskladnění/vyskladnění, množství, datum a čas)
- Výpis aktuálního stavu skladu i docházejících položek zvlášť

---

## Ukládání a zabezpečení dat

Aplikace ukládá data do souborů JSON:

- `zamestnanci.json` + `zamestnanci.json.bak` (automatická záloha)
- `zvirata.json` + `zvirata.json.bak` (automatická záloha)
- `sklad.json` + `sklad.json.bak` (automatická záloha)
- `sklad_historie.json` + `sklad_historie.json.bak` (automatická záloha)
- `config.ini` + `config.ini.bak` (cesty k datovým souborům a stav roční archivace, nastavené při prvním spuštění)

**Bezpečný zápis (crash-safe)**  
Ukládání neprobíhá přímým přepsáním souboru, ale přes dočasný soubor (`.tmp`) a atomickou náhradu (`File.Replace`). Díky tomu nemůže při pádu aplikace nebo výpadku uprostřed zápisu dojít k poškození nebo ztrátě posledního platného souboru – buď se zápis provede celý, nebo se nic nezmění a zůstane zachovaná předchozí verze.

**Automatická obnova**  
Pokud je při načítání hlavní soubor (zaměstnanci, zvířata, sklad, historie skladu nebo `config.ini`) poškozený nebo chybí, aplikace se automaticky pokusí obnovit data ze zálohy `.bak`. Uživatel je o průběhu informován barevně odlišenými hláškami v konzoli (zelená = úspěch, červená = chyba/varování).

**Roční archivace dat**  
Datové soubory (zaměstnanci, zvířata, sklad, historie skladu) jsou platné vždy jen do konce kalendářního roku, bez ohledu na to, kdy byly založeny. Při každém spuštění aplikace se zkontroluje, zda uplynul kalendářní rok od poslední archivace:

- Pokud ano, vytvoří se kopie aktuálních souborů (např. `zamestnanci_2026.json`) do složky `Archiv` uvnitř datové složky
- Pracovní soubory zůstávají nedotčené a aplikace v nich pokračuje dál i po přelomu roku
- Archivní soubory starší než 5 let jsou automaticky odstraněny
- Stav poslední provedené archivace se ukládá do `config.ini`, takže se archivace neprovede vícekrát za stejný rok

---

## Struktura projektu

/ZOO

├── Program.cs

├── ZOO.cs

├── Zamestnanec.cs

├── Zvire.cs

├── SpravceZamestnancu.cs

├── SpravceZvirat.cs

└── README.md

---

### Externí knihovny (samostatné projekty)

Část funkčnosti byla vyčleněna do samostatných, znovupoužitelných knihoven, aby šly nezávisle referencovat i v jiných projektech:

- **TextHelper** – zpracování a validace uživatelských vstupů (`UpravaVstupu.ZeptejSeAUprav`)
- **SelectHelper** – výběr položky ze seznamu (`SelectHelp.VybratPolozku`)
- **InputHelper** – převod textu na TitleCase (`TitleCase.ToTitleCase`)
- **DateConverterForJson** – JSON konvertor pro typ `DateOnly` (`DateOnlyConverter`)
- **VekZviratHelper** – výpočet aktuálního věku zvířete z data narození (`VypocetVeku.VypocitejVek`, `VypocetVeku.VypocitejVekTextove`), včetně skloňování slov "rok"/"měsíc"

---

## Použité techniky

- OOP (třídy, vlastnosti, zapouzdření)
- Primary constructors
- Enumy (kategorie skladových položek, typ skladového pohybu)
- Delegáty pro lazy loading dat (`RegistrujNacitani` / `ZajistiData`)
- Práce se soubory (`File.ReadAllText`, `File.WriteAllText`, `File.Copy`, `File.Replace`, `File.Move`, `File.Delete`)
- Crash-safe zápis přes dočasný soubor a atomickou náhradu
- Automatická obnova dat ze zálohy při poškození nebo ztrátě souboru
- Automatizovaná roční archivace dat s retenční politikou (uchování 5 let)
- Validace vstupů
- Try-catch bloky
- DateOnly, DateTime
- Dynamicky počítané vlastnosti (výpočet věku a stavu "dochází" za běhu, bez ukládání zastaralé hodnoty)
- LINQ (`Where`, `OrderByDescending`, `Sum`)
- Kolekce (`List<T>`)
- Serializace/deserializace do JSON souborů (`System.Text.Json`)
- Zadání cest k adresáři a souborům jenom při prvním spuštění aplikace – nastavení a jejich uložení do konfiguračního souboru
- Lazy loading pro načtení souborů až když je potřeba
- Barevný konzolový výstup pro přehlednost (úspěch/chyba/varování/menu)
- Úprava formátování výpisů do tabulek

---

## Ukázka kódu

```csharp
public void Pridat()
{
    Console.WriteLine("ZADÁNÍ NOVÉHO ZAMĚSTNANCE");
    string jmeno = UpravaVstupu.ZeptejSeAUprav(
        "", "jméno",
        v => v,
        s => s,
        jeNove: true);

    string prijmeni = UpravaVstupu.ZeptejSeAUprav(
        "", "příjmení",
        v => v,
        s => s,
        jeNove: true);

    string pracovniPozice = UpravaVstupu.ZeptejSeAUprav(
        "", "pracovní pozice",
        v => v,
        s => s,
        jeNove: true);

    DateOnly datumNarozeni = UpravaVstupu.ZeptejSeAUprav(
        DateOnly.MinValue, "datum narození",
        v => v.ToString(),
        s => DateOnly.Parse(s),
        jeNove: true);

    int mzda = UpravaVstupu.ZeptejSeAUprav(
        0, "mzda",
        v => v.ToString(),
        s => int.Parse(s),
        jeNove: true);

    zoo.Zamestnanci.Add(new Zamestnanec(jmeno, prijmeni, datumNarozeni, mzda, pracovniPozice));
    zoo.UlozZamestnance();

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Zaměstnanec byl úspěšně přidán.");
    Console.ResetColor();
}
```

---

## Plánované funkce

- Základ účetnictví
- Mzdové účetnictví
- Rozšíření evidence zaměstnanců o další údaje
- Přidání transakčního ukládání
- Přidání logování změn
- Export statistik
- Automatické testy
- GUI verze (WPF nebo MAUI)

---

## Release

**v1.0.0** – První vydaná spustitelná verze  
[Stáhnout zde](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2/releases)

**v1.1.0** – Spustitelná verze se zapracovanými novými features  
[Stáhnout zde](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2/releases/tag/v1.1.0)

**v1.2.0** – Úprava kódu, odebrání tříd `Vstupy` a `DataConverterJson`; funkce přesunuty do samostatných knihoven a zapracovány zpět do aplikace  
[Stáhnout zde](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2/releases/tag/v1.2.0)

**v1.3.0** – Barevné odlišení výstupů v konzoli – zelená pro úspěšné akce, červená pro chyby a varování, modrá pro menu, žlutá pro neplatné volby. Oprava chyby ve vyhledávání (chybějící složené závorky u podmínky) způsobující nesprávné zobrazení hlášky "nenalezeno".  
[Stáhnout zde](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2/releases/tag/v1.3.0)

**v1.4.0** – Přidána nová knihovna `VekZviratHelper` pro automatický výpočet věku zvířat. Třída `Zvire` nahrazuje ruční zadávání věku (`int Vek`) datem narození (`DateOnly DatumNarozeni`) – věk se nyní počítá dynamicky při každém zobrazení a je vždy aktuální bez nutnosti ruční aktualizace.  
[Stáhnout zde](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2/releases/tag/v1.4.0)

**v1.5.0** – Přidán modul Sklad (krmivo, pomůcky, léky/veterinární materiál) včetně naskladnění, vyskladnění, upozornění na docházející položky a historie pohybů. Přidána automatizovaná roční archivace všech datových souborů do složky `Archiv` s retenční politikou 5 let.