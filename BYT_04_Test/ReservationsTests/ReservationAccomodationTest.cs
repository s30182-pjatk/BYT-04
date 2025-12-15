using BYT_04;
using BYT_04_Test.TestUtils;
using BYT_04.Reservations;

namespace BYT_04_Test.ReservationsTests;

[TestFixture]
public class ReservationAccomodationTest
{
    private string _tempDir;
    private string _xmlFile;
    private Customer _testCustomer;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "res_acc_persistence_tests");
        _xmlFile = Path.Combine(_tempDir, "reservationaccomodation.xml");
        _testCustomer = new Customer(
            "Test",
            null,
            "TripCustomer",
            DateTime.Today.AddYears(-30),
            "Male",
            "123456789",
            "trip@example.com",
            new Address("1 Test St", "Test City", "TS", "12345", "USA"),
            false,
            0
        );
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);

        Directory.CreateDirectory(_tempDir);

        ReservationAccomodation.SetDirectory(_tempDir);

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
        ClearList.ClearStaticList<ReservationAccomodation>("_reservationAccomodations");
    }

    //Dummy Data Helpers

    private Reservation CreateDummyReservation()
    {
        // If Reservation adds itself to a static list in constructor, it might persist unless cleared.
        return _testCustomer.CreateReservation(
            1,
            DateTime.Today,
            DateTime.Today.AddDays(1),
            ReservationStatus.Pending,
            100m
        );
    }

    private Accomodation CreateDummyAccomodation()
    {
        return new Accomodation("101", AccomodationType.Room, 2);
    }

    //Validation Tests

    [Test]
    public void TestReservationAccomodationZeroGuests()
    {
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();

        // Test 0 guests
        Assert.Throws<ArgumentException>(() =>
            new ReservationAccomodation
            {
                Reservation = res,
                Accomodation = acc,
                NumberOfGuests = 0,
            }
        );
    }

    [Test]
    public void TestReservationAccomodationNegativeGuests()
    {
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();

        // Test negative guests
        Assert.Throws<ArgumentException>(() =>
            new ReservationAccomodation
            {
                Reservation = res,
                Accomodation = acc,
                NumberOfGuests = -5,
            }
        );
    }

    [Test]
    public void TestReservationAccomodationInvalidCheckInDate()
    {
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();

        var ra = new ReservationAccomodation(
            res,
            acc,
            2,
            DateTime.Today,
            DateTime.Today.AddDays(1),
            "Ok"
        );

        var futureDate = DateTime.Today.AddDays(10);

        // CheckIn cannot be > Today
        Assert.Throws<ArgumentException>(() => ra.CheckInDate = futureDate);
    }

    [Test]
    public void TestReservationAccomodationInvalidCheckOutDate()
    {
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();

        var checkIn = DateTime.Today;
        var invalidCheckOut = DateTime.Today.AddDays(-1); // Before CheckIn

        var ra = new ReservationAccomodation(res, acc, 2, checkIn, DateTime.Today.AddDays(1), "Ok");

        Assert.Throws<ArgumentException>(() => ra.CheckOutDate = invalidCheckOut);
    }

    [Test]
    public void TestReservationAccomodationNullReservation()
    {
        var acc = CreateDummyAccomodation();

        Assert.Throws<ArgumentException>(() =>
            new ReservationAccomodation { Reservation = null!, Accomodation = acc }
        );
    }

    [Test]
    public void TestReservationAccomodationNullAccomodation()
    {
        var res = CreateDummyReservation();

        Assert.Throws<ArgumentException>(() =>
            new ReservationAccomodation { Reservation = res, Accomodation = null! }
        );
    }

    // Persistence Tests

    [Test]
    public void SaveReservationAccomodation_WritesCorrectly()
    {
        // Arrange
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();

        var ra = new ReservationAccomodation(
            res,
            acc,
            numberOfGuests: 2,
            checkInDate: DateTime.Today,
            checkOutDate: DateTime.Today.AddDays(4),
            conditionBefore: "Good",
            notes: "Heater check"
        );

        // Act
        ReservationAccomodation.Save();

        // Assert
        Assert.That(File.Exists(_xmlFile), Is.True, "XML file should exist after Save().");
    }

    [Test]
    public void LoadReservationAccomodation_ReadsCorrectly()
    {
        // Arrange
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();

        var checkIn = DateTime.Today;
        var checkOut = DateTime.Today.AddDays(4);

        var original = new ReservationAccomodation(
            res,
            acc,
            numberOfGuests: 2,
            checkIn,
            checkOut,
            conditionBefore: "Good",
            notes: "Heater needs to be fixed"
        );

        ReservationAccomodation.Save();

        ClearAllExtents();
        Assert.That(
            ReservationAccomodation.ReservationAccomodations.Count,
            Is.EqualTo(0),
            "Memory should be empty before load."
        );

        // Act
        ReservationAccomodation.Load();

        // Assert
        Assert.That(
            ReservationAccomodation.ReservationAccomodations.Count,
            Is.EqualTo(1),
            "Should have loaded exactly 1 item."
        );

        var loaded = ReservationAccomodation.ReservationAccomodations.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.NumberOfGuests, Is.EqualTo(2));
            Assert.That(loaded.ConditionBefore, Is.EqualTo("Good"));
            Assert.That(loaded.Notes, Is.EqualTo("Heater needs to be fixed"));
            Assert.That(loaded.CheckInDate, Is.EqualTo(checkIn));
            Assert.That(loaded.CheckOutDate, Is.EqualTo(checkOut));
            Assert.That(loaded.Reservation.ReservationId, Is.EqualTo(res.ReservationId));
            Assert.That(loaded.Accomodation.Number, Is.EqualTo(acc.Number));
        });
    }

    // Association Tests

    [Test]
    public void TestReservationConstructorCreatesReverseConnectionsAutomatically()
    {
        // Arrange
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();

        // Act
        var ra = new ReservationAccomodation(
            res,
            acc,
            2,
            DateTime.Today,
            DateTime.Today.AddDays(1),
            "Good"
        );

        // Assert
        // 1. Check Reservation knows about the link
        Assert.That(
            res.ReservationAccomodations.Contains(ra),
            Is.True,
            "Reservation should contain the link in its collection."
        );

        // 2. Check Accomodation knows about the link
        Assert.That(
            acc.ReservationAccomodations.Contains(ra),
            Is.True,
            "Accomodation should contain the link in its collection."
        );
    }

    [Test]
    public void TestReservationRemoveLinkRemovesReverseConnection()
    {
        // Arrange
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();
        var ra = new ReservationAccomodation(
            res,
            acc,
            2,
            DateTime.Today,
            DateTime.Today.AddDays(1),
            "Good"
        );

        // Pre-check
        Assert.That(acc.ReservationAccomodations.Contains(ra), Is.True);

        // Act
        // remove ink from one side
        acc.RemoveReservationAccomodation(ra);

        // Assert
        Assert.That(
            acc.ReservationAccomodations.Contains(ra),
            Is.False,
            "Link should be removed from Accomodation collection."
        );
    }

    [Test]
    public void BagConstraint_AllowsMultipleLinks_BetweenSameObjects()
    {
        // Arrange
        var res = CreateDummyReservation();
        var acc = CreateDummyAccomodation();

        // Act
        // Link 1
        var ra1 = new ReservationAccomodation(
            res,
            acc,
            2,
            DateTime.Today,
            DateTime.Today.AddDays(1),
            "Good"
        );

        // Link 2 (Same Reservation, Same Room, distinct link object)
        var ra2 = new ReservationAccomodation(
            res,
            acc,
            2,
            DateTime.Today,
            DateTime.Today.AddDays(6),
            "Good"
        );

        // Assert
        Assert.That(acc.ReservationAccomodations.Count(), Is.EqualTo(2));
        Assert.That(acc.ReservationAccomodations.Contains(ra1), Is.True);
        Assert.That(acc.ReservationAccomodations.Contains(ra2), Is.True);
    }
}
