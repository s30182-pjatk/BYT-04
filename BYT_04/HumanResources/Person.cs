namespace BYT_04;

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[Serializable]
public class Person
{
    
    private string _name = null!;
    private string? _middleName;
    private string _surname = null!;
    private DateTime _birthDate;
    private string _gender = null!;
    private string _phoneNumber = null!;
    private string _email = null!;
    private Address _address = null!;

    public string Name
    {
        get => _name;
        set => _name = ValidateRequiredString(value, nameof(Name));
    }

    public string? MiddleName
    {
        get => _middleName;
        set => _middleName = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public string Surname
    {
        get => _surname;
        set => _surname = ValidateRequiredString(value, nameof(Surname));
    }

    public DateTime BirthDate
    {
        get => _birthDate;
        set
        {
            if (value > DateTime.Today)
                throw new ArgumentException("Birth date cannot be in the future.");
            _birthDate = value;
        }
    }

    public string Gender
    {
        get => _gender;
        set => _gender = ValidateRequiredString(value, nameof(Gender));
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = ValidateRequiredString(value, nameof(PhoneNumber));
    }

    public string Email
    {
        get => _email;
        set => _email = ValidateRequiredString(value, nameof(Email));
    }

    public Address Address
    {
        get => _address;
        set => _address = value ?? throw new ArgumentException("Address cannot be null.");
    }

    public Person() { }

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
        Name = name;
        MiddleName = middleName;
        Surname = surname;
        BirthDate = birthDate;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
    }


    public int GetAge()
    {
        var today = DateTime.Today;
        int age = today.Year - BirthDate.Year;
        if (BirthDate.Date > today.AddYears(-age))
            age--;
        return age;
    }

    private static string ValidateRequiredString(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{propertyName} cannot be null, empty, or whitespace.");

        return value;
    }
}

[Serializable]
public class Address
{
    private string _street = null!;
    private string _city = null!;
    private string _state = null!;
    private string _postalCode = null!;
    private string _country = null!;

    public string Street
    {
        get => _street;
        set => _street = ValidateRequiredString(value, nameof(Street));
    }

    public string City
    {
        get => _city;
        set => _city = ValidateRequiredString(value, nameof(City));
    }

    public string State
    {
        get => _state;
        set => _state = ValidateRequiredString(value, nameof(State));
    }

    public string PostalCode
    {
        get => _postalCode;
        set => _postalCode = ValidateRequiredString(value, nameof(PostalCode));
    }

    public string Country
    {
        get => _country;
        set => _country = ValidateRequiredString(value, nameof(Country));
    }

    public Address() { }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    private static string ValidateRequiredString(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{propertyName} cannot be null, empty, or whitespace.");
        return value;
    }
}

public static class PersonExtent
{
    // Now adjustable (not readonly)
    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "HumanResources", "persistence"));

    // FilePath must always reflect current directory
    private static string FilePath => Path.Combine(_directoryPath, "persons.xml");

    public static List<Person> Persons { get; private set; } = new();

    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory path cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

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

