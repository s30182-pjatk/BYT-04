using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BYT_04.Reservations;
using NUnit.Framework;

namespace BYT_04.Tests.Reservations
{
    [TestFixture]
    public class TripTests
    {
        [Test]
        public void Constructor_WithValidData_SetsPropertiesAndAddsToExtent()
        {
            // arrange
            var start = DateTime.Today.AddDays(10);
            var end = start.AddDays(7);
            var initialCount = Trip.Trips.Count;

            // act
            var trip = new Trip(
                name: "Winter Camp " + Guid.NewGuid().ToString("N"),
                destination: "Alps",
                startDate: start,
                endDate: end,
                pricePerPerson: 1500m,
                description: "Ski trip"
            );

            // assert
            Assert.That(trip.Name, Is.Not.Empty);
            Assert.That(trip.Destination, Is.EqualTo("Alps"));
            Assert.That(trip.StartDate, Is.EqualTo(start));
            Assert.That(trip.EndDate, Is.EqualTo(end));
            Assert.That(trip.PricePerPerson, Is.EqualTo(1500m));
            Assert.That(trip.Description, Is.EqualTo("Ski trip"));

            Assert.That(Trip.Trips.Count, Is.EqualTo(initialCount + 1));
            Assert.That(Trip.Trips.Last(), Is.SameAs(trip));
        }

       
        [TestCase("")]
        [TestCase("   ")]
        public void Name_Invalid_Throws(string invalidName)
        {
            var start = DateTime.Today.AddDays(5);
            var end = start.AddDays(3);

            Assert.Throws<ArgumentException>(() =>
                new Trip(
                    name: invalidName!,
                    destination: "Alps",
                    startDate: start,
                    endDate: end,
                    pricePerPerson: 1000m));
        }

        
        [TestCase("")]
        [TestCase("   ")]
        public void Destination_Invalid_Throws(string invalidDestination)
        {
            var start = DateTime.Today.AddDays(5);
            var end = start.AddDays(3);

            Assert.Throws<ArgumentException>(() =>
                new Trip(
                    name: "Trip",
                    destination: invalidDestination!,
                    startDate: start,
                    endDate: end,
                    pricePerPerson: 1000m));
        }

        [Test]
        public void StartDate_InPast_Throws()
        {
            var start = DateTime.Today.AddDays(-1);
            var end = DateTime.Today.AddDays(3);

            Assert.Throws<ArgumentException>(() => new Trip("Trip", "Alps", start, end, 1000m));
        }

        [Test]
        public void EndDate_BeforeStart_Throws()
        {
            var start = DateTime.Today.AddDays(10);
            var end = start.AddDays(-1);

            Assert.Throws<ArgumentException>(() => new Trip("Trip", "Alps", start, end, 1000m));
        }

        [Test]
        public void PricePerPerson_Negative_Throws()
        {
            var start = DateTime.Today.AddDays(10);
            var end = start.AddDays(2);

            Assert.Throws<ArgumentException>(() => new Trip("Trip", "Alps", start, end, -1m));
        }

        [Test]
        public void Description_Whitespace_BecomesNull()
        {
            var start = DateTime.Today.AddDays(5);
            var end = start.AddDays(3);
            var trip = new Trip("Trip", "Alps", start, end, 1000m);

            trip.Description = "   ";

            Assert.That(trip.Description, Is.Null);
        }

        [Test]
        public void TripsExtent_IsReadOnlyFromOutside()
        {
            var start = DateTime.Today.AddDays(6);
            var end = start.AddDays(3);
            var trip = new Trip("Extent Test " + Guid.NewGuid().ToString("N"), "Tatras", start, end, 500m);

            var extent = Trip.Trips;

            Assert.Throws<NotSupportedException>(() =>
            {
                var collection = (ICollection<Trip>)extent;
                collection.Add(trip);
            });
        }

        [Test]
        public void SaveAndLoad_PreservesExtentContent()
        {
            // arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Trip.SetDirectory(tempDir);

            var start = DateTime.Today.AddDays(8);
            var end = start.AddDays(4);
            _ = new Trip("Persist Trip " + Guid.NewGuid().ToString("N"), "Dolomites", start, end, 800m);

            var before = Trip.Trips
                .Select(t => new
                {
                    t.Name,
                    t.Destination,
                    t.StartDate,
                    t.EndDate,
                    t.PricePerPerson,
                    t.Description
                })
                .ToList();

            // act
            Trip.Save();
            Trip.Load();

            // assert
            var after = Trip.Trips
                .Select(t => new
                {
                    t.Name,
                    t.Destination,
                    t.StartDate,
                    t.EndDate,
                    t.PricePerPerson,
                    t.Description
                })
                .ToList();

            Assert.That(after.Count, Is.EqualTo(before.Count));
            Assert.That(after, Is.EqualTo(before));
        }

        [Test]
        public void Save_CreatesTripXmlFile()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Trip.SetDirectory(tempDir);

            var start = DateTime.Today.AddDays(5);
            var end = start.AddDays(3);
            _ = new Trip("File Trip " + Guid.NewGuid().ToString("N"), "Alps", start, end, 1000m);

            Trip.Save();

            var expectedPath = Path.Combine(tempDir, "trips.xml");
            Assert.That(File.Exists(expectedPath), Is.True);
        }
    }
}
