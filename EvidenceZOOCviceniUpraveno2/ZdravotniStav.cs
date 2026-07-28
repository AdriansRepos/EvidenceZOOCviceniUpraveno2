using System.ComponentModel;

namespace EvidenceZOOCviceniUpraveno2
{
    enum ZdravotniStav
    {
        [Description("Dobrý")]
        Dobry,

        [Description("Omezený")]
        Omezeny,

        [Description("Invalidita 1. stupně")]
        Invalidita1,

        [Description("Invalidita 2. stupně")]
        Invalidita2,

        [Description("Invalidita 3. stupně")]
        Invalidita3
    }
}