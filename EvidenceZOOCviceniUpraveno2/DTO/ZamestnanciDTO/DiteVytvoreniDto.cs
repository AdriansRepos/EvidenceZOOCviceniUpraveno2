using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceZOOCviceniUpraveno2.DTO.ZamestnanciDTO;

public class DiteVytvoreniDto
{
    public string OsobniCisloZamestnance { get; set; } = string.Empty;
    public string Jmeno { get; set; } = string.Empty;
    public string Prijmeni { get; set; } = string.Empty;
    public DateOnly DatumNarozeni { get; set; }
    public string RodneCislo { get; set; } = string.Empty;
    public bool JeDrzitelZtpP { get; set; }
    public bool UplatnenBonus { get; set; }
}