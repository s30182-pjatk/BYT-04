using BYT_04;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace BYT_04_Test;

public class EmployeeTests
{
    // ============================================================
    // Validation Tests
    // ============================================================
    
    [Test]
    public void TestEmployeeInvalidEmploymentDate()
    {
        var futureDate = DateTime.Today.AddDays(1);

        Assert.Throws<ArgumentException>(() =>
            new Employee(futureDate, 5000));
    }

    [Test]
    public void TestEmployeeInvalidSalary()
    {
        var date = new DateTime(2020, 1, 1);

        Assert.Throws<ArgumentException>(() =>
            new Employee(date, -100));
    }

    [Test]
    public void TestAddSubordinate_SetsManagerCorrectly()
    {
        var boss = new Employee(new DateTime(2010, 1, 1), 9000);
        var worker = new Employee(new DateTime(2020, 1, 1), 3000);

        boss.AddSubordinate(worker);

        Assert.Multiple(() =>
        {
            Assert.That(boss.Subordinates.Contains(worker), Is.True);
            Assert.That(worker.Manager, Is.EqualTo(boss));
        });
    }

    // ============================================================
    // 1) Save test – writes XML only
    // ============================================================

    [Test]
    public void SaveEmployee_WritesCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "employee_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "employees.xml");

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

        EmployeeExtent.SetDirectory(tempDir);
        EmployeeExtent.Employees.Clear();

        var employee = new Employee(
            new DateTime(2015, 3, 15),
            4500m
        );

        EmployeeExtent.Employees.Add(employee);

        // Act
        EmployeeExtent.Save();

        // Assert
        Assert.That(File.Exists(xmlFile), Is.True,
            "XML file should exist after Save().");
    }

    // ============================================================
    // 2) Load test – reads XML only
    // ============================================================

    [Test]
    public void LoadEmployee_ReadsCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "employee_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "employees.xml");

        EmployeeExtent.SetDirectory(tempDir);

        // Ensure XML exists even if test runs independently
        if (!File.Exists(xmlFile))
            SaveEmployee_WritesCorrectly();

        EmployeeExtent.Employees.Clear();

        // Act
        EmployeeExtent.Load();

        // Assert
        Assert.That(EmployeeExtent.Employees.Count, Is.EqualTo(1));

        var loaded = EmployeeExtent.Employees.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.EmploymentDate, Is.EqualTo(new DateTime(2015, 3, 15)));
            Assert.That(loaded.Salary, Is.EqualTo(4500m));
            
            // Manager/Subordinates are ignored in XML => always null / empty
            Assert.That(loaded.Manager, Is.Null);
            Assert.That(loaded.Subordinates.Count, Is.EqualTo(0));
        });
    }
}
