using System.ComponentModel;

namespace EvidenceZOOCviceniUpraveno2.Enumy
{
    /// <summary>
    /// Kategorie prodávané vstupenky.
    /// </summary>
    enum TypVstupenky
    {
        [Description("Dětská")]
        Detska,

        [Description("Dospělá")]
        Dospela,

        [Description("ZTP")]
        ZTP,

        [Description("Důchodce (65+)")]
        Duchodce,

        [Description("Rodinná – úplná (2 dospělí + děti)")]
        RodinaUplna,

        [Description("Rodinná – neúplná (1 dospělý + děti)")]
        RodinaNeuplna,

        [Description("Skupinová")]
        Skupina
    }
}