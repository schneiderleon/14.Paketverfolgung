-- Falls Datenbank nicht existiert, wird sie erstellt
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PaketverfolgungDB')
BEGIN
    CREATE DATABASE PaketverfolgungDB;
END
GO

-- Datenbank auswählen
USE PaketverfolgungDB;
GO

-- Falls Tabellen bereits existieren, löschen
IF OBJECT_ID('Bestellung', 'U') IS NOT NULL DROP TABLE Bestellung;
IF OBJECT_ID('Kunde', 'U') IS NOT NULL DROP TABLE Kunde;
GO

-- Tabelle Kunde erstellen
CREATE TABLE Kunde (
    KundeID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Adresse NVARCHAR(200),
    EMail NVARCHAR(100),
    Telefonnummer NVARCHAR(50)
);
GO

-- Tabelle Bestellung erstellen
CREATE TABLE Bestellung (
    BestellungID INT IDENTITY(1,1) PRIMARY KEY,
    Bestelldatum DATE NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Produktname NVARCHAR(100) NOT NULL,
    KundeID INT NOT NULL,
    FOREIGN KEY (KundeID) REFERENCES Kunde(KundeID)
);
GO

-- Beispiel-Datensätze für Kunden
INSERT INTO Kunde (Name, Adresse, EMail, Telefonnummer) VALUES
('Anna Müller', 'Hauptstr. 12, Berlin', 'anna.mueller@example.com', '030-123456'),
('Peter Schmidt', 'Bahnhofstr. 8, Hamburg', 'peter.schmidt@example.com', '040-987654'),
('Julia Becker', 'Marktplatz 5, München', 'julia.becker@example.com', '089-246810'),
('Thomas Lange', 'Wiesenweg 3, Köln', 'thomas.lange@example.com', '0221-998877'),
('Sabine Keller', 'Seestr. 9, Stuttgart', 'sabine.keller@example.com', '0711-112233');
GO

-- Beispiel-Datensätze für Bestellungen
INSERT INTO Bestellung (Bestelldatum, Status, Produktname, KundeID) VALUES
('2025-10-20', 'in Bearbeitung', 'Bluetooth Kopfhörer', 1),
('2025-10-21', 'versendet', 'Smartwatch', 1),
('2025-10-22', 'zugestellt', 'Laptop Tasche', 2),
('2025-10-23', 'in Bearbeitung', 'USB-C Ladegerät', 3),
('2025-10-24', 'versendet', 'Gaming Maus', 4);
GO

-- Beispielausgabe mit JOIN
SELECT 
    b.BestellungID,
    k.Name AS Kunde,
    b.Produktname,
    b.Status,
    b.Bestelldatum
FROM Bestellung b
JOIN Kunde k ON b.KundeID = k.KundeID
ORDER BY b.Bestelldatum;
GO
