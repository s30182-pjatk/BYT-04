using System;
using System.Collections.Generic;
using System.IO;
using BYT_04.Reservations;
using NUnit.Framework;

namespace BYT_04.Tests.Reservations
{
    [TestFixture]
    public class TripEquipmentTests
    {
        private Trip CreateSampleTrip()
        {
            var start = DateTime.Today.AddDays(7);
            var end = start.AddDays(4);
            return new Trip("Sample Trip " + Guid.NewGuid().ToString("N"),
                            "Alps", start, end, 900m);
        }

        private Equipment CreateSampleEquipment()
        {
            return new Equipment("Skis " + Guid.NewGuid().ToString("N"),
                                 DateTime.Today.AddDays(-5));
        }

        [Test]
        public void Constructor_WithValidData_SetsPropertiesAndAddsToExtent()
        {
            // arrange
            var trip = CreateSampleTrip();
            var eq = CreateSampleEquipment();
            var initialCount = TripEquipment.TripEquipments.Count;

            // act
            var link = new TripEquipment(trip, eq, 5, "For advanced skiers");

            // assert
            Assert.That(link.Trip, Is.SameAs(trip));
            Assert.That(link.Equipment, Is.SameAs(eq));
            Assert.That(link.Quantity, Is.EqualTo(5));
            Assert.That(link.Notes, Is.EqualTo("For advanced skiers"));

            Assert.That(TripEquipment.TripEquipments.Count, Is.EqualTo(initialCount + 1));
            Assert.That(TripEquipment.TripEquipments[^1], Is.SameAs(link));
        }

        [Test]
        public void Constructor_NullTrip_Throws()
        {
            var eq = CreateSampleEquipment();

            Assert.Throws<ArgumentException>(() => new TripEquipment(null!, eq, 1));
        }

        [Test]
        public void Constructor_NullEquipment_Throws()
        {
            var trip = CreateSampleTrip();

            Assert.Throws<ArgumentException>(() => new TripEquipment(trip, null!, 1));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_NonPositiveQuantity_Throws(int qty)
        {
            var trip = CreateSampleTrip();
            var eq = CreateSampleEquipment();

            Assert.Throws<ArgumentException>(() => new TripEquipment(trip, eq, qty));
        }

        [Test]
        public void Notes_Whitespace_BecomesNull()
        {
            var trip = CreateSampleTrip();
            var eq = CreateSampleEquipment();
            var link = new TripEquipment(trip, eq, 2, "something");

            link.Notes = "   ";

            Assert.That(link.Notes, Is.Null);
        }

        [Test]
        public void TripEquipmentsExtent_IsReadOnlyFromOutside()
        {
            var trip = CreateSampleTrip();
            var eq = CreateSampleEquipment();
            var link = new TripEquipment(trip, eq, 1);

            var extent = TripEquipment.TripEquipments;

            Assert.Throws<NotSupportedException>(() =>
            {
                var collection = (ICollection<TripEquipment>)extent;
                collection.Add(link);
            });
        }

        [Test]
        public void SaveAndLoad_KeepsSameNumberOfLinks()
        {
            // arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            TripEquipment.SetDirectory(tempDir);

            var trip = CreateSampleTrip();
            var eq = CreateSampleEquipment();
            _ = new TripEquipment(trip, eq, 3, "Persist");

            var beforeCount = TripEquipment.TripEquipments.Count;

            // act
            TripEquipment.Save();
            TripEquipment.Load();

            // assert
            var afterCount = TripEquipment.TripEquipments.Count;
            Assert.That(afterCount, Is.EqualTo(beforeCount));
        }

        [Test]
        public void Save_CreatesTripEquipmentXmlFile()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            TripEquipment.SetDirectory(tempDir);

            var trip = CreateSampleTrip();
            var eq = CreateSampleEquipment();
            _ = new TripEquipment(trip, eq, 3);

            TripEquipment.Save();

            var expectedPath = Path.Combine(tempDir, "tripequipment.xml");
            Assert.That(File.Exists(expectedPath), Is.True);
        }
    }
}
