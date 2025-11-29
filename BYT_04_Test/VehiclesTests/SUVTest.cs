using BYT_04.Vehicles;
using BYT_04_Test.TestUtils;
using NUnit.Framework;

namespace BYT_04_Test.VehiclesTests;

[TestFixture]
public class SUVTest
{
    private string _tempDir;
    private string _xmlFile;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "suv_persistence_tests");
        _xmlFile = Path.Combine(_tempDir, "suvs.xml");
        
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);

        Directory.CreateDirectory(_tempDir);
        
        SUV.SetDirectory(_tempDir);

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
        ClearList.ClearStaticList<SUV>("_suvs");
        ClearList.ClearStaticList<Vehicle>("_vehicles");
    }
    
    // Property Tests
    
    [Test]
    public void TestSUVProperties()
    {
        var fuel = new Fuel(100f);
        var suv = new SUV("ABC123", "Toyota", 5, true, fuel, true);

        Assert.Multiple(() =>
        {
            Assert.That(suv.PlateNumber, Is.EqualTo("ABC123"));
            Assert.That(suv.Model, Is.EqualTo("Toyota"));
            Assert.That(suv.Capacity, Is.EqualTo(5));
            Assert.That(suv.ContainMedKit, Is.True);
            Assert.That(suv.HasWinch, Is.True);
            Assert.That(suv.PowerType, Is.EqualTo(fuel));
        });
    }

    [Test]
    public void TestSUVStaticProperties()
    {
        SUV.HasWinterTires = true;
        SUV.IsFourWheelDrive = true;
        
        Assert.Multiple(() =>
        {
            Assert.That(SUV.HasWinterTires, Is.True);
            Assert.That(SUV.IsFourWheelDrive, Is.True);
        });
    }

    [Test]
    public void TestSUVMaxSpeed()
    {
        var suv = new SUV("ABC123", "Toyota", 5, true, new Fuel(100f), true);
        suv.MaxSpeedInKpH = 120.5f;
        
        Assert.That(suv.MaxSpeedInKpH, Is.EqualTo(120.5f));
    }

    // Extent Tests
    
    [Test]
    public void TestSUVExtent_ShouldAddSUV()
    {
        var suv = new SUV("ABC123", "Toyota", 5, true, new Fuel(100f), true);
        
        Assert.Multiple(() =>
        {
            Assert.That(SUV.SUVs.Count, Is.EqualTo(1));
            Assert.That(SUV.SUVs.Contains(suv), Is.True);
            Assert.That(Vehicle.Vehicles.Count, Is.EqualTo(1));
        });
    }

    // Persistence Tests
    
    [Test]
    public void SaveSUV_WritesCorrectly()
    {
        // Arrange
        var fuel = new Fuel(100f);
        var suv = new SUV("ABC123", "Toyota", 5, true, fuel, true);

        // Act
        SUV.Save();

        // Assert
        Assert.That(File.Exists(_xmlFile), Is.True, "XML file should exist after Save().");
    }

    [Test]
    public void LoadSUV_ReadsCorrectly()
    {
        // Arrange
        var fuel = new Fuel(100f);
        var original = new SUV("ABC123", "Toyota", 5, true, fuel, true);
        original.MaxSpeedInKpH = 120.5f;
        
        SUV.Save();
        
        ClearAllExtents();
        Assert.That(SUV.SUVs.Count, Is.EqualTo(0), "Memory should be empty before load.");

        // Act
        SUV.Load();

        // Assert
        Assert.That(SUV.SUVs.Count, Is.EqualTo(1));

        var loaded = SUV.SUVs.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.PlateNumber, Is.EqualTo("ABC123"));
            Assert.That(loaded.Model, Is.EqualTo("Toyota"));
            Assert.That(loaded.Capacity, Is.EqualTo(5));
            Assert.That(loaded.ContainMedKit, Is.True);
            Assert.That(loaded.HasWinch, Is.True);
            Assert.That(loaded.MaxSpeedInKpH, Is.EqualTo(120.5f));
        });
    }

    [Test]
    public void LoadSUV_MultipleSUVs_ReadsCorrectly()
    {
        // Arrange
        var fuel1 = new Fuel(100f);
        var fuel2 = new Fuel(150f);
        var suv1 = new SUV("ABC123", "Toyota", 5, true, fuel1, true);
        var suv2 = new SUV("XYZ789", "Honda", 7, false, fuel2, false);
        
        SUV.Save();
        
        ClearAllExtents();
        
        // Act
        SUV.Load();

        // Assert
        Assert.That(SUV.SUVs.Count, Is.EqualTo(2));
        
        var loaded1 = SUV.SUVs.First(s => s.PlateNumber == "ABC123");
        var loaded2 = SUV.SUVs.First(s => s.PlateNumber == "XYZ789");
        
        Assert.Multiple(() =>
        {
            Assert.That(loaded1.HasWinch, Is.True);
            Assert.That(loaded2.HasWinch, Is.False);
            Assert.That(loaded1.ContainMedKit, Is.True);
            Assert.That(loaded2.ContainMedKit, Is.False);
        });
    }
}

