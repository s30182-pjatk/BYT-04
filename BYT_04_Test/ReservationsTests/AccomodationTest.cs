using BYT_04.Reservations;

namespace BYT_04_Test.ReservationsTests;

public class AccomodationTest
{
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
        var accomodation = new Accomodation { Type = AccomodationType.Cabin};
        
        Assert.That(accomodation.Type, Is.EqualTo(AccomodationType.Cabin));
    }
    
    [Test]
    public void SaveAndLoadAccomodation_WritesAndReadsCorrectly()
    {
        // --- Arrange ---
        //clear extent before testing
        var tempDir = Path.Combine(Path.GetTempPath(), "persistence");
        AccomodationExtent.SetDirectory(tempDir);
        AccomodationExtent.Accomodations.Clear();
        
        var accomodation = new Accomodation(
            "A160",
            AccomodationType.Room,
            7);

        AccomodationExtent.Accomodations.Add(accomodation);

        // --- Act ---
        AccomodationExtent.Save();             // Writes to XML
        AccomodationExtent.Accomodations.Clear(); // Clear memory
        AccomodationExtent.Load();             // Reads back from XML
        
        AccomodationExtent.DisplayAll(); 

        // --- Assert ---
        Assert.That(AccomodationExtent.Accomodations.Count, Is.EqualTo(1), "Extent should contain exactly 1 loaded item.");

        var loaded = AccomodationExtent.Accomodations.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.Number, Is.EqualTo("A160"));
            Assert.That(loaded.Type, Is.EqualTo(AccomodationType.Room));
            Assert.That(loaded.Capacity, Is.EqualTo(7));
        });
        
    }
}