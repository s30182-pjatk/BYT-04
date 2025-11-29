using BYT_04;
using BYT_04_Test.TestUtils;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace BYT_04_Test
{
    public class ParamedicTests
    {
        [SetUp]
        public void Reset()
        {
            // Clear static lists before each test (both Paramedic and Person collections)
            ClearList.ClearStaticList<Paramedic>("_paramedics");
            ClearList.ClearStaticList<Person>("_persons");
        }

        // ============================================================
        // Validation Tests
        // ============================================================

        [Test]
        public void TestInvalidCPRNumber_ThrowsException()
        {
            DateTime birth = new(1990, 1, 1);
            var address = new Address("A", "B", "C", "00000", "X");

            Assert.Throws<ArgumentException>(() =>
                new Paramedic(
                    "John", null, "Doe",
                    birth,
                    "Male",
                    "123",
                    "email@example.com",
                    address,
                    ""   // invalid CPR number
                )
            );
        }

        [Test]
        public void TestParamedicPropertiesAssignedCorrectly()
        {
            var birth = new DateTime(1985, 6, 15);
            var address = new Address("Main", "City", "ST", "11111", "Country");

            var p = new Paramedic(
                "Alice",
                "M",
                "Stone",
                birth,
                "Female",
                "555-333",
                "a@b.com",
                address,
                "CPR-999"
            );

            Assert.Multiple(() =>
            {
                Assert.That(p.CPRCertificationNumber, Is.EqualTo("CPR-999"));
                Assert.That(p.Name, Is.EqualTo("Alice"));
                Assert.That(p.Surname, Is.EqualTo("Stone"));
                Assert.That(p.Address.City, Is.EqualTo("City"));
            });
        }


        // ============================================================
        // Save test — writes XML only
        // ============================================================

        [Test]
        public void SaveParamedic_WritesCorrectly()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "paramedic_persistence_tests");
            var xmlFile = Path.Combine(tempDir, "paramedics.xml");

            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);

            Paramedic.SetDirectory(tempDir);

            var address = new Address("Street", "Town", "Region", "11111", "Country");

            var p = new Paramedic(
                "Bob",
                null,
                "Marley",
                new DateTime(1992, 2, 2),
                "Male",
                "12345",
                "bob@example.com",
                address,
                "CPR-777"
            );
          

            // Act
            Paramedic.Save();

            // Assert
            Assert.That(File.Exists(xmlFile), Is.True);
        }


        // ============================================================
        // Load test — reads XML only
        // ============================================================

        [Test]
        public void LoadParamedic_ReadsCorrectly()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "paramedic_persistence_tests");
            var xmlFile = Path.Combine(tempDir, "paramedics.xml");

            // Clean up any existing test files
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);

            Paramedic.SetDirectory(tempDir);

            // Clear collections before creating test data
            ClearList.ClearStaticList<Paramedic>("_paramedics");
            ClearList.ClearStaticList<Person>("_persons");

            // Create and save test data
            var address = new Address("Street", "Town", "Region", "11111", "Country");
            var p = new Paramedic(
                "Bob",
                null,
                "Marley",
                new DateTime(1992, 2, 2),
                "Male",
                "12345",
                "bob@example.com",
                address,
                "CPR-777"
            );
            Paramedic.Save();

            // Clear lists before loading (both Paramedic and Person collections)
            ClearList.ClearStaticList<Paramedic>("_paramedics");
            ClearList.ClearStaticList<Person>("_persons");

            // Act
            Paramedic.Load();

            // Assert
            Assert.That(Paramedic.Paramedics.Count, Is.EqualTo(1));

            var loaded = Paramedic.Paramedics.First();

            Assert.Multiple(() =>
            {
                Assert.That(loaded.Name, Is.EqualTo("Bob"));
                Assert.That(loaded.Surname, Is.EqualTo("Marley"));
                Assert.That(loaded.CPRCertificationNumber, Is.EqualTo("CPR-777"));

                Assert.That(loaded.Address.Street, Is.EqualTo("Street"));
                Assert.That(loaded.Address.City, Is.EqualTo("Town"));
                Assert.That(loaded.Address.Country, Is.EqualTo("Country"));
            });
        }
    }
}
