using BYT_04;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace BYT_04_Test;

public class CustomerTests
{
    [Test]
    public void TestCustomerCheckBalance()
    {
        DateTime birth = new DateTime(2015, 8, 28);

        var address = new Address("some", "another", "dom", "somecode", "someplace");
        var customer = new Customer(
            "Gleb", null, "Denisov",
            birth,
            "male",
            "+48999999999",
            "email@gmail.com",
            address,
            isVip: true,
            loyaltyPoints: 20
        );

        Assert.That(customer.LoyaltyPoints, Is.EqualTo(20));
    }

    [Test]
    public void TestCustomerIsVip()
    {
        DateTime birth = new DateTime(2015, 8, 28);

        var address = new Address("some", "another", "dom", "somecode", "someplace");
        var customer = new Customer(
            "Gleb", null, "Denisov",
            birth,
            "male",
            "+48999999999",
            "email@gmail.com",
            address,
            isVip: true,
            loyaltyPoints: 20
        );

        Assert.That(customer.IsVip, Is.EqualTo(true));
    }

    //====================================================================
    // 1) Save test – writes XML only
    //====================================================================
    [Test]
    public void SaveCustomer_WritesCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "customer_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "customers.xml");

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

        CustomerExtent.SetDirectory(tempDir);
        CustomerExtent.Customers.Clear();

        var customer = new Customer(
            "John",
            "B",
            "Walker",
            new DateTime(1985, 4, 20),
            "Male",
            "+48111222333",
            "john.walker@example.com",
            new Address("Market St", "City", "State", "22-222", "Country"),
            isVip: true,
            loyaltyPoints: 150
        );

        CustomerExtent.Customers.Add(customer);

        // Act
        CustomerExtent.Save();

        // Assert
        Assert.That(File.Exists(xmlFile), Is.True,
            "XML file should exist after Save().");
    }

    //====================================================================
    // 2) Load test – reads XML only
    //====================================================================
    [Test]
    public void LoadCustomer_ReadsCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "customer_persistence_tests");
        var xmlFile = Path.Combine(tempDir, "customers.xml");

        CustomerExtent.SetDirectory(tempDir);

        // Ensure XML exists if test runs alone
        if (!File.Exists(xmlFile))
            SaveCustomer_WritesCorrectly();

        CustomerExtent.Customers.Clear();

        // Act
        CustomerExtent.Load();

        // Assert
        Assert.That(CustomerExtent.Customers.Count, Is.EqualTo(1));

        var loaded = CustomerExtent.Customers.First();

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
    }
}
