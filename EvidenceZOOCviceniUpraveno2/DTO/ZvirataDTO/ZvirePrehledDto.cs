
namespace EvidenceZOOCviceniUpraveno2.DTO.ZvirataDTO;

/// <summary>
/// Data zvířete určená pro výpis a vyhledávání.
/// </summary>
internal sealed class ZvirePrehledDto
{
    public string Nazev { get; init; } = string.Empty;
    public string Vek { get; init; } = string.Empty;
    public double Vaha { get; init; }
}