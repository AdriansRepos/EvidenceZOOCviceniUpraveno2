using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceZOOCviceniUpraveno2.DTO.ZamestnanciDTO;

public class PartnerPrehledDto
{
   public string Jmeno { get; set; } = string.Empty;
   public string Prijmeni { get; set; } = string.Empty;
   public string RodneCislo { get; set; } = string.Empty;
   public bool JeDrzitelZtpP { get; set; }
   public bool PecujeODiteDo3Let { get; set; }
   public bool PrijemDo68Tisic { get; set; }
   public bool UplatnitSlevu { get; set; }
}