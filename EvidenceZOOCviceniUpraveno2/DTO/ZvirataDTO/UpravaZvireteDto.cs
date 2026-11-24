
namespace EvidenceZOOCviceniUpraveno2.DTO.ZvirataDTO;

/// <summary>
/// Data pro úpravu existujícího zvířete.
/// </summary>
internal sealed class UpravaZvireteDto
{
    public string Nazev { get; init; } = string.Empty;
    public DateOnly DatumNarozeni { get; init; }
    public double Vaha { get; init; }
}