using BYT_04.Reservations;
using BYT_04.Vehicles;
using BYT_04_Test.TestUtils;
using NUnit.Framework;

namespace BYT_04_Test.ReservationsTests;

[TestFixture]
public class ReservationVehicleTest
{
    private string _tempDir;
    private string _xmlFile;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "res_vehicle_persistence_tests");
        _xmlFile = Path.Combine(_tempDir, "reservationvehicles.xml");
        
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);

        Directory.CreateDirectory(_tempDir);
        
        ReservationVehicle.SetDirectory(_tempDir);

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
        ClearList.ClearStaticList<ReservationVehicle>("_reservationVehicles");
    }

    //Dummy Data Helpers
    
    private Reservation CreateDummyReservation()
    {
        return new Reservation(1, DateTime.Today, DateTime.Today.AddDays(1), ReservationStatus.Pending, 100m);
    }

    private Vehicle CreateDummyVehicle()
    {
        return new SUV("ABC123", "Toyota", 5, true, new Fuel(100f), true);
    }
    
    //Validation Tests

    [Test]
    public void TestReservationVehicleNullReservation()
    {
        var vehicle = CreateDummyVehicle();
        
        Assert.Throws<ArgumentException>(() => new ReservationVehicle 
        { 
            Reservation = null!, 
            Vehicle = vehicle 
        });
    }

    [Test]
    public void TestReservationVehicleNullVehicle()
    {
        var res = CreateDummyReservation();
        
        Assert.Throws<ArgumentException>(() => new ReservationVehicle 
        { 
            Reservation = res, 
            Vehicle = null! 
        });
    }

    [Test]
    public void TestReservationVehicleInvalidUsagePurpose()
    {
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var rv = new ReservationVehicle(res, vehicle, "Transport", "Good", 50f, 30f);
        
        Assert.Throws<ArgumentException>(() => rv.UsgagePurpose = null!);
        Assert.Throws<ArgumentException>(() => rv.UsgagePurpose = "");
        Assert.Throws<ArgumentException>(() => rv.UsgagePurpose = "   ");
    }

    [Test]
    public void TestReservationVehicleInvalidConditionBefore()
    {
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var rv = new ReservationVehicle(res, vehicle, "Transport", "Good", 50f, 30f);
        
        Assert.Throws<ArgumentException>(() => rv.ConditionBefore = null!);
        Assert.Throws<ArgumentException>(() => rv.ConditionBefore = "");
        Assert.Throws<ArgumentException>(() => rv.ConditionBefore = "   ");
    }

    [Test]
    public void TestReservationVehicleInvalidFuelLevelBefore()
    {
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var rv = new ReservationVehicle(res, vehicle, "Transport", "Good", 50f, 30f);
        
        Assert.Throws<ArgumentException>(() => rv.FuelLevelBefore = -1f);
        Assert.Throws<ArgumentException>(() => rv.FuelLevelBefore = -10f);
    }

    [Test]
    public void TestReservationVehicleInvalidFuelLevelAfter()
    {
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var rv = new ReservationVehicle(res, vehicle, "Transport", "Good", 50f, 30f);
        
        Assert.Throws<ArgumentException>(() => rv.FuelLevelAfter = -1f);
        Assert.Throws<ArgumentException>(() => rv.FuelLevelAfter = -10f);
    }

    [Test]
    public void TestReservationVehicleValidFuelLevels()
    {
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var rv = new ReservationVehicle(res, vehicle, "Transport", "Good", 50f, 30f);
        
        rv.FuelLevelBefore = 0f;
        rv.FuelLevelAfter = 0f;
        rv.FuelLevelBefore = 100f;
        rv.FuelLevelAfter = 75.5f;
        
        Assert.Multiple(() =>
        {
            Assert.That(rv.FuelLevelBefore, Is.EqualTo(100f));
            Assert.That(rv.FuelLevelAfter, Is.EqualTo(75.5f));
        });
    }

    [Test]
    public void TestReservationVehicleConditionAfterCanBeNull()
    {
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var rv = new ReservationVehicle(res, vehicle, "Transport", "Good", 50f, 30f);
        
        rv.ConditionAfter = null;
        Assert.That(rv.ConditionAfter, Is.Null);
        
        rv.ConditionAfter = "";
        Assert.That(rv.ConditionAfter, Is.Null);
        
        rv.ConditionAfter = "   ";
        Assert.That(rv.ConditionAfter, Is.Null);
        
        rv.ConditionAfter = "Excellent";
        Assert.That(rv.ConditionAfter, Is.EqualTo("Excellent"));
    }

    [Test]
    public void TestReservationVehicleNotesCanBeNull()
    {
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var rv = new ReservationVehicle(res, vehicle, "Transport", "Good", 50f, 30f);
        
        rv.Notes = null;
        Assert.That(rv.Notes, Is.Null);
        
        rv.Notes = "";
        Assert.That(rv.Notes, Is.Null);
        
        rv.Notes = "   ";
        Assert.That(rv.Notes, Is.Null);
        
        rv.Notes = "Some notes";
        Assert.That(rv.Notes, Is.EqualTo("Some notes"));
    }

    // Property Tests
    
    [Test]
    public void TestReservationVehicleProperties()
    {
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var rv = new ReservationVehicle(
            res,
            vehicle,
            "Transport",
            "Good",
            50f,
            30f,
            "Excellent",
            "No issues"
        );

        Assert.Multiple(() =>
        {
            Assert.That(rv.Reservation, Is.EqualTo(res));
            Assert.That(rv.Vehicle, Is.EqualTo(vehicle));
            Assert.That(rv.UsgagePurpose, Is.EqualTo("Transport"));
            Assert.That(rv.ConditionBefore, Is.EqualTo("Good"));
            Assert.That(rv.ConditionAfter, Is.EqualTo("Excellent"));
            Assert.That(rv.FuelLevelBefore, Is.EqualTo(50f));
            Assert.That(rv.FuelLevelAfter, Is.EqualTo(30f));
            Assert.That(rv.Notes, Is.EqualTo("No issues"));
        });
    }

    // Extent Tests
    
    [Test]
    public void TestReservationVehicleExtent_ShouldAddReservationVehicle()
    {
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var rv = new ReservationVehicle(res, vehicle, "Transport", "Good", 50f, 30f);
        
        Assert.Multiple(() =>
        {
            Assert.That(ReservationVehicle.ReservationVehicles.Count, Is.EqualTo(1));
            Assert.That(ReservationVehicle.ReservationVehicles.Contains(rv), Is.True);
        });
    }

    // Persistence Tests

    [Test]
    public void SaveReservationVehicle_WritesCorrectly()
    {
        // Arrange
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var rv = new ReservationVehicle(
            res,
            vehicle,
            "Transport",
            "Good",
            50f,
            30f,
            "Excellent",
            "No issues"
        );

        // Act
        ReservationVehicle.Save();

        // Assert
        Assert.That(File.Exists(_xmlFile), Is.True, "XML file should exist after Save().");
    }

    [Test]
    public void LoadReservationVehicle_ReadsCorrectly()
    {
        // Arrange
        var res = CreateDummyReservation();
        var vehicle = CreateDummyVehicle();
        
        var original = new ReservationVehicle(
            res,
            vehicle,
            "Transport",
            "Good",
            50f,
            30f,
            "Excellent",
            "No issues"
        );

        ReservationVehicle.Save();               
        
        ClearAllExtents(); 
        Assert.That(ReservationVehicle.ReservationVehicles.Count, Is.EqualTo(0), "Memory should be empty before load.");

        // Act
        ReservationVehicle.Load();               

        // Assert
        Assert.That(ReservationVehicle.ReservationVehicles.Count, Is.EqualTo(1), "Should have loaded exactly 1 item.");

        var loaded = ReservationVehicle.ReservationVehicles.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.UsgagePurpose, Is.EqualTo("Transport"));
            Assert.That(loaded.ConditionBefore, Is.EqualTo("Good"));
            Assert.That(loaded.ConditionAfter, Is.EqualTo("Excellent"));
            Assert.That(loaded.FuelLevelBefore, Is.EqualTo(50f));
            Assert.That(loaded.FuelLevelAfter, Is.EqualTo(30f));
            Assert.That(loaded.Notes, Is.EqualTo("No issues"));
            Assert.That(loaded.Reservation.ReservationId, Is.EqualTo(res.ReservationId));
            Assert.That(loaded.Vehicle.PlateNumber, Is.EqualTo(vehicle.PlateNumber));
        });
    }

    [Test]
    public void LoadReservationVehicle_MultipleReservationVehicles_ReadsCorrectly()
    {
        // Arrange
        var res1 = CreateDummyReservation();
        var res2 = new Reservation(2, DateTime.Today, DateTime.Today.AddDays(2), ReservationStatus.Pending, 200m);
        var vehicle1 = CreateDummyVehicle();
        var vehicle2 = new ATV("XYZ789", "Mercedes", 4, true, new Electric(50f));
        
        var rv1 = new ReservationVehicle(res1, vehicle1, "Transport", "Good", 50f, 30f, "Excellent", "No issues");
        var rv2 = new ReservationVehicle(res2, vehicle2, "Tour", "Fair", 80f, 60f, null, null);
        
        ReservationVehicle.Save();               
        
        ClearAllExtents(); 
        
        // Act
        ReservationVehicle.Load();               

        // Assert
        Assert.That(ReservationVehicle.ReservationVehicles.Count, Is.EqualTo(2), "Should have loaded exactly 2 items.");
        
        var loaded1 = ReservationVehicle.ReservationVehicles.First(rv => rv.Reservation.ReservationId == 1);
        var loaded2 = ReservationVehicle.ReservationVehicles.First(rv => rv.Reservation.ReservationId == 2);
        
        Assert.Multiple(() =>
        {
            Assert.That(loaded1.UsgagePurpose, Is.EqualTo("Transport"));
            Assert.That(loaded1.ConditionAfter, Is.EqualTo("Excellent"));
            Assert.That(loaded1.Notes, Is.EqualTo("No issues"));
            
            Assert.That(loaded2.UsgagePurpose, Is.EqualTo("Tour"));
            Assert.That(loaded2.ConditionAfter, Is.Null);
            Assert.That(loaded2.Notes, Is.Null);
        });
    }
}

