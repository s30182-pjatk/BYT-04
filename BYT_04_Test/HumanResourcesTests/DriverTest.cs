using BYT_04;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using BYT_04.Vehicles;

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

    /// <summary>
    /// Completely resets static Driver.Drivers by overwriting the XML
    /// with an empty list, then loading it to replace _drivers.
    /// </summary>
    private void ResetDriverExtent(string directory)
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        Driver.SetDirectory(directory);

        var xmlFile = Path.Combine(directory, "drivers.xml");

        var emptyList = new List<Driver>();
        var serializer = new XmlSerializer(typeof(List<Driver>));

        using (var fs = new FileStream(xmlFile, FileMode.Create))
        {
            serializer.Serialize(fs, emptyList);
        }

        Driver.Load(); // loads empty → clears static extent
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
        var tempDir = Path.Combine(Path.GetTempPath(), "driver_persistence_tests");
        ResetDriverExtent(tempDir); // CLEAN

        var driver = MakeDriver();  // auto-adds to static list

        Driver.Save();

        var xmlFile = Path.Combine(tempDir, "drivers.xml");

        Assert.That(File.Exists(xmlFile), Is.True);
    }


    // ============================================================
    // Load Test – Reads XML
    // ============================================================

    [Test]
    public void LoadDriver_ReadsCorrectly()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "driver_persistence_tests");

        ResetDriverExtent(tempDir); // clean start

        var driver = MakeDriver();  // 1 driver
        Driver.Save();

        Driver.Load();

        Assert.That(Driver.Drivers.Count, Is.EqualTo(1));

        var loaded = Driver.Drivers.First();

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
        
        ResetDriverExtent(tempDir); // clear
    }
    
    // ============================================================
    // Association Tests
    // ============================================================
    
    [Test]
    public void TestDriverAssignVehicleToDriverCreatesReverseConnection()
    {
        // Arrange
        var driver = MakeDriver();
        var vehicle = new SUV("V1", "Jeep", 4, true, new Fuel(100f), true);

        // Act
        driver.AddAssignedVehicle(vehicle);

        // Assert
        // Assign from Driver side
        // Check Driver has Vehicle
        Assert.That(driver.AssignedVehicles.Contains(vehicle), Is.True, "Driver should have the vehicle.");
        
        // Check Vehicle has Driver (Reverse Connection)
        Assert.That(vehicle.AssignedDriver, Is.EqualTo(driver), "Vehicle should know its assigned driver.");
    }

    [Test]
    public void TestDriverAssignDriverToVehicleCreatesReverseConnection()
    {
        // Arrange
        var driver = MakeDriver();
        var vehicle = new SUV("V2", "Ford", 4, true, new Fuel(100f), true);

        // Act
        // Assign from Vehicle side
        vehicle.AssignedDriver = driver;

        // Assert
        // Check if Vehicle has Driver
        Assert.That(vehicle.AssignedDriver, Is.EqualTo(driver));
        
        // Check if Driver has Vehicle (Reverse Connection)
        Assert.That(driver.AssignedVehicles.Contains(vehicle), Is.True, "Driver should automatically have the vehicle added.");
    }

    [Test]
    public void ChangeDriver_UpdatesBothDrivers()
    {
        // Arrange
        var driver1 = MakeDriver(licenseNumber: "D1");
        var driver2 = MakeDriver(licenseNumber: "D2");
        var vehicle = new SUV("V3", "Toyota", 4, true, new Fuel(100f), true);

        // Act
        vehicle.AssignedDriver = driver1; // Assign to D1
        vehicle.AssignedDriver = driver2; // Reassign to D2

        // Assert
        // Driver with licenseNumber D1 should NO LONGER have the vehicle
        Assert.That(driver1.AssignedVehicles.Contains(vehicle), Is.False, "Vehicle should be removed from old driver.");
        
        // Driver with licenseNumber D2 SHOULD have the vehicle
        Assert.That(driver2.AssignedVehicles.Contains(vehicle), Is.True, "Vehicle should be added to new driver.");
        
        // Vehicle should point to D2
        Assert.That(vehicle.AssignedDriver, Is.EqualTo(driver2));
    }

    [Test]
    public void TestDriverRemoveAssignedVehicleRemovesReverseConnection()
    {
        // Arrange
        var driver = MakeDriver();
        var vehicle = new SUV("V4", "Honda", 4, true, new Fuel(100f), true);
        driver.AddAssignedVehicle(vehicle);

        // Pre-check
        Assert.That(vehicle.AssignedDriver, Is.EqualTo(driver));

        // Act
        driver.RemoveAssignedVehicle(vehicle);

        // Assert
        Assert.That(driver.AssignedVehicles.Contains(vehicle), Is.False);
        Assert.That(vehicle.AssignedDriver, Is.Null, "Vehicle should be unassigned when removed from driver.");
    }
}
