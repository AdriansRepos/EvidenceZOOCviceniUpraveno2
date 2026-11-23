using EvidenceZOOCviceniUpraveno2.DTO.ZamestnanciDTO;
using PohybHelper;

namespace EvidenceZOOCviceniUpraveno2.Vypisy.Zamestnanci
{
    internal static class ZamestnanciVypisy
    {
        public static void VypisHlavicku()
        {
            Console.WriteLine("VÝPIS ZAMĚSTNANCŮ");
            Console.WriteLine(
                $"\n{"Číslo",-12} {"Jméno",-15} {"Příjmení",-15} {"Datum narození",-15} {"Mzda",-10} {"Pozice",-20}"
            );
            Console.WriteLine(new string('-', 90));
        }

        public static void VypisZamestnance(ZamestnanecPrehledDto zam)
        {
            Console.WriteLine(
                $"{zam.OsobniCislo,-12} {zam.Jmeno,-15} {zam.Prijmeni,-15} {zam.DatumNarozeni,-15} {zam.Mzda,-10} {zam.PracovniPozice,-20}"
            );
        }

        public static void VypisKontaktniUdaje(ZamestnanecPrehledDto zam)
        {
            Console.WriteLine($"Adresa:            {zam.Ulice}, {zam.PSC} {zam.Mesto}");
            Console.WriteLine($"Telefon:           {zam.Telefon}");
            Console.WriteLine($"E-mail:            {zam.Email}");
            Console.WriteLine($"Rodinný stav:      {PopiskyHelper.ZiskejPopisek(zam.RodinnyStav)}");
            Console.WriteLine($"Zdravotní stav:    {PopiskyHelper.ZiskejPopisek(zam.ZdravotniStav)}");
            Console.WriteLine($"Doklad totožnosti: {zam.TypDokladu} {zam.CisloDokladu}");

            if (zam.Deti.Count > 0)
            {
                Console.WriteLine("Děti:");
                foreach (DitePrehledDto dite in zam.Deti)
                {
                    string bonus = dite.UplatnenBonus ? " (uplatněn bonus)" : "";
                    string ztpP = dite.JeDrzitelZtpP ? " [ZTP/P]" : "";
                    Console.WriteLine($"  - {dite.Jmeno} {dite.Prijmeni}, nar. {dite.DatumNarozeni:d}, RČ: {dite.RodneCislo}{ztpP}{bonus}");
                }
            }

            if (zam.Partner != null)
            {
                Console.WriteLine("Partner / Manžel(ka):");
                string ztp = zam.Partner.JeDrzitelZtpP ? " [ZTP/P]" : "";
                string peceDo3 = zam.Partner.PecujeODiteDo3Let ? " [péče do 3 let]" : "";
                string prijem = zam.Partner.PrijemDo68Tisic ? " [příjem do 68 tis.]" : "";
                string sleva = zam.Partner.UplatnitSlevu ? " (uplatněna sleva)" : "";
                Console.WriteLine($"  - {zam.Partner.Jmeno} {zam.Partner.Prijmeni}, RČ: {zam.Partner.RodneCislo}{ztp}{peceDo3}{prijem}{sleva}");
            }
        }
    }
}