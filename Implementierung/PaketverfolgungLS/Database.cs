using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace PaketverfolgungStep4;

public static class Database
{
    // Wenn du NICHT LocalDB nutzt, ändere den Server-Teil.
    private const string ConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=PaketverfolgungDB;Trusted_Connection=True;TrustServerCertificate=True";

    public static SqlConnection OpenConnection()
    {
        var con = new SqlConnection(ConnectionString);
        con.Open();
        return con;
    }

    // ---------------- Orders ----------------

    public static List<OrderRow> GetOrders(string? search = null)
    {
        using var con = OpenConnection();

        var sql = @"
SELECT 
    b.BestellungID AS Id,
    k.Name AS Customer,
    b.Produktname AS Product,
    b.Status AS Status,
    CONVERT(varchar(10), b.Bestelldatum, 104) AS Date
FROM Bestellung b
JOIN Kunde k ON b.KundeID = k.KundeID
WHERE (@q IS NULL OR @q = '' 
       OR k.Name LIKE '%' + @q + '%'
       OR b.Produktname LIKE '%' + @q + '%'
       OR b.Status LIKE '%' + @q + '%'
       OR CONVERT(varchar(10), b.Bestelldatum, 104) LIKE '%' + @q + '%')
ORDER BY b.Bestelldatum DESC;
";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = (object?)(search?.Trim()) ?? DBNull.Value;

        using var r = cmd.ExecuteReader();
        var list = new List<OrderRow>();
        while (r.Read())
        {
            list.Add(new OrderRow
            {
                Id = r.GetInt32(0),
                Customer = r.GetString(1),
                Product = r.GetString(2),
                Status = r.GetString(3),
                Date = r.GetString(4),
            });
        }
        return list;
    }

    public static OrderEditRow? GetOrderById(int orderId)
    {
        using var con = OpenConnection();
        var sql = @"
SELECT BestellungID, Bestelldatum, Status, Produktname, KundeID
FROM Bestellung
WHERE BestellungID = @id;
";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = orderId;

        using var r = cmd.ExecuteReader();
        if (!r.Read())
            return null;

        return new OrderEditRow
        {
            Id = r.GetInt32(0),
            OrderDate = r.GetDateTime(1),
            Status = r.GetString(2),
            Product = r.GetString(3),
            CustomerId = r.GetInt32(4)
        };
    }

    public static int InsertOrder(int kundeId, DateTime bestelldatum, string status, string produktname)
    {
        using var con = OpenConnection();
        var sql = @"
INSERT INTO Bestellung (Bestelldatum, Status, Produktname, KundeID)
VALUES (@d, @s, @p, @k);
SELECT SCOPE_IDENTITY();
";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@d", SqlDbType.Date).Value = bestelldatum.Date;
        cmd.Parameters.Add("@s", SqlDbType.NVarChar, 50).Value = status;
        cmd.Parameters.Add("@p", SqlDbType.NVarChar, 100).Value = produktname;
        cmd.Parameters.Add("@k", SqlDbType.Int).Value = kundeId;

