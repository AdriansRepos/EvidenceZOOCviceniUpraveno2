
namespace EvidenceZOOCviceniUpraveno2.DTO.ZvirataDTO;

/// <summary>
/// Data potřebná k založení nového zvířete.
/// </summary>
internal sealed class NoveZvireDto
{
    public string Nazev { get; init; } = string.Empty;
    public DateOnly DatumNarozeni { get; init; }
    public double Vaha { get; init; }
    public DateOnly? DatumPrijetiZJineZoo { get; init; }
}