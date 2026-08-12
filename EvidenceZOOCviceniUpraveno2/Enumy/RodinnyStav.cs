using System.ComponentModel;

namespace EvidenceZOOCviceniUpraveno2.Enumy
{
    public enum RodinnyStav
    {
        [Description("Svobodný/á")]
        Svobodny,

        [Description("Ženatý/vdaná")]
        ZenatyVdana,

        [Description("Rozvedený/á")]
        Rozvedeny,

        [Description("Vdovec/vdova")]
        Vdovec
    }
}