using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paketverfolgung;
using Paketverfolgung.Models;
using System;
using System.Linq;

namespace PaketVerfolgungstest
{

    

    [TestClass]
    public class DatabaseTests
    {
        private DbContextOptions<PaketverfolgungContext> _options;

        [TestInitialize]
        public void Setup()
        {
            
            _options = new DbContextOptionsBuilder<PaketverfolgungContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            
            using (var context = new PaketverfolgungContext(_options))
            {
                var k1 = new Kunde { KundeID = 1, Name = "Test Kunde", Adresse = "Testweg 1" };
                context.Kunden.Add(k1);
                context.Bestellungen.Add(new Bestellung
                {
                    BestellungID = 1,
                    KundeID = 1,
                    Produktname = "Test Produkt",
                    Status = "in Bearbeitung",
                    Bestelldatum = DateTime.Today
                });
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Test_Kunde_Existiert_In_InMemory_DB()
        {
            using (var context = new PaketverfolgungContext(_options))
            {
                var kunden = context.Kunden.ToList();
                Assert.AreEqual(1, kunden.Count);
                Assert.AreEqual("Test Kunde", kunden[0].Name);
            }
        }

        [TestMethod]
        public void Test_Neue_Bestellung_Hinzufuegen()
        {
            using (var context = new PaketverfolgungContext(_options))
            {
                var neueBestellung = new Bestellung
                {
                    KundeID = 1,
                    Produktname = "Neues Handy",
                    Status = "versendet",
                    Bestelldatum = DateTime.Today
                };

                context.Bestellungen.Add(neueBestellung);
                context.SaveChanges();

                Assert.AreEqual(2, context.Bestellungen.Count());
            }
        }

        [TestMethod]
        public void Test_Suche_Nach_Produktname()
        {
            using (var context = new PaketverfolgungContext(_options))
            {
                var treffer = context.Bestellungen
                    .Where(b => b.Produktname.Contains("Test"))
                    .ToList();

                Assert.IsTrue(treffer.Any());
                Assert.AreEqual("Test Produkt", treffer[0].Produktname);
            }
        }

        [TestMethod]
        public void Test_Kunde_Loeschen()
        {
            using (var context = new PaketverfolgungContext(_options))
            {
                var kunde = context.Kunden.First();
                context.Kunden.Remove(kunde);
                context.SaveChanges();

                Assert.AreEqual(0, context.Kunden.Count());
            }
        }

        [TestMethod]
        public void Test_Bestellstatus_Aktualisieren()
        {
            using (var context = new PaketverfolgungContext(_options))
            {
                var bestellung = context.Bestellungen.First();
                bestellung.Status = "zugestellt";
                context.SaveChanges();

                var aktualisiert = context.Bestellungen.First();
                Assert.AreEqual("zugestellt", aktualisiert.Status);
            }
        }
    }
}
