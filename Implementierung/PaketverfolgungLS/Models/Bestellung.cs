using System;

namespace Paketverfolgung.Models;

public class Bestellung
{
    public int BestellungID { get; set; }

    public DateTime Bestelldatum { get; set; }
    public string Status { get; set; } = "";
    public string Produktname { get; set; } = "";

    public int KundeID { get; set; }
    public Kunde? Kunde { get; set; }
}
