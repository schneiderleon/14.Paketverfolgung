using Microsoft.EntityFrameworkCore;
using Paketverfolgung.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;DeletedRowInaccessibleException 

namespace Paketverfolgung;

public static class Database
{
    public static void EnsureCreatedAndSeeded()
    {
        using var db = new PaketverfolgungContext();
        db.Database.EnsureCreated();

        if (!db.Kunden.Any())
        {
            var k1 = new Kunde { Name = "Anna Müller", Adresse = "Hauptstr. 12", EMail = "anna@example.com", Telefonnummer = "030-123456" };
            var k2 = new Kunde { Name = "Peter Schmidt", Adresse = "Bahnhofstr. 8", EMail = "peter@example.com", Telefonnummer = "040-987654" };
            var k3 = new Kunde { Name = "Julia Becker", Adresse = "Marktplatz 5", EMail = "julia@example.com", Telefonnummer = "089-246810" };

            db.Kunden.AddRange(k1, k2, k3);
            db.SaveChanges();

            db.Bestellungen.AddRange(
                new Bestellung { KundeID = k1.KundeID, Produktname = "Bluetooth Kopfhörer", Status = "in Bearbeitung", Bestelldatum = DateTime.Today.AddDays(-2) },
                new Bestellung { KundeID = k2.KundeID, Produktname = "USB-C Ladegerät", Status = "versendet", Bestelldatum = DateTime.Today.AddDays(-1) },
                new Bestellung { KundeID = k3.KundeID, Produktname = "Laptop Tasche", Status = "zugestellt", Bestelldatum = DateTime.Today }
            );

            db.SaveChanges();
        }
    }

    

    public static List<OrderRow> GetOrders(string? search = null)
    {
        using var db = new PaketverfolgungContext();

        var q = db.Bestellungen
            .AsNoTracking()
            .Include(b => b.Kunde)
            .AsQueryable();

        var s = (search ?? "").Trim();
        if (s.Length > 0)
        {
            var hasDate = DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt);

            q = q.Where(b =>
                EF.Functions.Like(b.Kunde!.Name, $"%{s}%") ||
                EF.Functions.Like(b.Produktname, $"%{s}%") ||
                EF.Functions.Like(b.Status, $"%{s}%") ||
                (hasDate && b.Bestelldatum.Date == dt.Date));
        }

