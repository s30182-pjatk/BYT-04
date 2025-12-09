using BYT_04.Vehicles;
using BYT_04_Test.TestUtils;
using BYT_04.Reservations;
using NUnit.Framework;

namespace BYT_04_Test.VehiclesTests;

[TestFixture]
public class VehicleTest
{
    private string _tempDir;
    private string _xmlFile;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "vehicle_persistence_tests");
        _xmlFile = Path.Combine(_tempDir, "vehicles.xml");
        
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);

        Directory.CreateDirectory(_tempDir);
        
        Vehicle.SetDirectory(_tempDir);

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
        ClearList.ClearStaticList<Vehicle>("_vehicles");
    }
    
    // Validation Tests
    
    [Test]
    public void TestVehicleInvalidPlateNumber()
    {
        var vehicle = new SUV("1234567890", "Toyota", 5, true, new Fuel(100f), true);
        Assert.Throws<ArgumentException>(() => vehicle.PlateNumber = null!);
        Assert.Throws<ArgumentException>(() => vehicle.PlateNumber = "");
        Assert.Throws<ArgumentException>(() => vehicle.PlateNumber = "   ");
    }

    [Test]
    public void TestVehicleInvalidModel()
    {
        var vehicle = new SUV("1234567890", "Toyota", 5, true, new Fuel(100f), true);
        Assert.Throws<ArgumentException>(() => vehicle.Model = null!);
        Assert.Throws<ArgumentException>(() => vehicle.Model = "");
        Assert.Throws<ArgumentException>(() => vehicle.Model = "   ");
    }

    [Test]
    public void TestVehicleInvalidCapacity()
    {
        var vehicle = new SUV("1234567890", "Toyota", 5, true, new Fuel(100f), true);
        Assert.Throws<ArgumentOutOfRangeException>(() => vehicle.Capacity = 0);
        Assert.Throws<ArgumentOutOfRangeException>(() => vehicle.Capacity = -1);
    }

    [Test]
    public void TestVehicleInvalidPowerType()
    {
        var vehicle = new SUV("1234567890", "Toyota", 5, true, new Fuel(100f), true);
        Assert.Throws<ArgumentNullException>(() => vehicle.PowerType = null!);
    }

    // Property Tests
    
    [Test]
    public void TestVehicleProperties()
    {
        var fuel = new Fuel(100f);
        var vehicle = new SUV("ABC123", "Toyota", 5, true, fuel, true);

        Assert.Multiple(() =>
        {
            Assert.That(vehicle.PlateNumber, Is.EqualTo("ABC123"));
            Assert.That(vehicle.Model, Is.EqualTo("Toyota"));
            Assert.That(vehicle.Capacity, Is.EqualTo(5));
            Assert.That(vehicle.ContainMedKit, Is.True);
            Assert.That(vehicle.PowerType, Is.EqualTo(fuel));
        });
    }

    // Extent Tests
    
    [Test]
    public void TestVehicleExtent_ShouldAddVehicle()
    {
        var vehicle = new SUV("ABC123", "Toyota", 5, true, new Fuel(100f), true);
        
        Assert.That(Vehicle.Vehicles.Count, Is.EqualTo(1));
        Assert.That(Vehicle.Vehicles.Contains(vehicle), Is.True);
    }

    // Persistence Tests
    
    [Test]
    public void SaveVehicle_WritesCorrectly()
    {
        // Arrange
        var fuel = new Fuel(100f);
        var vehicle = new SUV("ABC123", "Toyota", 5, true, fuel, true);

        // Act
        Vehicle.Save();

        // Assert
        Assert.That(File.Exists(_xmlFile), Is.True, "XML file should exist after Save().");
    }

    [Test]
    public void LoadVehicle_ReadsCorrectly()
    {
        // Arrange
        var fuel = new Fuel(100f);
        var original = new SUV("ABC123", "Toyota", 5, true, fuel, true);
        
        Vehicle.Save();
        
        ClearAllExtents();
        Assert.That(Vehicle.Vehicles.Count, Is.EqualTo(0), "Memory should be empty before load.");

        // Act
        Vehicle.Load();

        // Assert
        Assert.That(Vehicle.Vehicles.Count, Is.EqualTo(1));

        var loaded = Vehicle.Vehicles.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.PlateNumber, Is.EqualTo("ABC123"));
            Assert.That(loaded.Model, Is.EqualTo("Toyota"));
            Assert.That(loaded.Capacity, Is.EqualTo(5));
            Assert.That(loaded.ContainMedKit, Is.True);
            Assert.That(loaded, Is.InstanceOf<SUV>());
        });
    }

    [Test]
    public void LoadVehicle_MultipleVehicles_ReadsCorrectly()
    {
        // Arrange
        var fuel = new Fuel(100f);
        var electric = new Electric(50f);
        var suv = new SUV("ABC123", "Toyota", 5, true, fuel, true);
        var atv = new ATV("XYZ789", "Mercedes", 4, true, electric);
        
        Vehicle.Save();
        
        ClearAllExtents();
        
        // Act
        Vehicle.Load();

        // Assert
        Assert.That(Vehicle.Vehicles.Count, Is.EqualTo(2));
        
        var loadedSuv = Vehicle.Vehicles.OfType<SUV>().First();
        var loadedAtv = Vehicle.Vehicles.OfType<ATV>().First();
        
        Assert.Multiple(() =>
        {
            Assert.That(loadedSuv.PlateNumber, Is.EqualTo("ABC123"));
            Assert.That(loadedAtv.PlateNumber, Is.EqualTo("XYZ789"));
        });
    }
    
    [Test]
    public void TestTripVehicleAssociationShouldCreateReverseConnection()
    {
        // Arrange
        var trip = new Trip("Safari", "Africa", DateTime.Today, DateTime.Today.AddDays(5), 1000m);
        var vehicle = new SUV("SAFARI-01", "Jeep", 6, true, new Fuel(100f), true);

        // Act
        trip.AddVehicle(vehicle);

        // Assert
        // check if Trip contains Vehicle
        Assert.That(trip.Vehicles.Contains(vehicle), Is.True);
    
        // check if Vehicle contains Trip (Reverse Connection)
        Assert.That(vehicle.Trips.Contains(trip), Is.True);
    }
}

