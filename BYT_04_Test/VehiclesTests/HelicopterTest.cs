using BYT_04.Vehicles;
using BYT_04_Test.TestUtils;
using NUnit.Framework;

namespace BYT_04_Test.VehiclesTests;

[TestFixture]
public class HelicopterTest
{
    private string _tempDir;
    private string _xmlFile;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "helicopter_persistence_tests");
        _xmlFile = Path.Combine(_tempDir, "helicopters.xml");
        
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);

        Directory.CreateDirectory(_tempDir);
        
        Helicoper.SetDirectory(_tempDir);

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
        ClearList.ClearStaticList<Helicoper>("_helicopters");
        ClearList.ClearStaticList<Vehicle>("_vehicles");
    }
    
    // Property Tests
    
    [Test]
    public void TestHelicopterProperties()
    {
        var fuel = new Fuel(200f);
        var helicopter = new Helicoper("HELI001", "Bell", 6, true, fuel);

        Assert.Multiple(() =>
        {
            Assert.That(helicopter.PlateNumber, Is.EqualTo("HELI001"));
            Assert.That(helicopter.Model, Is.EqualTo("Bell"));
            Assert.That(helicopter.Capacity, Is.EqualTo(6));
            Assert.That(helicopter.ContainMedKit, Is.True);
            Assert.That(helicopter.PowerType, Is.EqualTo(fuel));
        });
    }

    // Extent Tests
    
    [Test]
    public void TestHelicopterExtent_ShouldAddHelicopter()
    {
        var helicopter = new Helicoper("HELI001", "Bell", 6, true, new Fuel(200f));
        
        Assert.Multiple(() =>
        {
            Assert.That(Helicoper.Helicopters.Count, Is.EqualTo(1));
            Assert.That(Helicoper.Helicopters.Contains(helicopter), Is.True);
            Assert.That(Vehicle.Vehicles.Count, Is.EqualTo(1));
        });
    }

    // Persistence Tests
    
    [Test]
    public void SaveHelicopter_WritesCorrectly()
    {
        // Arrange
        var fuel = new Fuel(200f);
        var helicopter = new Helicoper("HELI001", "Bell", 6, true, fuel);

        // Act
        Helicoper.Save();

        // Assert
        Assert.That(File.Exists(_xmlFile), Is.True, "XML file should exist after Save().");
    }

    [Test]
    public void LoadHelicopter_ReadsCorrectly()
    {
        // Arrange
        var fuel = new Fuel(200f);
        var original = new Helicoper("HELI001", "Bell", 6, true, fuel);
        
        Helicoper.Save();
        
        ClearAllExtents();
        Assert.That(Helicoper.Helicopters.Count, Is.EqualTo(0), "Memory should be empty before load.");

        // Act
        Helicoper.Load();

        // Assert
        Assert.That(Helicoper.Helicopters.Count, Is.EqualTo(1));

        var loaded = Helicoper.Helicopters.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.PlateNumber, Is.EqualTo("HELI001"));
            Assert.That(loaded.Model, Is.EqualTo("Bell"));
            Assert.That(loaded.Capacity, Is.EqualTo(6));
            Assert.That(loaded.ContainMedKit, Is.True);
        });
    }

    [Test]
    public void LoadHelicopter_MultipleHelicopters_ReadsCorrectly()
    {
        // Arrange
        var fuel1 = new Fuel(200f);
        var fuel2 = new Fuel(300f);
        var helicopter1 = new Helicoper("HELI001", "Bell", 6, true, fuel1);
        var helicopter2 = new Helicoper("HELI002", "Robinson", 4, false, fuel2);
        
        Helicoper.Save();
        
        ClearAllExtents();
        
        // Act
        Helicoper.Load();

        // Assert
        Assert.That(Helicoper.Helicopters.Count, Is.EqualTo(2));
        
        var loaded1 = Helicoper.Helicopters.First(h => h.PlateNumber == "HELI001");
        var loaded2 = Helicoper.Helicopters.First(h => h.PlateNumber == "HELI002");
        
        Assert.Multiple(() =>
        {
            Assert.That(loaded1.Model, Is.EqualTo("Bell"));
            Assert.That(loaded2.Model, Is.EqualTo("Robinson"));
            Assert.That(loaded1.ContainMedKit, Is.True);
            Assert.That(loaded2.ContainMedKit, Is.False);
        });
    }
}

