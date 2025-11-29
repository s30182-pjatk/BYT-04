using BYT_04;
using BYT_04_Test.TestUtils;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace BYT_04_Test
{
    public class PersonTests
    {
        [SetUp]
        public void Reset()
        {
            // Clear static collection before each test
            // Also clear Customer collection since we're using Customer instances
            ClearList.ClearStaticList<Person>("_persons");
            ClearList.ClearStaticList<Customer>("_customers");
        }

        [Test]
        public void TestPersonInvalidDate()
        {
            var invalidDate = DateTime.Today.AddDays(1);
            var address = new Address("some", "another", "dom", "somecode", "someplace");

            Assert.Throws<ArgumentException>(() =>
                new Customer("Gleb", null, "Denisov", invalidDate, "male",
                    "+48999999999", "email@gmail.com", address, false, 0));
        }

        [Test]
        public void TestPersonCorrectAge()
        {
            DateTime date1 = new DateTime(2015, 8, 28);

            var address = new Address("some", "another", "dom", "somecode", "someplace");
            var person = new Customer("Gleb", null, "Denisov", date1,
                "male", "+48999999999", "email@gmail.com", address, false, 0);

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

            Person.SetDirectory(tempDir);

            var person = new Customer(
                "John", "A", "Doe",
                new DateTime(1990, 5, 12),
                "Male",
                "123456789",
                "john.doe@example.com",
                new Address("123 Street", "City", "State", "11111", "Country"),
                false,
                0
            );

            Person.Add(person);

            // Act
            Person.Save();

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

            // Clean up any existing test files
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);

            Person.SetDirectory(tempDir);

            // Clear collections before creating test data
            ClearList.ClearStaticList<Person>("_persons");
            ClearList.ClearStaticList<Customer>("_customers");

            // Create and save test data
            var person = new Customer(
                "John", "A", "Doe",
                new DateTime(1990, 5, 12),
                "Male",
                "123456789",
                "john.doe@example.com",
                new Address("123 Street", "City", "State", "11111", "Country"),
                false,
                0
            );

            Person.Save();

            ClearList.ClearStaticList<Person>("_persons");
            ClearList.ClearStaticList<Customer>("_customers");

            // Act
            Person.Load();

            // Assert
            Assert.That(Person.Persons.Count, Is.EqualTo(1));

            var loaded = Person.Persons.First();

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
}
