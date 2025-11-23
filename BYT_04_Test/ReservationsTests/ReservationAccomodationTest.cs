using BYT_04.Reservations;

namespace BYT_04_Test.ReservationsTests;

public class ReservationAccomodationTest
{
    // --- dummy objects to satisfy the constructor ---
    private Reservation CreateDummyReservation()
    {
        return new Reservation(1, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
    }

    private Accomodation CreateDummyAccomodation()
    {
        return new Accomodation("101", AccomodationType.Room, 2);
    }

    [Test]
    public void TestReservationAccomodationInvalidGuests()
    {
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();
        
        // Test 0 guests
        Assert.Throws<ArgumentException>(() => new ReservationAccomodation 
        { 
            Reservation = res, 
            Accomodation = acc, 
            NumberOfGuests = 0 
        });

        // Test negative guests
        Assert.Throws<ArgumentException>(() => new ReservationAccomodation 
        { 
            Reservation = res, 
            Accomodation = acc, 
            NumberOfGuests = -5 
        });
    }

    [Test]
    public void TestReservationAccomodationInvalidCheckInDate()
    {
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();
        var reservationaccomodation = new ReservationAccomodation { Reservation = res, Accomodation = acc };
        
        var futureDate = DateTime.Today.AddDays(1);

        Assert.Throws<ArgumentException>(() => reservationaccomodation.CheckInDate = futureDate);
    }

    [Test]
    public void TestReservationAccomodationInvalidCheckOutDate()
    {
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();
        
        var checkIn = DateTime.Today;
        var invalidCheckOut = DateTime.Today.AddDays(-1); // Before CheckIn

        // Initialize with valid CheckIn first
        var reservationaccomodation = new ReservationAccomodation 
        { 
            Reservation = res, 
            Accomodation = acc,
            CheckInDate = checkIn
        };

        Assert.Throws<ArgumentException>(() => reservationaccomodation.CheckOutDate = invalidCheckOut);
    }

    [Test]
    public void TestReservationAccomodationNullReferences()
    {
        var acc = CreateDummyAccomodation();
        var res = CreateDummyReservation();

        // Test Null Reservation
        Assert.Throws<ArgumentException>(() => new ReservationAccomodation 
        { 
            Reservation = null!, 
            Accomodation = acc 
        });

        // Test Null Accomodation
        Assert.Throws<ArgumentException>(() => new ReservationAccomodation 
        { 
            Reservation = res, 
            Accomodation = null! 
        });
    }

    [Test]
    public void SaveAndLoadReservationAccomodation_WritesAndReadsCorrectly()
    {
        // --- Arrange ---
        var tempDir = Path.Combine(Path.GetTempPath(), "persistence");
        ReservationAccomodationExtent.SetDirectory(tempDir);
        ReservationAccomodationExtent.ReservationAccomodations.Clear();

        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();
        
        var checkIn = DateTime.Today;
        var checkOut = DateTime.Today.AddDays(4);

        var reservationaccomodation = new ReservationAccomodation(
            res,
            acc,
            numberOfGuests: 2,
            checkIn,
            checkOut,
            conditionBefore: "Good",
            notes: "Heater needs to be fixed"
        );

        ReservationAccomodationExtent.ReservationAccomodations.Add(reservationaccomodation);

        // --- Act ---
        ReservationAccomodationExtent.Save();               // Writes to XML
        ReservationAccomodationExtent.ReservationAccomodations.Clear(); // Clear memory
        ReservationAccomodationExtent.Load();               // Reads back from XML
        
        ReservationAccomodationExtent.DisplayAll();

        // --- Assert ---
        Assert.That(ReservationAccomodationExtent.ReservationAccomodations.Count, Is.EqualTo(1), "Extent should contain exactly 1 loaded item.");

        var loaded = ReservationAccomodationExtent.ReservationAccomodations.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.NumberOfGuests, Is.EqualTo(2));
            Assert.That(loaded.ConditionBefore, Is.EqualTo("Good"));
            Assert.That(loaded.Notes, Is.EqualTo("Heater needs to be fixed"));
            Assert.That(loaded.CheckInDate, Is.EqualTo(checkIn));
            Assert.That(loaded.CheckOutDate, Is.EqualTo(checkOut));
        });

        // --- Cleanup ---
        if(Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
    }
}