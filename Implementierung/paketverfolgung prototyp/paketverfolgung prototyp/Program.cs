using System;
using System.IO;

class Program
{

    const string DataFile = "bestellungen.txt";

    static void Main()
    {

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Paketverfolgung (sehr einfacher Prototyp) ===");
            Console.WriteLine("1) Neue Bestellung anlegen");
            Console.WriteLine("2) Alle Bestellungen anzeigen");
            Console.WriteLine("3) Versandstatus ändern");
            Console.WriteLine("0) Beenden");
            Console.Write("Auswahl: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    NeueBestellung();
                    break;
                case "2":
                    AlleBestellungenAnzeigen();
                    break;
                case "3":
                    StatusÄndern();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Ungültige Auswahl. Weiter mit Taste...");
                    Console.ReadKey();
                    break;
            }
        }
    }


    static void NeueBestellung()
    {
        Console.Clear();
        Console.WriteLine("=== Neue Bestellung anlegen ===");

        Console.Write("Kundenname: ");
        string kunde = Console.ReadLine();

        Console.Write("Produktname: ");
        string produkt = Console.ReadLine();

        Console.Write("Status (z.B. in Bearbeitung / versendet / zugestellt): ");
        string status = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(kunde) || string.IsNullOrWhiteSpace(produkt))
        {
            Console.WriteLine("Kunde und Produkt dürfen nicht leer sein.");
            Console.WriteLine("Weiter mit Taste...");
            Console.ReadKey();
            return;
        }

        int neueId = ErmittleNächsteID();


        string zeile = neueId + ";" + kunde + ";" + produkt + ";" + status;

        File.AppendAllLines(DataFile, new[] { zeile });

        Console.WriteLine();
        Console.WriteLine("Bestellung angelegt mit ID: " + neueId);
        Console.WriteLine("Weiter mit Taste...");
        Console.ReadKey();
    }


    static void AlleBestellungenAnzeigen()
    {
        Console.Clear();
        Console.WriteLine("=== Alle Bestellungen ===");

        if (!File.Exists(DataFile))
        {
            Console.WriteLine("Keine Bestellungen vorhanden (Datei existiert nicht).");
            Console.WriteLine("Weiter mit Taste...");
            Console.ReadKey();
            return;
        }

        string[] zeilen = File.ReadAllLines(DataFile);

        if (zeilen.Length == 0)
        {
            Console.WriteLine("Keine Bestellungen vorhanden (Datei ist leer).");
        }
        else
        {
            foreach (string zeile in zeilen)
            {

                string[] teile = zeile.Split(';');
                if (teile.Length < 4)
                {

                    Console.WriteLine("DEFECT: " + zeile);
                }
                else
                {
                    string id = teile[0];
                    string kunde = teile[1];
                    string produkt = teile[2];
                    string status = teile[3];

                    Console.WriteLine("#" + id + " | " + kunde + " | " + produkt + " | " + status);
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine("Weiter mit Taste...");
        Console.ReadKey();
    }


    static void StatusÄndern()
    {
        Console.Clear();
        Console.WriteLine("=== Versandstatus ändern ===");

        if (!File.Exists(DataFile))
        {
            Console.WriteLine("Keine Bestellungen vorhanden (Datei existiert nicht).");
            Console.WriteLine("Weiter mit Taste...");
            Console.ReadKey();
            return;
        }

        Console.Write("Bestell-ID: ");
        string idInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(idInput))
        {
            Console.WriteLine("Keine ID eingegeben.");
            Console.WriteLine("Weiter mit Taste...");
            Console.ReadKey();
            return;
        }

        string[] zeilen = File.ReadAllLines(DataFile);
        bool gefunden = false;

        for (int i = 0; i < zeilen.Length; i++)
        {
            string zeile = zeilen[i];
            string[] teile = zeile.Split(';');

            if (teile.Length < 4)
                continue;

            string id = teile[0];

            if (id == idInput)
            {
                string kunde = teile[1];
                string produkt = teile[2];
                string alterStatus = teile[3];

                Console.WriteLine("Gefunden: #" + id + " | " + kunde + " | " + produkt + " | " + alterStatus);
                Console.Write("Neuer Status: ");
                string neuerStatus = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(neuerStatus))
                {
                    Console.WriteLine("Status darf nicht leer sein. Abbruch.");
                    Console.WriteLine("Weiter mit Taste...");
                    Console.ReadKey();
                    return;
                }


                zeilen[i] = id + ";" + kunde + ";" + produkt + ";" + neuerStatus;
                gefunden = true;
                break;
            }
        }

        if (!gefunden)
        {
            Console.WriteLine("Keine Bestellung mit dieser ID gefunden.");
        }
        else
        {

            File.WriteAllLines(DataFile, zeilen);
            Console.WriteLine("Status aktualisiert und gespeichert.");
        }

        Console.WriteLine("Weiter mit Taste...");
        Console.ReadKey();
    }


    static int ErmittleNächsteID()
    {
        if (!File.Exists(DataFile))
            return 1;

        string[] zeilen = File.ReadAllLines(DataFile);

        int maxId = 0;

        for (int i = 0; i < zeilen.Length; i++)
        {
            string zeile = zeilen[i];
            string[] teile = zeile.Split(';');
            if (teile.Length < 1)
                continue;

            if (int.TryParse(teile[0], out int id))
            {
                if (id > maxId)
                    maxId = id;
            }
        }

        return maxId + 1;
    }
}
