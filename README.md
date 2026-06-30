# ZOO – Konzolová aplikace v C#


Tento projekt je jednoduchá, ale plně funkční konzolová aplikace pro správu ZOO.  
Umožňuje evidovat \*\*zvířata\*\*, \*\*zaměstnance\*\*, provádět \*\*statistiky\*\*, ukládat data do souborů a pracovat s uživatelskými vstupy.

---

## Funkce aplikace

### Zvířata

- Přidání nového zvířete  
- Výpis všech zvířat  
- Úprava existujícího zvířete
- Smazání zvířete
- Vyhledávání podle názvu
- Statistiky (průměrná váha, počet druhů…)



### Zaměstnanci

- Přidání zaměstnance
- Úprava zaměstnance
- Smazání zaměstnance
- Výpis všech zaměstnanců
- Vyhledávání podle jména
- Ukládání do souboru + záloha `.bak`

---

## Ukládání dat

Aplikace ukládá data do textových souborů:

- `zamestnanci.txt` 
- `zamestnanci.bak` (automatická záloha) 
- `zvirata.txt`
- `zvirata.bak` (automatická záloha)

Každý řádek obsahuje hodnoty oddělené znakem `|`.

---

## Struktura projektu

/ZOO

├── Program.cs

├── ZOO.cs

├── Zamestnanec.cs

├── Zvire.cs

├── SpravceZamestnancu.cs

├── SpravceZvirat.cs

├── Vstupy.cs

├── DateOnlyConverter.cs

└── README.md

---

## Použité techniky

- OOP (třídy, vlastnosti, zapouzdření)
- Výčtové typy (enum)
- Delegáty a události pro přístup k souborům
- Práce se soubory (`File.ReadAllText`, `File.WriteAllLines`)
- Validace vstupů
- Try-catch bloky
- DateOnly
- Kolekce (`List<T>`)
- Parsování data pro JSON soubor
- Ukládání do JSON souborů
- Zadání cest k adresáři a souborům jenom při prvním spuštění aplikace - nastavení a jejich uložení do konfiguračního souboru
- Lazy loading pro načtení souborů až když je potřeba
- Úprava formátování výpisů do tabulek

---

```csharp

 public void Pridat()
 {
     Console.WriteLine("ZADÁNÍ NOVÉHO ZAMĚSTNANCE");
     string jmeno = Vstupy.ZeptejSeAUprav(
         "", "jméno",
         v => v,
         s => s,
         jeNove: true);

     string prijmeni = Vstupy.ZeptejSeAUprav(
         "", "příjmení",
         v => v,
         s => s,
         jeNove: true);

     string pracovniPozice = Vstupy.ZeptejSeAUprav(
         "", "pracovní pozice",
         v => v,
         s => s,
         jeNove: true);

     DateOnly datumNarozeni = Vstupy.ZeptejSeAUprav(
         DateOnly.MinValue, "datum narození",
         v => v.ToString(),
         s => DateOnly.Parse(s),
         jeNove: true);

     int mzda = Vstupy.ZeptejSeAUprav(
         0, "mzda",
         v => v.ToString(),
         s => int.Parse(s),
         jeNove: true);

     zoo.Zamestnanci.Add(new Zamestnanec(jmeno, prijmeni, datumNarozeni, mzda, pracovniPozice));
     zoo.UlozZamestnance();
     Console.WriteLine("Zaměstnanec byl úspěšně přidán.");
 }
```

## Plánované funkce:

- Export statistik
- Automatické testy
- GUI verze (WPF nebo MAUI)Ukázka kódu

### Release
- První vydaná spustitelná verze
- Aktuální verze: v1.0.0
- Stáhnout zde: Releases
https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2/releases