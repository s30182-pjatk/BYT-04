using BYT_04.Reservations;
using BYT_04_Test.TestUtils;
using NUnit.Framework;

namespace BYT_04_Test.ReservationsTests;

[TestFixture]
public class ReservationTest
{
    private string _tempDir;
    private string _xmlFile;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "reservation_persistence_tests");
        _xmlFile = Path.Combine(_tempDir, "reservations.xml");
        
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);

        Directory.CreateDirectory(_tempDir);
        
        Reservation.SetDirectory(_tempDir);

        // Prevent data bleeding between tests
        ClearAllExtents();
    }

    [TearDown]
    public void Cleanup()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
        
        ClearAllExtents();
    }
    
    private void ClearAllExtents()
    {
        //Helper to clear static lists via reflection
        ClearList.ClearStaticList<Reservation>("_reservations");
    }
    
    // Validation Tests
    
    [Test]
    public void TestReservationInvalidStartDate()
    {
        var invalidDate = DateTime.Today.AddDays(-1);
        Assert.Throws<ArgumentException>(() => new Reservation { StartDate = invalidDate });
    }

    [Test]
    public void TestReservationInvalidEndDate()
    {
        var startDate = DateTime.Today.AddDays(5);
        var endDate = DateTime.Today.AddDays(4);
        Assert.Throws<ArgumentException>(() => new Reservation { StartDate = startDate, EndDate = endDate });
    }

    [Test]
    public void TestReservationInvalidTotalPrice()
    {
        var invalidPrice = -100m;
        Assert.Throws<ArgumentException>(() => new Reservation { TotalPrice = invalidPrice });
    }

    
    // LOGIC TESTS
    
    [Test]
    public void TestReservationFinalizeReservation_ShouldChangeStatus_WhenPending()
    {
        var reservation = new Reservation { Status = ReservationStatus.Pending };

        reservation.FinalizeReservation();

        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Confirmed));
    }

    [Test]
    public void TestReservationChangeStatus_ShouldUpdateStatus()
    {
        var reservation = new Reservation { Status = ReservationStatus.Confirmed };
        reservation.ChangeReservationStatus(ReservationStatus.Completed);
        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Completed));
    }

    
    // PERSISTENCE TESTS
    
    [Test]
    public void TestReservationCheckPendingReservations()
    {
        // Arrange 
        var r1 = new Reservation(1, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
        var r2 = new Reservation(2, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
        var r3 = new Reservation(3, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Completed, 100m);

        // Act
        Reservation.Save();             
        
        ClearAllExtents(); // Wipe memory to simulate restart
        
        Reservation.Load();             

        var pendingList = Reservation.CheckPendingReservations();

        // Assert
        Assert.That(Reservation.Reservations.Count, Is.EqualTo(3), "Should have loaded all 3 reservations.");
        Assert.That(pendingList.Count, Is.EqualTo(2), "Should filter down to 2 pending reservations.");
    }

    [Test]
    public void TestReservationRemoveCompletedReservations()
    {
        // Arrange
        var r1 = new Reservation(1, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
        r1.ChangeReservationStatus(ReservationStatus.Completed);

        var r2 = new Reservation(2, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
        r2.ChangeReservationStatus(ReservationStatus.Completed);

        var r3 = new Reservation(3, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);

        // Act
        Reservation.Save();             
        
        ClearAllExtents();
        
        Reservation.Load();             
        
        // Assert
        Assert.That(Reservation.Reservations.Count, Is.EqualTo(3), "Extent should contain exactly 3 loaded items before deletion.");
        
        Reservation.RemoveCompletedReservations();
        
        Assert.That(Reservation.Reservations.Count, Is.EqualTo(1), "Extent should contain exactly 1 item after deletion.");
    }

    [Test]
    public void SaveReservation_WritesCorrectly()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);

        var reservation = new Reservation(
            1,
            startDate,
            endDate,
            ReservationStatus.Pending,
            105m
        );

        // Act
        Reservation.Save();

        // Assert
        Assert.That(File.Exists(_xmlFile), Is.True, "XML file should exist after Save().");
    }

    [Test]
    public void LoadReservation_ReadsCorrectly()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);
        var original = new Reservation(1, startDate, endDate, ReservationStatus.Pending, 105m);
        
        Reservation.Save();
        
        ClearAllExtents();
        Assert.That(Reservation.Reservations.Count, Is.EqualTo(0), "Memory should be empty before load.");

        // Act
        Reservation.Load();

        // Assert
        Assert.That(Reservation.Reservations.Count, Is.EqualTo(1));

        var loaded = Reservation.Reservations.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.ReservationId, Is.EqualTo(1));
            Assert.That(loaded.TotalPrice, Is.EqualTo(105m));
            Assert.That(loaded.Status, Is.EqualTo(ReservationStatus.Pending));
            Assert.That(loaded.StartDate, Is.EqualTo(startDate));
            Assert.That(loaded.EndDate, Is.EqualTo(endDate));
        });
    }
    
    [Test]
    public void TestReservationAddTripShouldCreateReverseConnection()
    {
        // Arrange
        var res = new Reservation(1, DateTime.Today, DateTime.Today.AddDays(5), ReservationStatus.Pending, 1000m);
        var trip = new Trip("Ski Trip", "Alps", DateTime.Today, DateTime.Today.AddDays(5), 500m);

        // Act
        res.AddTrip(trip);

        // Assert
        // Check if Reservation contains the Trip
        Assert.That(res.ReservationsTrips.Contains(trip), Is.True, "Reservation should contain the Trip.");
        
        // Check if Trip contains Reservation 
        Assert.That(trip.Reservations.Contains(res), Is.True, "Trip should automatically contain the Reservation.");
    }

    [Test]
    public void TestReservationRemoveTripShouldRemoveReverseConnection()
    {
        // Arrange
        var res = new Reservation(1, DateTime.Today, DateTime.Today.AddDays(5), ReservationStatus.Pending, 1000m);
        var trip = new Trip("Ski Trip", "Alps", DateTime.Today, DateTime.Today.AddDays(5), 500m);
        res.AddTrip(trip);

        // Act
        res.RemoveTrip(trip);

        // Assert
        Assert.That(res.ReservationsTrips.Contains(trip), Is.False);
        Assert.That(trip.Reservations.Contains(res), Is.False, "Reverse connection in Trip should be removed.");
    }
    
}