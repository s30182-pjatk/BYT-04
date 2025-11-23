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
        var reservation = new Reservation { Status = ReservationStatus.Pending };
        reservation.ChangeReservationStatus(ReservationStatus.Cancelled);
        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Cancelled));
    }
    
    [Test]
    public void SaveAndLoadReservation_WritesAndReadsCorrectly()
    {
        // --- Arrange ---
        //clear extent before testing
        var tempDir = Path.Combine(Path.GetTempPath(), "persistence");
        ReservationExtent.SetDirectory(tempDir);
        ReservationExtent.Reservations.Clear();
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);
        
        
        var reservation = new Reservation(
            1,
            startDate, 
            endDate,
            ReservationStatus.Pending,
            105m
        );

        ReservationExtent.Reservations.Add(reservation);

        // --- Act ---
        ReservationExtent.Save();             // Writes to XML
        ReservationExtent.Reservations.Clear(); // Clear memory
        ReservationExtent.Load();             // Reads back from XML
        
        ReservationExtent.DisplayAll(); 

        // --- Assert ---
        Assert.That(ReservationExtent.Reservations.Count, Is.EqualTo(1), "Extent should contain exactly 1 loaded item.");

        var loaded = ReservationExtent.Reservations.First();

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