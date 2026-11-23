using BarevneVypisyHelper;

namespace EvidenceZOOCviceniUpraveno2.Vypisy;

static class TransakceVypisy
{
    public static void VypisNeuspesnouTransakci(string popisOperace, Exception ex)
    {
        VypisyDoKonzole.VypisVarovani(
            $"Operace '{popisOperace}' se nezdařila a byla vrácena zpět: {ex.Message}");
    }
}