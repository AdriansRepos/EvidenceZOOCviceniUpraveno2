using BarevneVypisyHelper;
using EvidenceZOOCviceniUpraveno2.DTO.ZvirataDTO;

namespace EvidenceZOOCviceniUpraveno2.Vypisy.Zvirata;

internal static class ZvirataVypisy
{
    public static void VypisHlavicku()
    {
        VypisyDoKonzole.VypisInformaci("VÝPIS ZVÍŘAT");
        VypisyDoKonzole.VypisInformaci($"\n{"Název",-20} {"Věk",-15} {"Váha",-10}");
        VypisyDoKonzole.VypisInformaci(new string('-', 45));
    }

    public static void VypisZvire(ZvirePrehledDto zvire)
    {
        VypisyDoKonzole.VypisInformaci(
            $"{zvire.Nazev,-20} {zvire.Vek,-15} {zvire.Vaha + " kg",-10}"
        );
    }

    public static void VypisVyhledaneZvire(ZvirePrehledDto zvire)
    {
        VypisyDoKonzole.VypisInformaci(
            $"Nalezeno:\t{zvire.Nazev}\tVěk: {zvire.Vek}\tVáha: {zvire.Vaha} kg"
        );
    }
}