using BYT_04.Vehicles;
using BYT_04_Test.TestUtils;
using NUnit.Framework;

namespace BYT_04_Test.VehiclesTests;

[TestFixture]
public class ATVTest
{
    private string _tempDir;
    private string _xmlFile;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "atv_persistence_tests");
        _xmlFile = Path.Combine(_tempDir, "atvs.xml");
        
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);

        Directory.CreateDirectory(_tempDir);
        
        ATV.SetDirectory(_tempDir);

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
        ClearList.ClearStaticList<ATV>("_atvs");
        ClearList.ClearStaticList<Vehicle>("_vehicles");
    }
    
    // Property Tests
    
    [Test]
    public void TestATVProperties()
    {
        var electric = new Electric(50f);
        var atv = new ATV("ABC123", "Mercedes", 4, true, electric);

        Assert.Multiple(() =>
        {
            Assert.That(atv.PlateNumber, Is.EqualTo("ABC123"));
            Assert.That(atv.Model, Is.EqualTo("Mercedes"));
            Assert.That(atv.Capacity, Is.EqualTo(4));
            Assert.That(atv.ContainMedKit, Is.True);
            Assert.That(atv.PowerType, Is.EqualTo(electric));
        });
    }

    [Test]
    public void TestATVStaticProperties()
    {
        ATV.HasWinterTires = true;
        ATV.IsFourWheelDrive = true;
        
        Assert.Multiple(() =>
        {
            Assert.That(ATV.HasWinterTires, Is.True);
            Assert.That(ATV.IsFourWheelDrive, Is.True);
        });
    }

    [Test]
    public void TestATVMaxSpeed()
    {
        var atv = new ATV("ABC123", "Mercedes", 4, true, new Electric(50f));
        atv.MaxSpeedInKpH = 80.0f;
        
        Assert.That(atv.MaxSpeedInKpH, Is.EqualTo(80.0f));
    }

    [Test]
    public void TestATVCargoWrack()
    {
        var atv = new ATV("ABC123", "Mercedes", 4, true, new Electric(50f));
        atv.HasGargoWrack = true;
        
        Assert.That(atv.HasGargoWrack, Is.True);
    }

    // Extent Tests
    
    [Test]
    public void TestATVExtent_ShouldAddATV()
    {
        var atv = new ATV("ABC123", "Mercedes", 4, true, new Electric(50f));
        
        Assert.Multiple(() =>
        {
            Assert.That(ATV.ATVs.Count, Is.EqualTo(1));
            Assert.That(ATV.ATVs.Contains(atv), Is.True);
            Assert.That(Vehicle.Vehicles.Count, Is.EqualTo(1));
        });
    }

    // Persistence Tests
    
    [Test]
    public void SaveATV_WritesCorrectly()
    {
        // Arrange
        var electric = new Electric(50f);
        var atv = new ATV("ABC123", "Mercedes", 4, true, electric);

        // Act
        ATV.Save();

        // Assert
        Assert.That(File.Exists(_xmlFile), Is.True, "XML file should exist after Save().");
    }

    [Test]
    public void LoadATV_ReadsCorrectly()
    {
        // Arrange
        var electric = new Electric(50f);
        var original = new ATV("ABC123", "Mercedes", 4, true, electric);
        original.MaxSpeedInKpH = 80.0f;
        original.HasGargoWrack = true;
        
        ATV.Save();
        
        ClearAllExtents();
        Assert.That(ATV.ATVs.Count, Is.EqualTo(0), "Memory should be empty before load.");

        // Act
        ATV.Load();

        // Assert
        Assert.That(ATV.ATVs.Count, Is.EqualTo(1));

        var loaded = ATV.ATVs.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.PlateNumber, Is.EqualTo("ABC123"));
            Assert.That(loaded.Model, Is.EqualTo("Mercedes"));
            Assert.That(loaded.Capacity, Is.EqualTo(4));
            Assert.That(loaded.ContainMedKit, Is.True);
            Assert.That(loaded.MaxSpeedInKpH, Is.EqualTo(80.0f));
            Assert.That(loaded.HasGargoWrack, Is.True);
        });
    }

    [Test]
    public void LoadATV_MultipleATVs_ReadsCorrectly()
    {
        // Arrange
        var electric1 = new Electric(50f);
        var electric2 = new Electric(75f);
        var atv1 = new ATV("ABC123", "Mercedes", 4, true, electric1);
        var atv2 = new ATV("XYZ789", "Yamaha", 2, false, electric2);
        atv1.HasGargoWrack = true;
        atv2.HasGargoWrack = false;
        
        ATV.Save();
        
        ClearAllExtents();
        
        // Act
        ATV.Load();

        // Assert
        Assert.That(ATV.ATVs.Count, Is.EqualTo(2));
        
        var loaded1 = ATV.ATVs.First(a => a.PlateNumber == "ABC123");
        var loaded2 = ATV.ATVs.First(a => a.PlateNumber == "XYZ789");
        
        Assert.Multiple(() =>
        {
            Assert.That(loaded1.HasGargoWrack, Is.True);
            Assert.That(loaded2.HasGargoWrack, Is.False);
            Assert.That(loaded1.ContainMedKit, Is.True);
            Assert.That(loaded2.ContainMedKit, Is.False);
        });
    }
}

