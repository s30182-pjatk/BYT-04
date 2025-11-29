using BYT_04.Reservations;
using BYT_04_Test.TestUtils;

namespace BYT_04_Test.ReservationsTests;

[TestFixture]
public class AccomodationTest
{
    private string _tempDir;
    private string _xmlFile;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "accomodation_persistence_tests");
        _xmlFile = Path.Combine(_tempDir, "accomodations.xml");
        
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
        
        Directory.CreateDirectory(_tempDir);
        
        Accomodation.SetDirectory(_tempDir);
        
        // Prevent data bleeding between tests
        ClearAllExtents();
    }

    [TearDown]
    public void Cleanup()
    {
        // Clean up files after every test
        if (Directory.Exists(_tempDir)) 
            Directory.Delete(_tempDir, true);
        
        ClearAllExtents();
    }
    
    private void ClearAllExtents()
    {
        //Helper to clear static lists via reflection
        ClearList.ClearStaticList<Accomodation>("_accomodations");
    }
    
    //Logic Tests
    
    [Test]
    public void TestAccomodationCorrectCapacity()
    {
        int capacity = 4;
        var accomodation = new Accomodation { Capacity = 4 };
        Assert.That(accomodation.Capacity, Is.EqualTo(capacity));
    }

    [Test]
    public void TestAccomodationInvalidCapacity()
    {
        int invalidcapacity = -4;
        Assert.Throws<ArgumentException>(() => new Accomodation { Capacity = invalidcapacity });
    }

    [Test]
    public void TestAccomodationCorrectAccomodationType()
    {
        var accomodation = new Accomodation { Type = AccomodationType.Cabin };
        Assert.That(accomodation.Type, Is.EqualTo(AccomodationType.Cabin));
    }

   //Persistence Tests

    [Test]
    public void SaveAccomodation_WritesCorrectly()
    {
        // Arrange
        var accomodation = new Accomodation("A160", AccomodationType.Room, 7);

        // Act
        Accomodation.Save();

        // Assert
        Assert.That(File.Exists(_xmlFile), Is.True, "XML file should exist after Save().");
    }

    [Test]
    public void LoadAccomodation_ReadsCorrectly()
    {
        // Arrange 
        var original = new Accomodation("A160", AccomodationType.Room, 7);
        
        Accomodation.Save();
        
        ClearAllExtents(); 
        
        Assert.That(Accomodation.Accomodations.Count, Is.EqualTo(0), "Memory should be empty before Load.");

        // Act 
        Accomodation.Load();

        // Assert 
        Assert.That(Accomodation.Accomodations.Count, Is.EqualTo(1), "Should load 1 accomodation from file.");

        var loaded = Accomodation.Accomodations.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(loaded.Number, Is.EqualTo("A160"));
            Assert.That(loaded.Type, Is.EqualTo(AccomodationType.Room));
            Assert.That(loaded.Capacity, Is.EqualTo(7));
        });
    }
}