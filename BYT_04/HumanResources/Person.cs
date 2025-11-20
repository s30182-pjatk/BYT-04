namespace BYT_04;

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[Serializable]
public class Person
{
    public string Name { get; set; }
    public string? MiddleName { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; }

    public Person(
        string name,
        string? middleName,
        string surname,
        DateTime birthDate,
        string gender,
        string phoneNumber,
        string email,
        Address address)
    {
        if (birthDate > DateTime.Today)
        {
            throw new ArgumentException("Birth date cannot be in the future.");
        }

        Name = name;
        MiddleName = middleName;
        Surname = surname;
        BirthDate = birthDate;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        
    }

    public Person()
    {
    }

    public int GetAge()
    {
        var today = DateTime.Today;
        int age = today.Year - BirthDate.Year;
        if (BirthDate.Date > today.AddYears(-age))
        {
            age--;
        }
        return age;
    }
}

[Serializable]
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    public Address()
    {
    }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
}

public static class PersonExtent
{
    private static readonly string DirectoryPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "HumanResources", "persistence"));

    private static readonly string FilePath = Path.Combine(DirectoryPath, "persons.xml");

    public static List<Person> Persons { get; private set; } = new();

    public static void Save()
    {
        Console.WriteLine("Saving to: " + FilePath);

        if (!Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);

        XmlSerializer serializer = new(typeof(List<Person>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, Persons);
    }

    public static void Load()
    {
        Console.WriteLine("Loading from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Person>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Person> loaded)
            Persons = loaded;
    }
    
    public static void DisplayAll()
    {
        if (Persons.Count == 0)
        {
            Console.WriteLine("No persons found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Persons ---\n");

        foreach (var p in Persons)
        {
            Console.WriteLine(
                $"Name: {p.Name} {p.MiddleName} {p.Surname}\n" +
                $"Birth Date: {p.BirthDate.ToShortDateString()}\n" +
                $"Gender: {p.Gender}\n" +
                $"Phone: {p.PhoneNumber}\n" +
                $"Email: {p.Email}\n" +
                $"Address: {p.Address.Street}, {p.Address.City}, {p.Address.State}, " +
                $"{p.Address.PostalCode}, {p.Address.Country}\n" +
                "-----------------------------\n"
            );
        }
    }
}
