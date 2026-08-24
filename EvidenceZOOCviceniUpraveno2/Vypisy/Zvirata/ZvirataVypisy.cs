using EvidenceZOOCviceniUpraveno2.Entity;

namespace EvidenceZOOCviceniUpraveno2.Vypisy.Zvirata
{
    internal static class ZvirataVypisy
    {
        public static void VypisHlavicku()
        {
            Console.WriteLine("VÝPIS ZVÍŘAT");
            Console.WriteLine();
            Console.WriteLine($"{"Název",-20} {"Věk",-15} {"Váha",-10}");
            Console.WriteLine(new string('-', 45));
        }

        public static void VypisZvire(Zvire zvire)
        {
            Console.WriteLine(
                $"{zvire.Nazev,-20} {zvire.Vek,-15} {zvire.Vaha + " kg",-10}"
            );
        }

        public static void VypisVyhledaneZvire(Zvire zvire)
        {
            Console.WriteLine($"Nalezeno:\t{zvire.Nazev}\tVěk: {zvire.Vek}\tVáha: {zvire.Vaha} kg");
        }
    }
}