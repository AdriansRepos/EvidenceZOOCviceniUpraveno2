# ZOO – Konzolová aplikace v C#

Tento projekt je jednoduchá, ale plně funkční konzolová aplikace pro správu ZOO.  
Umožňuje evidovat **zvířata**, **zaměstnance**, **sklad**, **pokladnu**, provádět **statistiky**, ukládat data do souborů a pracovat s uživatelskými vstupy.

---

## Funkce aplikace

### Zvířata

- Přidání nového zvířete  
- Výpis všech zvířat  
- Úprava existujícího zvířete
- Smazání zvířete
- Vyhledávání podle názvu
- Automatický výpočet aktuálního věku z data narození (roky, měsíce)

### Zaměstnanci

- Přidání zaměstnance
- Úprava zaměstnance
- Smazání zaměstnance
- Výpis všech zaměstnanců, včetně adresních a kontaktních údajů
- Vyhledávání podle příjmení
- Evidence adresy (město, ulice s číslem, PSČ) a kontaktů (telefon, e-mail)
- Automatické velké písmena v názvu města s ohledem na české předložky (např. "Ústí nad Labem")
- Základní formátová validace e-mailové adresy při zadání/úpravě
- Ukládání do souboru + automatická záloha

### Sklad

- Evidence položek ve třech kategoriích: krmivo, pomůcky, léky/veterinární materiál
- Přidání nové položky (název, kategorie, množství, jednotka, minimální stav)
- Naskladnění a vyskladnění s kontrolou proti zápornému množství
- Upozornění na docházející položky (pokles na/pod minimální stav)
- Historie všech skladových pohybů (naskladnění/vyskladnění, množství, datum a čas)
- Výpis aktuálního stavu skladu i docházejících položek zvlášť

### Pokladna

- Prodej vstupenek: dětská, dospělá, ZTP, důchodce (65+), rodinná (úplná/neúplná), skupinová
- U rodinných a skupinových vstupenek ruční zadání počtu dětí, případně i dospělých
- Storno prodané vstupenky
- Konfigurovatelný ceník (základní ceny + procentuální slevy na rodinné a skupinové vstupenky), upravitelný za běhu aplikace
- Denní uzávěrka s rozpadem tržby podle kategorií vstupenek
- Filtrovaný výpis pohybů podle typu (prodej/storno) a rozmezí data – pro měsíční i roční uzávěrky

### Statistiky

- počet zvířat
- počet zaměstnanců
- součet mezd
- průměrná denní návštěvnost za zvolený měsíc

---

## Ukládání a zabezpečení dat

Aplikace ukládá data do dvou oddělených větví uvnitř uživatelem zvolené kořenové složky:

- `Data/` – aktuální pracovní soubory, rozdělené do podsložek podle modulu (Zamestnanci, Zvirata, Sklad, Účetnictví/Pokladna)
- `Zalohy/` – zálohy odpovídajících souborů se stejnou podsložkovou strukturou
- `Archiv/` – roční archivy dat

Konfigurační soubor `config.ini` (cesta ke kořenové složce) je uložen v `AppData` a slouží jako "ukazatel" při startu aplikace. Jeho záloha (`config.ini.bak`) je ale uložena přímo v datové složce uživatele (`Zalohy/config.ini.bak`), ne v `AppData` – díky tomu přežije i reinstalaci aplikace nebo operačního systému. Pokud aplikace při prvním spuštění na novém/přeinstalovaném počítači najde tuto zálohu v zadané složce, automaticky obnoví veškeré nastavení bez nutnosti cokoliv zadávat znovu.

**Bezpečný zápis (crash-safe)**  
Ukládání neprobíhá přímým přepsáním souboru, ale přes dočasný soubor (`.tmp`) a atomickou náhradu (`File.Replace`). Díky tomu nemůže při pádu aplikace nebo výpadku uprostřed zápisu dojít k poškození nebo ztrátě posledního platného souboru.

**Automatická obnova**  
Pokud je při načítání hlavní soubor poškozený nebo chybí, aplikace se automaticky pokusí obnovit data ze zálohy. Uživatel je o průběhu informován barevně odlišenými hláškami v konzoli.

**Roční archivace dat**  
Datové soubory jsou platné vždy jen do konce kalendářního roku. Při každém spuštění aplikace se zkontroluje, zda uplynul kalendářní rok od poslední archivace – pokud ano, vytvoří se kopie aktuálních souborů do složky `Archiv`. Pracovní soubory zůstávají nedotčené. Archivní soubory starší než 5 let jsou automaticky odstraněny.

---

## Struktura projektu

/ZOO

├── Program.cs

├── ZOO.cs

├── Zamestnanec.cs

├── Zvire.cs

├── SkladovaPolozka.cs

├── SkladovyPohyb.cs

├── Cenik.cs

├── TypVstupenky.cs

├── VypocetCenyVstupenky.cs

├── PokladniPohyb.cs

├── SpravceZamestnancu.cs

├── SpravceZvirat.cs

├── SpravceSkladu.cs

├── SpravcePokladny.cs

├── Transakce.cs

├── AuditZaznam.cs

├── TechnickyZaznam.cs

├── TypAkce.cs

├── UrovenLogu.cs

└── README.md

---

### Externí knihovny (samostatné projekty)

Část funkčnosti byla vyčleněna do samostatných, znovupoužitelných knihoven, aby šly nezávisle referencovat i v jiných projektech (např. v aplikaci Losovač, která používá vlastní, jednodušší verzi `InputHelper`):

