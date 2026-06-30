using System.Text.Json.Serialization;

namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Reprezentuje jedno zvíře v zoologické zahradě.
    /// Uchovává jeho název, věk a váhu.
    /// </summary>
    class Zvire
    {
        /// <summary>
        /// Název zvířete. Nikdy nesmí být null.
        /// Je automaticky převáděn do formátu TitleCase.
        /// </summary>
        private string _nazev = string.Empty;
        [JsonPropertyName("nazev")]
        public string Nazev
        {
            get => _nazev;
            internal set => _nazev = Vstupy.ToTitleCase(value);
        }

        /// <summary>
        /// Věk zvířete v letech.
        /// </summary>
        [JsonPropertyName("vek")]
        public int Vek { get; internal set; }

        /// <summary>
        /// Váha zvířete v kilogramech.
        /// </summary>
        [JsonPropertyName("vaha")]
        public double Vaha { get; internal set; }

        /// <summary>
        /// Vytvoří nové zvíře a nastaví jeho název, věk a váhu.
        /// </summary>
        /// <param name="nazev">Název zvířete.</param>
        /// <param name="vek">Věk v letech.</param>
        /// <param name="vaha">Váha v kilogramech.</param>
        [JsonConstructor]
        public Zvire(string nazev, int vek, double vaha)
        {
            Nazev = nazev;
            Vek = vek;
            Vaha = vaha;
        }

        /// <summary>
        /// Metoda pro správné tvarování slova rok
        /// </summary>
        /// <returns>Vrátí správný tvar slova rok podle věku zvířete</returns>
        private string VekSklonovany()
        {
            if (Vek == 1)
                return $"{Vek} rok";
            else if (Vek >= 2 && Vek <= 4)
                return $"{Vek} roky";
            else
                return $"{Vek} let";
        }

        /// <summary>
        /// Vypíše informace o zvířeti do konzole,        
        /// </summary>
        public void VypisZvire()
        {
            Console.WriteLine(
                $"{Nazev,-20} {VekSklonovany(),-12} {Vaha + " kg",-10}"
            );
        }       
    }
}