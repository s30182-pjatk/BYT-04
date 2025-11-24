using System.Reflection;
using BYT_04.Reservations;
using NUnit.Framework;

namespace BYT_04_Test.ReservationsTests;

public class ReservationTest
{
    
    [Test]
    public void TestReservationInvalidStartDate()
    {   
        var invalidDate = DateTime.Today.AddDays(-1);
        Assert.Throws<ArgumentException>(() => new Reservation { StartDate = invalidDate});
    }
    
    [Test]
    public void TestReservationInvalidEndDate()
    {   
        var startDate = DateTime.Today.AddDays(5);
        var endDate = DateTime.Today.AddDays(4);
        Assert.Throws<ArgumentException>(() => new Reservation {StartDate = startDate, EndDate = endDate});
    }

    [Test]
    public void TestReservationInvalidTotalPrice()
    {
        var invalidPrice = -100m;
        Assert.Throws<ArgumentException>(() => new Reservation { TotalPrice = invalidPrice});
        
    }

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

    [Test]
    public void TestReservationCheckPendingReservations()
    {
        // -- Arrange --
        var tempDir = Path.Combine(Path.GetTempPath(), "reservation_persistence_tests");
        Reservation.SetDirectory(tempDir);
        
        var fieldInfo = typeof(Reservation).GetField("_reservations", BindingFlags.NonPublic | BindingFlags.Static);
        var list = (List<Reservation>)fieldInfo.GetValue(null);
        list.Clear();
        
        var r1 = new Reservation(1, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
        var r2 = new Reservation(2, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
        var r3 = new Reservation(3, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Completed, 100m);
        
        // -- Act --
        Reservation.Save();             // Writes to XML
        Reservation.Load();             // Reads back from XML
        
        var pendingList = Reservation.CheckPendingReservations();

        // --- Assert ---
        Assert.That(Reservation.Reservations.Count, Is.EqualTo(3), "Should have loaded all 3 reservations.");
        Assert.That(pendingList.Count, Is.EqualTo(2), "Should filter down to 2 pending reservations.");
        
        // --- Cleanup ---
        if(Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
    }
    
    [Test]
    public void TestReservationRemoveCompletedReservations()
    {
        // -- Arrange --
        var tempDir = Path.Combine(Path.GetTempPath(), "reservation_persistence_tests");
        Reservation.SetDirectory(tempDir);
        
        var fieldInfo = typeof(Reservation).GetField("_reservations", BindingFlags.NonPublic | BindingFlags.Static);
        var list = (List<Reservation>)fieldInfo.GetValue(null);
        list.Clear();
        
        var r1 = new Reservation(1, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
        r1.ChangeReservationStatus(ReservationStatus.Completed);
        
        var r2 = new Reservation(2, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
        r2.ChangeReservationStatus(ReservationStatus.Completed);
        
        var r3 = new Reservation(3, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
        
        // -- Act --
        Reservation.Save();             // Writes to XML
        Reservation.Load();             // Reads back from XML
        
        Assert.That(Reservation.Reservations.Count, Is.EqualTo(3), "Extent should contain exactly 3 loaded item before deletion of completed reservations.");
        Reservation.RemoveCompletedReservations();
        Assert.That(Reservation.Reservations.Count, Is.EqualTo(1), "Extent should contain exactly 1 loaded item before deletion of completed reservations.");
    }
    
    [Test]
    public void SaveReservation_WritesCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "reservation_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "reservations.xml");

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

        Reservation.SetDirectory(tempDir);
        
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
        Assert.That(File.Exists(xmlFile), Is.True,
            "XML file should exist after Save().");
    }
    
    [Test]
    public void LoadReservation_ReadsCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "reservation_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "reservations.xml");
            
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);
        
        Reservation.SetDirectory(tempDir);

        // Ensure XML exists (if this test runs alone)
        if (!File.Exists(xmlFile))
            SaveReservation_WritesCorrectly();

        var fieldInfo = typeof(Reservation).GetField("_reservations", BindingFlags.NonPublic | BindingFlags.Static);
        var list = (List<Reservation>)fieldInfo.GetValue(null);
        list.Clear();

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
    
}