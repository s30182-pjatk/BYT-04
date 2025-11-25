using BYT_04;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace BYT_04_Test
{
    public class EmployeeTests
    {
        private Address MakeAddress() =>
            new Address("Street", "City", "State", "00000", "Country");

        private Employee MakeEmployee(DateTime employmentDate, decimal salary) =>
            new Employee(
                "John",
                null,
                "Doe",
                new DateTime(1990, 1, 1),
                "Male",
                "12345678",
                "john@doe.com",
                MakeAddress(),
                employmentDate,
                salary
            );

        // ============================================================
        // Validation Tests
        // ============================================================

        [Test]
        public void TestEmployeeInvalidEmploymentDate()
        {
            var futureDate = DateTime.Today.AddDays(1);

            Assert.Throws<ArgumentException>(() =>
                MakeEmployee(futureDate, 5000));
        }

        [Test]
        public void TestEmployeeInvalidSalary()
        {
            var date = new DateTime(2020, 1, 1);

            Assert.Throws<ArgumentException>(() =>
                MakeEmployee(date, -100));
        }

        [Test]
        public void TestAddSubordinate_SetsManagerCorrectly()
        {
            var boss = MakeEmployee(new DateTime(2010, 1, 1), 9000);
            var worker = MakeEmployee(new DateTime(2020, 1, 1), 3000);

            boss.AddSubordinate(worker);

            Assert.Multiple(() =>
            {
                Assert.That(boss.Subordinates.Contains(worker), Is.True);
                Assert.That(worker.Manager, Is.EqualTo(boss));
            });
        }

        // ============================================================
        // Save Test – writes XML
        // ============================================================

        [Test]
        public void SaveEmployee_WritesCorrectly()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "employee_persistence_tests");
            var xmlFile = Path.Combine(tempDir, "employees.xml");

            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);

            Employee.SetDirectory(tempDir);
            Employee.Load();            // loads nothing
            Employee.Employees.ToList().Clear(); // clear extent? impossible directly

            // Clear private static list via Save() overwrite:
            // We'll create 1 employee only
            var employee = MakeEmployee(new DateTime(2015, 3, 15), 4500m);

            Employee.Save();

            Assert.That(File.Exists(xmlFile), Is.True,
                "XML file should exist after Save().");
        }

        // ============================================================
        // Load Test – reads XML
        // ============================================================

        [Test]
        public void LoadEmployee_ReadsCorrectly()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "employee_persistence_tests");
            var xmlFile = Path.Combine(tempDir, "employees.xml");

            Employee.SetDirectory(tempDir);

            if (!File.Exists(xmlFile))
                SaveEmployee_WritesCorrectly();

            // Clear employees by overwriting XML with empty list
            File.WriteAllText(xmlFile, ""); // empty -> Load() will skip

            // Recreate file with one employee
            SaveEmployee_WritesCorrectly();

            Employee.Load();

            Assert.That(Employee.Employees.Count, Is.EqualTo(1));

            var loaded = Employee.Employees.First();

            Assert.Multiple(() =>
            {
                Assert.That(loaded.EmploymentDate, Is.EqualTo(new DateTime(2015, 3, 15)));
                Assert.That(loaded.Salary, Is.EqualTo(4500m));

                // Manager/Subordinates are ignored in XML
                Assert.That(loaded.Manager, Is.Null);
                Assert.That(loaded.Subordinates.Count, Is.EqualTo(0));
            });
        }
    }
}
