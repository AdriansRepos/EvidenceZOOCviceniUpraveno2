using EvidenceZOOCviceniUpraveno2.Data;
using EvidenceZOOCviceniUpraveno2.Entity;
using PohybHelper;

namespace EvidenceZOOCviceniUpraveno2.Vypisy.Zamestnanci
{
    internal static class ZamestnanciVypisy
    {
        public static void VypisHlavicku()
        {
            Console.WriteLine("VÝPIS ZAMĚSTNANCŮ");
            Console.WriteLine();
            Console.WriteLine(
                $"{"Číslo",-12} {"Jméno",-15} {"Příjmení",-15} {"Datum narození",-15} {"Mzda",-10} {"Pozice",-20}"
            );
            Console.WriteLine(new string('-', 90));
        }

        public static void VypisZamestnance(Zamestnanec zam)
        {
            Console.WriteLine(
                $"{"zam.OsobniCislo",-12} {zam.Jmeno,-15} {zam.Prijmeni,-15} {zam.DatumNarozeni,-15} {zam.Mzda,-10} {zam.PracovniPozice,-20}"
            );
        }

        public static void VypisKontaktniUdaje(Zamestnanec zam)
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
                foreach (Dite dite in zam.Deti)
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
                string sleva = zam.Partner.UplatnitSlevu ? " (uplatněna sleva)" : "";
                Console.WriteLine($"  - {zam.Partner.Jmeno} {zam.Partner.Prijmeni}, RČ: {zam.Partner.RodneCislo}{ztp}{sleva}");
            }
        }
    }
}
