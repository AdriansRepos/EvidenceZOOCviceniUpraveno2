\# ZOO – Konzolová aplikace v C#



Tento projekt je jednoduchá, ale plně funkční konzolová aplikace pro správu ZOO.  

Umožňuje evidovat \*\*zvířata\*\*, \*\*zaměstnance\*\*, provádět \*\*statistiky\*\*, ukládat data do souborů a pracovat s uživatelskými vstupy.



\---



\## Funkce aplikace



\### Zvířata

\- Přidání nového zvířete  

\- Výpis všech zvířat  

\- Úprava existujícího zvířete  

\- Smazání zvířete  

\- Vyhledávání podle názvu  

\- Statistiky (průměrná váha, počet druhů…)



\### Zaměstnanci

\- Přidání zaměstnance  

\- Úprava zaměstnance  

\- Smazání zaměstnance  

\- Výpis všech zaměstnanců  

\- Vyhledávání podle jména  

\- Ukládání do souboru + záloha `.bak`



\---



\## Ukládání dat



Aplikace ukládá data do textových souborů:



\- `zamestnanci.txt`  

\- `zamestnanci.bak` (automatická záloha)  

\- `zvirata.txt`



Každý řádek obsahuje hodnoty oddělené znakem `|`.



\---



\## Struktura projektu

/ZOO

├── Program.cs

├── ZOO.cs

├── Zamestnanec.cs

├── Zvire.cs

├── SpravceZamestnancu.cs

├── SpravceZvirat.cs

├── Vstupy.cs

├── zamestnanci.txt

├── zvirata.txt

└── README.md





\---



\## Použité techniky



\- OOP (třídy, vlastnosti, zapouzdření)

\- Výčtové typy (enum)

\- Delegáty a události

\- Práce se soubory (`File.ReadAllText`, `File.WriteAllLines`)

\- Validace vstupů

\- Try-catch bloky

\- DateOnly

\- Kolekce (`List<T>`)



\---



```csharp

public void PridatZamestnance()

{

&#x20;   string jmeno = Vstupy.NactiBezZakazanychZnaku("Zadejte jméno: ", '|');

&#x20;   string prijmeni = Vstupy.NactiBezZakazanychZnaku("Zadejte příjmení: ", '|');

&#x20;   Pozice pozice = Vstupy.ZeptejSeAUpravEnum<Pozice>("Pracovní pozice");

&#x20;   DateOnly narozeni = Vstupy.ZeptejSeAUpravDateOnly(DateOnly.MinValue, "Datum narození", "Zadejte", true);



&#x20;   Zamestnanec z = new(jmeno, prijmeni, pozice, narozeni);

&#x20;   Zamestnanci.Add(z);

&#x20;   UlozZamestnance();

}





\## Plánované funkce

Ukládání do JSON



Export statistik



Lepší validace vstupů



Automatické testy



GUI verze (WPF nebo MAUI)Ukázka kódu





