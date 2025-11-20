using BYT_04;

namespace BYT_04_Test;

public class PersonTests
{
    [Test]
    public void TestPersonInvalidDate()
    {
        var invalidDate = DateTime.Today.AddDays(1);

        var address = new Address("some", "another", "dom", "somecode", "someplace");
        Assert.Throws<ArgumentException>(() => new Person("Gleb", null, "Denisov", invalidDate, "male", "+48999999999", "email@gmail.com", address));
    }
    
    [Test]
    public void TestPersonCorrectAge()
    {
        DateTime date1 = new DateTime(2015, 8, 28);

        var address = new Address("some", "another", "dom", "somecode", "someplace");
        var person = new Person("Gleb", null, "Denisov", date1, "male", "+48999999999", "email@gmail.com", address);
        
        Assert.That(person.GetAge(), Is.EqualTo(10));
    }
    
    [Test]
    public void SaveAndLoadPerson_WritesAndReadsCorrectly()
    {
        // --- Arrange ---
        // Clear in-memory extent before test
        var tempDir = Path.Combine(Path.GetTempPath(), "persistence");
        PersonExtent.SetDirectory(tempDir);
        PersonExtent.Persons.Clear();

        var person = new Person(
            "John", "A", "Doe",
            new DateTime(1990, 5, 12),
            "Male",
            "123456789",
            "john.doe@example.com",
            new Address("123 Street", "City", "State", "11111", "Country")
        );

        PersonExtent.Persons.Add(person);

        // --- Act ---
        PersonExtent.Save();   // Writes XML
        PersonExtent.Persons.Clear(); // CLEAR memory to ensure we ONLY load from XML
        PersonExtent.Load();   // Reads XML
        
        PersonExtent.DisplayAll();
        
        // --- Assert ---
        Assert.That(PersonExtent.Persons.Count, Is.EqualTo(1));

        var loaded = PersonExtent.Persons.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.Name, Is.EqualTo("John"));
            Assert.That(loaded.MiddleName, Is.EqualTo("A"));
            Assert.That(loaded.Surname, Is.EqualTo("Doe"));
            Assert.That(loaded.Email, Is.EqualTo("john.doe@example.com"));
            Assert.That(loaded.Address.City, Is.EqualTo("City"));
        });
    }
}