using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BYT_04.Reservations;
using NUnit.Framework;

namespace BYT_04.Tests.Reservations
{
    [TestFixture]
    public class EquipmentTests
    {
        [Test]
        public void Constructor_WithValidData_SetsPropertiesAndAddsToExtent()
        {
            // arrange
            var name = "Ski poles " + Guid.NewGuid().ToString("N");
            var date = DateTime.Today.AddDays(-3);
            var initialCount = Equipment.Equipments.Count;

            // act
            var equipment = new Equipment(name, date);

            // assert
            Assert.That(equipment.Name, Is.EqualTo(name));
            Assert.That(equipment.LastMaintenanceDate, Is.EqualTo(date));
            Assert.That(Equipment.Equipments.Count, Is.EqualTo(initialCount + 1));
            Assert.That(Equipment.Equipments.Last(), Is.SameAs(equipment));
        }


        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WithInvalidName_Throws(string invalidName)
        {
            var date = DateTime.Today.AddDays(-1);
            Assert.Throws<ArgumentException>(() => new Equipment(invalidName!, date));
        }

        [Test]
        public void LastMaintenanceDate_InFuture_Throws()
        {
            var future = DateTime.Today.AddDays(1);
            Assert.Throws<ArgumentException>(() => new Equipment("Helmet", future));
        }

        [Test]
        public void EquipmentsExtent_IsReadOnlyFromOutside()
        {
            var eq = new Equipment("Boots " + Guid.NewGuid().ToString("N"), DateTime.Today.AddDays(-2));
            var extent = Equipment.Equipments;
            Assert.Throws<NotSupportedException>(() =>
            {
                var collection = (ICollection<Equipment>)extent;
                collection.Add(eq);
            });
        }

        [Test]
        public void SaveAndLoad_PreservesExtentContent()
        {
            // arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Equipment.SetDirectory(tempDir);
            _ = new Equipment("Goggles " + Guid.NewGuid().ToString("N"), DateTime.Today.AddDays(-5));

            var before = Equipment.Equipments
                .Select(e => new { e.Name, e.LastMaintenanceDate })
                .ToList();

            // act
            Equipment.Save();
            Equipment.Load();

            // assert
            var after = Equipment.Equipments
                .Select(e => new { e.Name, e.LastMaintenanceDate })
                .ToList();

            Assert.That(after.Count, Is.EqualTo(before.Count));
            Assert.That(after, Is.EqualTo(before));
        }

        [Test]
        public void Save_CreatesXmlFileInDirectory()
        {
            // arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Equipment.SetDirectory(tempDir);

            _ = new Equipment("Backpack " + Guid.NewGuid().ToString("N"), DateTime.Today.AddDays(-7));

            // act
            Equipment.Save();

            // assert
            var expectedPath = Path.Combine(tempDir, "equipment.xml");
            Assert.That(File.Exists(expectedPath), Is.True);
        }
    }
}