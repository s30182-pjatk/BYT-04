namespace BYT_04;
using BYT_04.Utility;

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[Serializable]
public class Person
{
    // ------------------------------
    //  STATIC PERSISTENCE MEMBERS
    // ------------------------------

    private static List<Person> _persons = new();                 // private collection
    private static string _directoryPath =                        // default directory
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "HumanResources", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "persons.xml");

    public static IReadOnlyList<Person> Persons => _persons;     // public read-only access

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

        serializer.Serialize(fs, _persons);
    }

    public static void Load()
    {
        Console.WriteLine("Loading from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Person>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Person> loaded)
            _persons = loaded;
    }

    public static void DisplayAll()
    {
        if (_persons.Count == 0)
        {
            Console.WriteLine("No persons found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Persons ---\n");

        foreach (var p in _persons)
        {
            Console.WriteLine(
                $"Name: {p.Name} {p.MiddleName} {p.Surname}\n" +
                $"Birth Date: {p.BirthDate.ToShortDateString()}\n" +
                $"Gender: {p.Gender}\n" +
                $"Phone: {p.PhoneNumber}\n" +
                $"Email: {p.Email}\n" +
                $"Address: {p.Address.Street}, {p.Address.City}, {p.Address.State}, {p.Address.PostalCode}, {p.Address.Country}\n" +
                "-----------------------------\n"
            );
        }
    }

    // Optional helpers:
    public static void Add(Person p) => _persons.Add(p);
    public static void Remove(Person p) => _persons.Remove(p);

    // --------------------------------
    //  INSTANCE PROPERTIES & LOGIC
    // --------------------------------

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
        set => _name = value.ValidateRequiredString(nameof(Name));
    }

    public string? MiddleName
    {
        get => _middleName;
        set => _middleName = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public string Surname
    {
        get => _surname;
        set => _surname = value.ValidateRequiredString(nameof(Surname));
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
        set => _gender = value.ValidateRequiredString(nameof(Gender));
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = value.ValidateRequiredString(nameof(PhoneNumber));
    }

    public string Email
    {
        get => _email;
        set => _email = value.ValidateRequiredString(nameof(Email));
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
        
        _persons.Add(this);
    }

    public int GetAge()
    {
        var today = DateTime.Today;
        int age = today.Year - BirthDate.Year;
        if (BirthDate.Date > today.AddYears(-age))
            age--;
        return age;
    }
}