        return q.OrderByDescending(b => b.Bestelldatum)
            .Select(b => new OrderRow
            {
                Id = b.BestellungID,
                Customer = b.Kunde!.Name,
                Product = b.Produktname,
                Status = b.Status,
                Date = b.Bestelldatum.ToString("dd.MM.yyyy")
            })
            .ToList();
    }

    public static OrderEditRow? GetOrderById(int orderId)
    {
        using var db = new PaketverfolgungContext();

        return db.Bestellungen.AsNoTracking()
            .Where(b => b.BestellungID == orderId)
            .Select(b => new OrderEditRow
            {
                Id = b.BestellungID,
                CustomerId = b.KundeID,
                Product = b.Produktname,
                Status = b.Status,
                OrderDate = b.Bestelldatum
            })
            .FirstOrDefault();
    }

    public static int InsertOrder(int kundeId, DateTime bestelldatum, string status, string produktname)
    {
        using var db = new PaketverfolgungContext();

        var entity = new Bestellung
        {
            KundeID = kundeId,
            Bestelldatum = bestelldatum.Date,
            Status = status,
            Produktname = produktname
        };

        db.Bestellungen.Add(entity);
        db.SaveChanges();
        return entity.BestellungID;
    }

    public static int UpdateOrder(int orderId, int kundeId, DateTime bestelldatum, string status, string produktname)
    {
        using var db = new PaketverfolgungContext();

        var entity = db.Bestellungen.FirstOrDefault(b => b.BestellungID == orderId);
        if (entity is null) return 0;

        entity.KundeID = kundeId;
        entity.Bestelldatum = bestelldatum.Date;
        entity.Status = status;
        entity.Produktname = produktname;

        return db.SaveChanges();
    }

    public static int DeleteOrder(int orderId)
    {
        using var db = new PaketverfolgungContext();

        var entity = db.Bestellungen.FirstOrDefault(b => b.BestellungID == orderId);
        if (entity is null) return 0;

        db.Bestellungen.Remove(entity);
        return db.SaveChanges();
    }

    public static List<OrderMiniRow> GetOrdersByCustomerId(int kundeId)
    {
        using var db = new PaketverfolgungContext();

        return db.Bestellungen.AsNoTracking()
            .Where(b => b.KundeID == kundeId)
            .OrderByDescending(b => b.Bestelldatum)
            .Select(b => new OrderMiniRow
            {
                Id = b.BestellungID,
                Product = b.Produktname,
                Status = b.Status,
                OrderDate = b.Bestelldatum
            })
            .ToList();
    }

    public static int DeleteOrdersByCustomerId(int kundeId)
    {
        using var db = new PaketverfolgungContext();

        var orders = db.Bestellungen.Where(b => b.KundeID == kundeId).ToList();
        if (orders.Count == 0) return 0;

        db.Bestellungen.RemoveRange(orders);
        return db.SaveChanges();
    }

    

    public static List<CustomerRow> GetCustomers()
    {
        using var db = new PaketverfolgungContext();

        return db.Kunden.AsNoTracking()
            .OrderBy(k => k.KundeID)
            .Select(k => new CustomerRow
            {
                Id = k.KundeID,
                Name = k.Name,
                Address = k.Adresse ?? "",
                Email = k.EMail ?? "",
                Phone = k.Telefonnummer ?? ""
            })
            .ToList();
    }

    public static List<CustomerLookup> GetCustomerLookup()
    {
        using var db = new PaketverfolgungContext();

        return db.Kunden.AsNoTracking()
            .OrderBy(k => k.Name)
            .Select(k => new CustomerLookup { Id = k.KundeID, Name = k.Name })
            .ToList();
    }

    public static bool CustomerExists(int kundeId)
    {
        using var db = new PaketverfolgungContext();
        return db.Kunden.AsNoTracking().Any(k => k.KundeID == kundeId);
    }

    public static int InsertCustomer(string name, string? adresse, string? email, string? phone)
    {
        using var db = new PaketverfolgungContext();

        var entity = new Kunde
        {
            Name = name,
            Adresse = adresse,
            EMail = email,
            Telefonnummer = phone
        };

        db.Kunden.Add(entity);
        db.SaveChanges();
        return entity.KundeID;
    }

    public static CustomerEditRow? GetCustomerById(int kundeId)
    {
        using var db = new PaketverfolgungContext();

        return db.Kunden.AsNoTracking()
            .Where(k => k.KundeID == kundeId)
            .Select(k => new CustomerEditRow
            {
                Id = k.KundeID,
                Name = k.Name,
                Address = k.Adresse ?? "",
                Email = k.EMail ?? "",
                Phone = k.Telefonnummer ?? ""
            })
            .FirstOrDefault();
    }

    public static int UpdateCustomer(int kundeId, string name, string? adresse, string? email, string? phone)
    {
        using var db = new PaketverfolgungContext();

        var entity = db.Kunden.FirstOrDefault(k => k.KundeID == kundeId);
        if (entity is null) return 0;

        entity.Name = name;
        entity.Adresse = adresse;
        entity.EMail = email;
        entity.Telefonnummer = phone;

        return db.SaveChanges();
    }

    public static int DeleteCustomer(int kundeId)
    {
        using var db = new PaketverfolgungContext();

        var entity = db.Kunden.FirstOrDefault(k => k.KundeID == kundeId);
        if (entity is null) return 0;

        db.Kunden.Remove(entity);
        return db.SaveChanges();
    }
}



public class OrderRow
{
    public int Id { get; set; }
    public string Customer { get; set; } = "";
    public string Product { get; set; } = "";
    public string Status { get; set; } = "";
    public string Date { get; set; } = "";
}

public class CustomerRow
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}

public class CustomerLookup
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public override string ToString() => $"{Id} - {Name}";
}

public class OrderEditRow
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Product { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime OrderDate { get; set; }
}

public class CustomerEditRow
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}

public class OrderMiniRow
{
    public int Id { get; set; }
    public string Product { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime OrderDate { get; set; }
    public string Date => OrderDate.ToString("dd.MM.yyyy");
}
