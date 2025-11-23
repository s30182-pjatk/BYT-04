namespace BYT_04;

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[Serializable]
public class Customer : Person
{
    private bool _isVip;
    private int _loyaltyPoints;

    public bool IsVip
    {
        get => _isVip;
        set => _isVip = value;
    }

    public int LoyaltyPoints
    {
        get => _loyaltyPoints;
        set
        {
            if (value < 0)
                throw new ArgumentException("Loyalty points cannot be negative.");
            _loyaltyPoints = value;
        }
    }

    public Customer() : base()
    {
    }

    public Customer(
        string name,
        string? middleName,
        string surname,
        DateTime birthDate,
        string gender,
        string phoneNumber,
        string email,
        Address address,
        bool isVip,
        int loyaltyPoints
    ) 
        : base(name, middleName, surname, birthDate, gender, phoneNumber, email, address)
    {
        IsVip = isVip;
        LoyaltyPoints = loyaltyPoints;
    }

    public int CheckLoyaltyPoints() => LoyaltyPoints;
}

public static class CustomerExtent
{
    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "HumanResources", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "customers.xml");

    public static List<Customer> Customers { get; private set; } = new();

    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory path cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving customers to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<Customer>));
        using FileStream fs = new(FilePath, FileMode.Create);

        serializer.Serialize(fs, Customers);
    }

    public static void Load()
    {
        Console.WriteLine("Loading customers from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Customer>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Customer> loaded)
            Customers = loaded;
    }

    public static void DisplayAll()
    {
        if (Customers.Count == 0)
        {
            Console.WriteLine("No customers found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Customers ---\n");

        foreach (var c in Customers)
        {
            Console.WriteLine(
                $"Name: {c.Name} {c.MiddleName} {c.Surname}\n" +
                $"Birth Date: {c.BirthDate.ToShortDateString()}\n" +
                $"Gender: {c.Gender}\n" +
                $"Phone: {c.PhoneNumber}\n" +
                $"Email: {c.Email}\n" +
                $"VIP: {(c.IsVip ? "Yes" : "No")}\n" +
                $"Loyalty Points: {c.LoyaltyPoints}\n" +
                $"Address: {c.Address.Street}, {c.Address.City}, {c.Address.State}, " +
                $"{c.Address.PostalCode}, {c.Address.Country}\n" +
                "-----------------------------\n"
            );
        }
    }
}