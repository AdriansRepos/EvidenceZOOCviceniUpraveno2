using EvidenceZOOCviceniUpraveno2.Enumy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceZOOCviceniUpraveno2.DTO.ZamestnanciDTO;

public class ZamestnanecPrehledDto
{
    public string OsobniCislo { get; set; } = string.Empty;
    public string Jmeno { get; set; } = string.Empty;
    public string Prijmeni { get; set; } = string.Empty;
    public DateOnly DatumNarozeni { get; set; }
    public string RodneCislo { get; set; } = string.Empty;
    public int Mzda { get; set; }
    public string PracovniPozice { get; set; } = string.Empty;
    public string Mesto { get; set; } = string.Empty;
    public string Ulice { get; set; } = string.Empty;
    public string PSC { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public RodinnyStav RodinnyStav { get; set; }
    public ZdravotniStav ZdravotniStav { get; set; }
    public string TypDokladu { get; set; } = string.Empty;
    public string CisloDokladu { get; set; } = string.Empty;
    public List<DitePrehledDto> Deti { get; set; } = [];
    public PartnerPrehledDto? Partner { get; set; }
}