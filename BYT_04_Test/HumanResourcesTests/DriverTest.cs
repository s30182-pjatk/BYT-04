using BYT_04;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace BYT_04_Test;

public class DriverTests
{
    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    private Address MakeAddress() =>
        new Address("Street", "City", "State", "12345", "Country");

    private Driver MakeDriver(
        string licenseNumber = "ABC123",
        DateTime? expiry = null
    )
    {
        return new Driver(
            "John",
            "M",
            "Doe",
            new DateTime(1990, 1, 1),
            "Male",
            "999999999",
            "john@example.com",
            MakeAddress(),
            licenseNumber,
            expiry ?? DateTime.Today.AddYears(1)
        );
    }


    // ============================================================
    // Validation Tests
    // ============================================================

    [Test]
    public void TestDriverInvalidLicenseNumber()
    {
        Assert.Throws<ArgumentException>(() =>
            MakeDriver(licenseNumber: ""));
    }

    [Test]
    public void TestDriverExpiredLicenseNotAllowed()
    {
        var pastDate = DateTime.Today.AddDays(-1);

        Assert.Throws<ArgumentException>(() =>
            MakeDriver(expiry: pastDate));
    }

    [Test]
    public void TestDriverIsLicenseValid()
    {
        var driver = MakeDriver(expiry: DateTime.Today.AddDays(10));

        Assert.That(driver.IsLicenseValid(), Is.True);
    }


    // ============================================================
    // Save Test – Writes XML
    // ============================================================

    [Test]
    public void SaveDriver_WritesCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "driver_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "drivers.xml");

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

        DriverExtent.SetDirectory(tempDir);
        DriverExtent.Drivers.Clear();

        var driver = MakeDriver();

        DriverExtent.Drivers.Add(driver);

        // Act
        DriverExtent.Save();

        // Assert
        Assert.That(File.Exists(xmlFile), Is.True,
            "XML file should exist after Save().");
    }


    // ============================================================
    // Load Test – Reads XML
    // ============================================================

    [Test]
    public void LoadDriver_ReadsCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "driver_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "drivers.xml");

        DriverExtent.SetDirectory(tempDir);

        // Ensure file exists if test runs alone
        if (!File.Exists(xmlFile))
            SaveDriver_WritesCorrectly();

        DriverExtent.Drivers.Clear();

        // Act
        DriverExtent.Load();

        // Assert
        Assert.That(DriverExtent.Drivers.Count, Is.EqualTo(1));

        var loaded = DriverExtent.Drivers.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.Name, Is.EqualTo("John"));
            Assert.That(loaded.MiddleName, Is.EqualTo("M"));
            Assert.That(loaded.Surname, Is.EqualTo("Doe"));
            Assert.That(loaded.Email, Is.EqualTo("john@example.com"));
            Assert.That(loaded.Address.City, Is.EqualTo("City"));

            Assert.That(loaded.LicenseNumber, Is.EqualTo("ABC123"));
            Assert.That(loaded.LicenseExpiry >= DateTime.Today, Is.True);
        });
    }
}