        var idObj = cmd.ExecuteScalar();
        return Convert.ToInt32(idObj);
    }

    public static int UpdateOrder(int orderId, int kundeId, DateTime bestelldatum, string status, string produktname)
    {
        using var con = OpenConnection();
        var sql = @"
UPDATE Bestellung
SET Bestelldatum = @d,
    Status = @s,
    Produktname = @p,
    KundeID = @k
WHERE BestellungID = @id;
";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@d", SqlDbType.Date).Value = bestelldatum.Date;
        cmd.Parameters.Add("@s", SqlDbType.NVarChar, 50).Value = status;
        cmd.Parameters.Add("@p", SqlDbType.NVarChar, 100).Value = produktname;
        cmd.Parameters.Add("@k", SqlDbType.Int).Value = kundeId;
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = orderId;

        return cmd.ExecuteNonQuery();
    }

    public static int DeleteOrder(int orderId)
    {
        using var con = OpenConnection();
        var sql = @"DELETE FROM Bestellung WHERE BestellungID = @id;";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = orderId;
        return cmd.ExecuteNonQuery();
    }

    public static List<OrderMiniRow> GetOrdersByCustomerId(int kundeId)
    {
        using var con = OpenConnection();
        var sql = @"
SELECT BestellungID, Produktname, Status, Bestelldatum
FROM Bestellung
WHERE KundeID = @id
ORDER BY Bestelldatum DESC;
";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = kundeId;

        using var r = cmd.ExecuteReader();
        var list = new List<OrderMiniRow>();
        while (r.Read())
        {
            list.Add(new OrderMiniRow
            {
                Id = r.GetInt32(0),
                Product = r.GetString(1),
                Status = r.GetString(2),
                OrderDate = r.GetDateTime(3)
            });
        }
        return list;
    }

    public static int DeleteOrdersByCustomerId(int kundeId)
    {
        using var con = OpenConnection();
        var sql = @"DELETE FROM Bestellung WHERE KundeID = @id;";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = kundeId;
        return cmd.ExecuteNonQuery();
    }

    // ---------------- Customers ----------------

    public static List<CustomerRow> GetCustomers()
    {
        using var con = OpenConnection();
        var sql = @"
SELECT KundeID AS Id, Name, Adresse, EMail AS Email, Telefonnummer AS Phone
FROM Kunde
ORDER BY KundeID;
";
        using var cmd = new SqlCommand(sql, con);

        using var r = cmd.ExecuteReader();
        var list = new List<CustomerRow>();
        while (r.Read())
        {
            list.Add(new CustomerRow
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Address = r.IsDBNull(2) ? "" : r.GetString(2),
                Email = r.IsDBNull(3) ? "" : r.GetString(3),
                Phone = r.IsDBNull(4) ? "" : r.GetString(4),
            });
        }
        return list;
    }

    public static List<CustomerLookup> GetCustomerLookup()
    {
        using var con = OpenConnection();
        var sql = @"SELECT KundeID, Name FROM Kunde ORDER BY Name;";
        using var cmd = new SqlCommand(sql, con);

        using var r = cmd.ExecuteReader();
        var list = new List<CustomerLookup>();
        while (r.Read())
        {
            list.Add(new CustomerLookup
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1)
            });
        }
        return list;
    }

    public static bool CustomerExists(int kundeId)
    {
        using var con = OpenConnection();
        var sql = @"SELECT COUNT(1) FROM Kunde WHERE KundeID = @id;";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = kundeId;

        var count = (int)cmd.ExecuteScalar()!;
        return count > 0;
    }

    public static int InsertCustomer(string name, string? adresse, string? email, string? phone)
    {
        using var con = OpenConnection();
        var sql = @"
INSERT INTO Kunde (Name, Adresse, EMail, Telefonnummer)
VALUES (@n, @a, @e, @t);
SELECT SCOPE_IDENTITY();
";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@n", SqlDbType.NVarChar, 100).Value = name;
        cmd.Parameters.Add("@a", SqlDbType.NVarChar, 200).Value = (object?)adresse ?? DBNull.Value;
        cmd.Parameters.Add("@e", SqlDbType.NVarChar, 100).Value = (object?)email ?? DBNull.Value;
        cmd.Parameters.Add("@t", SqlDbType.NVarChar, 50).Value = (object?)phone ?? DBNull.Value;

        var idObj = cmd.ExecuteScalar();
        return Convert.ToInt32(idObj);
    }

    public static CustomerEditRow? GetCustomerById(int kundeId)
    {
        using var con = OpenConnection();
        var sql = @"
SELECT KundeID, Name, Adresse, EMail, Telefonnummer
FROM Kunde
WHERE KundeID = @id;
";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = kundeId;

        using var r = cmd.ExecuteReader();
        if (!r.Read())
            return null;

        return new CustomerEditRow
        {
            Id = r.GetInt32(0),
            Name = r.GetString(1),
            Address = r.IsDBNull(2) ? "" : r.GetString(2),
            Email = r.IsDBNull(3) ? "" : r.GetString(3),
            Phone = r.IsDBNull(4) ? "" : r.GetString(4)
        };
    }

    public static int UpdateCustomer(int kundeId, string name, string? adresse, string? email, string? phone)
    {
        using var con = OpenConnection();
        var sql = @"
UPDATE Kunde
SET Name = @n,
    Adresse = @a,
    EMail = @e,
    Telefonnummer = @t
WHERE KundeID = @id;
";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@n", SqlDbType.NVarChar, 100).Value = name;
        cmd.Parameters.Add("@a", SqlDbType.NVarChar, 200).Value = (object?)adresse ?? DBNull.Value;
        cmd.Parameters.Add("@e", SqlDbType.NVarChar, 100).Value = (object?)email ?? DBNull.Value;
        cmd.Parameters.Add("@t", SqlDbType.NVarChar, 50).Value = (object?)phone ?? DBNull.Value;
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = kundeId;

        return cmd.ExecuteNonQuery();
    }

    public static int DeleteCustomer(int kundeId)
    {
        using var con = OpenConnection();
        var sql = @"DELETE FROM Kunde WHERE KundeID = @id;";
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = kundeId;
        return cmd.ExecuteNonQuery();
    }
}

// ---------------- DTOs (für DataGrid Binding) ----------------

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