- **TextHelper** – zpracování a validace uživatelských vstupů (`UpravaVstupu.ZeptejSeAUprav`), včetně zobrazení konkrétní chybové zprávy při neplatném vstupu
- **SelectHelper** – výběr položky ze seznamu (`SelectHelp.VybratPolozku`)
- **InputHelper** – převod textu na TitleCase (`TitleCase.ToTitleCase`) a správné velké písmena v názvech měst s ohledem na české předložky (`MestoTitleCase.ZpracujNazevMesta`)
- **DateConverterForJson** – JSON konvertor pro typ `DateOnly` (`DateOnlyConverter`)
- **VekZviratHelper** – výpočet aktuálního věku zvířete z data narození (`VypocetVeku.VypocitejVek`, `VypocetVeku.VypocitejVekTextove`), včetně skloňování slov "rok"/"měsíc"
- **PohybHelper** – sdílené rozhraní `IPohyb` a výčtové typy pohybů (`SkladovyTypPohybu`, `PokladniTypPohybu`, `FinancniTypPohybu`) pro sklad, pokladnu a budoucí účetnictví, včetně čitelných popisků (`PopiskyHelper`)

---

## Použité techniky

- OOP (třídy, vlastnosti, zapouzdření)
- Primary constructors
- Partial třídy a metody se zdrojově generovaným regulárním výrazem (`GeneratedRegex`)
- Enumy (kategorie skladových položek, typ skladového pohybu)
- Delegáty pro lazy loading dat (`RegistrujNacitani` / `ZajistiData`)
- Práce se soubory (`File.ReadAllText`, `File.WriteAllText`, `File.Copy`, `File.Replace`, `File.Move`, `File.Delete`)
- Crash-safe zápis přes dočasný soubor a atomickou náhradu
- Automatická obnova dat ze zálohy při poškození nebo ztrátě souboru
- Automatizovaná roční archivace dat s retenční politikou (uchování 5 let)
- Transakční zpracování s rollbackem v paměti při selhání zápisu (`Transakce.ProvedSUlozenim`)
- Append-only auditní log s ochranou proti přepisu, vyjmutý z retenční politiky archivace
- Validace vstupů, včetně formátové validace e-mailu
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

    string mesto = UpravaVstupu.ZeptejSeAUprav(
        "", "město", v => v, s => s, jeNove: true);

    string ulice = UpravaVstupu.ZeptejSeAUprav(
        "", "ulice a číslo popisné", v => v, s => s, jeNove: true);

    string psc = UpravaVstupu.ZeptejSeAUprav(
        "", "PSČ", v => v, s => s, jeNove: true);

    string telefon = UpravaVstupu.ZeptejSeAUprav(
        "", "telefonní číslo", v => v, s => s, jeNove: true);

    string email = UpravaVstupu.ZeptejSeAUprav(
        "", "e-mail",
        v => v,
        s =>
        {
            if (!Zamestnanec.JePlatnyEmail(s))
                throw new FormatException("E-mail nemá platný formát (očekává se např. jmeno@domena.cz).");
            return s;
        },
        jeNove: true);

    zoo.Zamestnanci.Add(new Zamestnanec(
        jmeno, prijmeni, datumNarozeni, mzda, pracovniPozice,
        mesto, ulice, psc, telefon, email));

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
[Stáhnout zde](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2/releases/tag/v1.5.0)

**v1.6.0** – Rozšířena evidence zaměstnanců o adresní a kontaktní údaje (město, ulice s číslem, PSČ, telefon, e-mail). Přidána nová třída `MestoTitleCase` v knihovně `InputHelper` pro správné velké písmena v názvech měst s předložkami (např. "Ústí nad Labem", ne "Ústí Nad Labem"). Validace e-mailu přes zdrojově generovaný regulární výraz (`GeneratedRegex`). Vylepšena knihovna `TextHelper` – při neplatném vstupu se nyní zobrazí konkrétní chybová zpráva a výzva k zadání se opakuje.  
[Stáhnout zde](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2/releases/tag/v1.6.0)

**v1.7.0** – Přidán modul Pokladna s prodejem a stornem vstupenek (dětská, dospělá, ZTP, důchodce, rodinná úplná/neúplná, skupinová), konfigurovatelným ceníkem včetně rodinných a skupinových slev, denní uzávěrkou s rozpadem podle kategorií a filtrovaným výpisem pohybů podle typu a rozmezí data (měsíční/roční uzávěrky). Přidána nová knihovna `PohybHelper` sjednocující typy pohybů (sklad, pokladna) přes společné rozhraní `IPohyb`. Rozšířeny Statistiky o průměrnou denní návštěvnost počítanou z prodaných vstupenek za zvolený měsíc. Kompletně přepracována struktura ukládání dat – oddělené složky `Data/` a `Zalohy/` s podsložkami po modulech, záloha `config.ini` uložená v datové složce uživatele (přežije reinstalaci aplikace i systému).  
[Stáhnout zde](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2/releases/tag/v1.7.0)

**v1.8.0** – Přidáno transakční ukládání s rollbackem v paměti při selhání zápisu na disk (`Transakce.ProvedSUlozenim`), zajišťující konzistenci mezi daty v paměti a na disku napříč zaměstnanci, skladem a pokladnou. Přidán auditní log (`AuditZaznam`) zaznamenávající důležité změny – mzdy, ceník, skladové a pokladní pohyby, smazání záznamů – včetně původní a nové hodnoty, času a přihlášeného uživatele; log je koncipován jako append-only a vyjmutý z retenční politiky archivace. Přidán technický log (`TechnickyZaznam`) pro diagnostiku na úrovni IT podpory. Ošetřena konzistence mezi souborem skladu a historií skladových pohybů při selhání dílčího zápisu.  
[Stáhnout zde](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2/releases/tag/v1.8.0)