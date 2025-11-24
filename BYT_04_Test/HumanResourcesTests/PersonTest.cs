using BYT_04;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace BYT_04_Test;

public class PersonTests
{
    [Test]
    public void TestPersonInvalidDate()
    {
        var invalidDate = DateTime.Today.AddDays(1);
        var address = new Address("some", "another", "dom", "somecode", "someplace");

        Assert.Throws<ArgumentException>(() =>
            new Person("Gleb", null, "Denisov", invalidDate, "male",
                "+48999999999", "email@gmail.com", address));
    }
    
    [Test]
    public void TestPersonCorrectAge()
    {
        DateTime date1 = new DateTime(2015, 8, 28);

        var address = new Address("some", "another", "dom", "somecode", "someplace");
        var person = new Person("Gleb", null, "Denisov", date1,
            "male", "+48999999999", "email@gmail.com", address);
        
        Assert.That(person.GetAge(), Is.EqualTo(10));
    }

    //====================================================================
    // 1) Save test – writes XML only
    //====================================================================
    [Test]
    public void SavePerson_WritesCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "person_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "persons.xml");

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

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

        // Act
        PersonExtent.Save();

        // Assert
        Assert.That(File.Exists(xmlFile), Is.True,
            "XML file should exist after Save().");
    }

    //====================================================================
    // 2) Load test – reads XML only
    //====================================================================
    [Test]
    public void LoadPerson_ReadsCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "person_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "persons.xml");

        PersonExtent.SetDirectory(tempDir);

        // Ensure XML exists (if this test runs alone)
        if (!File.Exists(xmlFile))
            SavePerson_WritesCorrectly();

        PersonExtent.Persons.Clear();

        // Act
        PersonExtent.Load();

        // Assert
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
