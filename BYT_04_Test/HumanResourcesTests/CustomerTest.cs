using BYT_04;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace BYT_04_Test;

public class CustomerTests
{
    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    private Address MakeAddress() =>
        new Address("Street", "City", "State", "12345", "Country");

    private Customer MakeCustomer(
        bool isVip = true,
        int points = 20
    )
    {
        return new Customer(
            "John",
            "B",
            "Walker",
            new DateTime(1985, 4, 20),
            "Male",
            "+48111222333",
            "john.walker@example.com",
            MakeAddress(),
            isVip,
            points
        );
    }

    /// <summary>
    /// Clears the Customer extent by overwriting XML with an empty list,
    /// then loading it to reset the static list.
    /// </summary>
    private void ResetCustomerExtent(string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        Customer.SetDirectory(dir);

        var xmlFile = Path.Combine(dir, "customers.xml");

        var emptyList = new List<Customer>();
        var serializer = new XmlSerializer(typeof(List<Customer>));

        using (var fs = new FileStream(xmlFile, FileMode.Create))
        {
            serializer.Serialize(fs, emptyList);
        }

        Customer.Load();
    }


    // ============================================================
    // Basic property tests
    // ============================================================

    [Test]
    public void TestCustomerCheckBalance()
    {
        var customer = MakeCustomer(points: 20);

        Assert.That(customer.LoyaltyPoints, Is.EqualTo(20));
    }

    [Test]
    public void TestCustomerIsVip()
    {
        var customer = MakeCustomer(isVip: true);

        Assert.That(customer.IsVip, Is.True);
    }

    [Test]
    public void TestCustomerMakeVip_ShouldSetIsVipToTrue()
    {
        // Arrange
        var customer = MakeCustomer(isVip: false);

        // Act
        customer.MakeVip();

        // Assert
        Assert.That(customer.IsVip, Is.True);
    }

    [Test]
    public void TestCustomerMakeVip_WhenAlreadyVip_ShouldRemainVip()
    {
        // Arrange
        var customer = MakeCustomer(isVip: true);

        // Act
        customer.MakeVip();

        // Assert
        Assert.That(customer.IsVip, Is.True);
    }
    

    // ============================================================
    // Save Test
    // ============================================================

    [Test]
    public void SaveCustomer_WritesCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "customer_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "customers.xml");

        ResetCustomerExtent(tempDir); // clean state

        var customer = MakeCustomer(points: 150);

        // Act
        Customer.Save();

        // Assert
        Assert.That(File.Exists(xmlFile), Is.True);
    }


    // ============================================================
    // Load Test
    // ============================================================

    [Test]
    public void LoadCustomer_ReadsCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "customer_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "customers.xml");

        ResetCustomerExtent(tempDir);

        // Create and save one customer
        var customer = MakeCustomer(points: 150);
        Customer.Save();

        

        // Act
        Customer.Load();

        // Assert
        Assert.That(Customer.Customers.Count, Is.EqualTo(1));

        var loaded = Customer.Customers.First();

        Assert.Multiple(() =>
        {
            Assert.That(loaded.Name, Is.EqualTo("John"));
            Assert.That(loaded.MiddleName, Is.EqualTo("B"));
            Assert.That(loaded.Surname, Is.EqualTo("Walker"));
            Assert.That(loaded.Email, Is.EqualTo("john.walker@example.com"));
            Assert.That(loaded.Address.City, Is.EqualTo("City"));

            Assert.That(loaded.IsVip, Is.True);
            Assert.That(loaded.LoyaltyPoints, Is.EqualTo(150));
        });
        
        ResetCustomerExtent(tempDir);
    }
}
