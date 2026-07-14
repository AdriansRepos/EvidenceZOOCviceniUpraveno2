namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// Provede operaci nad daty drženými v paměti a jejich uložení jako
    /// jednu logickou jednotku. Pokud uložení selže, změna v paměti se
    /// vrátí zpět (rollback), aby stav v paměti odpovídal stavu na disku.
    /// </summary>
    static class Transakce
    {
        /// <summary>
        /// Provede akci a následné uložení. Při selhání uložení vrátí
        /// data do stavu před akcí pomocí rollback a chybu nahlásí uživateli.
        /// </summary>
        /// <param name="akce">Změna prováděná nad daty v paměti (např. přidání položky do seznamu).</param>
        /// <param name="rollback">Akce vracející data do stavu před provedením akce.</param>
        /// <param name="ulozeni">Uložení dat na disk.</param>
        /// <param name="popisOperace">Popis operace pro chybovou hlášku.</param>
        /// <returns>True, pokud operace i uložení proběhly úspěšně; jinak false.</returns>
        public static bool ProvedSUlozenim(Action akce, Action rollback, Action ulozeni, string popisOperace)
        {
            akce.Invoke();

            try
            {
                ulozeni.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                rollback.Invoke();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Operace '{popisOperace}' se nezdařila a byla vrácena zpět: {ex.Message}");
                Console.ResetColor();
                return false;
            }
        }
    }
}