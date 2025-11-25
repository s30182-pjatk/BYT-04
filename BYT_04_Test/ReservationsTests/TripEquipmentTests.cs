using System;
using System.IO;
using BYT_04.Reservations;
using NUnit.Framework;

namespace BYT_04.Tests.ReservationsTests
{
    [TestFixture]
    public class TripEquipmentTests
    {
        private int _tripCounter = 1;
        private int _equipmentCounter = 1;

        [Test]
        public void Ctor_ValidArguments_SetsPropertiesAndAddsToExtent()
        {
            var trip = CreateSampleTrip();
            var eq = CreateSampleEquipment();
            var initialCount = TripEquipment.TripEquipments.Count;

            var link = new TripEquipment(trip, eq, 5, "For advanced skiers");

            Assert.That(link.Trip, Is.SameAs(trip));
            Assert.That(link.Equipment, Is.SameAs(eq));
            Assert.That(link.Quantity, Is.EqualTo(5));
            Assert.That(link.Notes, Is.EqualTo("For advanced skiers"));

            Assert.That(TripEquipment.TripEquipments.Count, Is.EqualTo(initialCount + 1));
            Assert.That(TripEquipment.TripEquipments.Contains(link), Is.True);
        }

        [Test]
        public void Ctor_NullTrip_ThrowsArgumentException()
        {
            var eq = CreateSampleEquipment();

            Assert.Throws<ArgumentException>(() => new TripEquipment(null!, eq, 1));
        }

        [Test]
        public void Ctor_NullEquipment_ThrowsArgumentException()
        {
            var trip = CreateSampleTrip();

            Assert.Throws<ArgumentException>(() => new TripEquipment(trip, null!, 1));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Ctor_NonPositiveQuantity_ThrowsArgumentException(int quantity)
        {
            var trip = CreateSampleTrip();
            var eq = CreateSampleEquipment();

            Assert.Throws<ArgumentException>(() => new TripEquipment(trip, eq, quantity));
        }

        [Test]
        public void Notes_Whitespace_IsStoredAsNull()
        {
            var trip = CreateSampleTrip();
            var eq = CreateSampleEquipment();
            var link = new TripEquipment(trip, eq, 2, "something");

            link.Notes = "   ";

            Assert.That(link.Notes, Is.Null);
        }

        [Test]
        public void SaveAndLoad_RestoresSameNumberOfLinks()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            TripEquipment.SetDirectory(tempDir);

            var trip = CreateSampleTrip();
            var eq = CreateSampleEquipment();
            _ = new TripEquipment(trip, eq, 3, "Persist");

            var countBefore = TripEquipment.TripEquipments.Count;

            TripEquipment.Save();
            TripEquipment.Load();

            var countAfter = TripEquipment.TripEquipments.Count;
            Assert.That(countAfter, Is.EqualTo(countBefore));
        }

        [Test]
        public void Save_WritesXmlFileToConfiguredDirectory()
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

        // helpers

        private Trip CreateSampleTrip()
        {
            var start = DateTime.Today.AddDays(7);
            var end = start.AddDays(4);

            return new Trip(
                $"Sample Trip {_tripCounter++}",
                "Alps",
                start,
                end,
                900m
            );
        }

        private Equipment CreateSampleEquipment()
        {
            return new Equipment(
                $"Skis {_equipmentCounter++}",
                DateTime.Today.AddDays(-5)
            );
        }
    }
}
