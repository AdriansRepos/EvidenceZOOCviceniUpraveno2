using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.Entity;
using PohybHelper;

namespace EvidenceZOOCviceniUpraveno2.Vypisy.Sklad
{
    internal static class SkladVypisy
    {
        public static void VypisHlavickuSkladu()
        {
            Console.WriteLine("STAV SKLADU");            
            Console.WriteLine($"\n{"Název",-20} {"Kategorie",-25} {"Množství",8} {"Jednotka",-8}");
            Console.WriteLine(new string('-', 70));
        }

        public static void VypisPolozku(SkladovaPolozka polozka)
        {
            if (polozka.JeDochazejici)                
                VypisyDoKonzole.VypisVarovani(
                    $"{polozka.Nazev,-20} {polozka.Kategorie,-25} {polozka.Mnozstvi,8:0.##} {polozka.Jednotka,-8} " +
                    $"(min. {polozka.MinimalniStav:0.##})"
                );
        }

        public static void VypisHlavickuPohybu()
        {
            Console.WriteLine($"{"Datum a čas",-20} {"Popis",-45}");
            Console.WriteLine(new string('-', 65));
        }

        public static void VypisPohyb(SkladovyPohyb pohyb)
        {
            Console.ForegroundColor = pohyb.TypPohybu == SkladovyTypPohybu.Naskladneni
                ? ConsoleColor.Green
                : ConsoleColor.Yellow;

            Console.WriteLine($"{pohyb.DatumCas,-20:g} {pohyb.Popis(),-45}");
            Console.ResetColor();
        }
    }
}