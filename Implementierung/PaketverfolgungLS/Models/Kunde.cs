using System.Collections.Generic;

namespace Paketverfolgung.Models;

public class Kunde
{
    public int KundeID { get; set; }

    public string Name { get; set; } = "";
    public string? Adresse { get; set; }
    public string? EMail { get; set; }
    public string? Telefonnummer { get; set; }

    public List<Bestellung> Bestellungen { get; set; } = new();
}
