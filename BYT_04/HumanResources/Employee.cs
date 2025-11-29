namespace BYT_04;

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using BYT_04.Utility;

[Serializable]
public class Employee : Person
{
    // ============================================================
    //                STATIC EXTENT & PERSISTENCE
    // ============================================================

    private static List<Employee> _employees = new();

    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..",
            "HumanResources", "persistence"
        ));

    private static string FilePath => Path.Combine(_directoryPath, "employees.xml");

    public static IReadOnlyList<Employee> Employees => _employees;


    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory path cannot be null or empty.");

        _directoryPath = newDirectory;
    }


    public static void Save()
    {
        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<Employee>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, _employees);
    }


    public static void Load()
    {
        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Employee>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Employee> loaded)
            _employees = loaded;
    }


    public static void DisplayAll()
    {
        if (_employees.Count == 0)
        {
            Console.WriteLine("No employees found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Employees ---\n");

        foreach (var e in _employees)
        {
            Console.WriteLine(
                $"Name: {e.Name} {e.MiddleName} {e.Surname}\n" +
                $"Birth Date: {e.BirthDate.ToShortDateString()}\n" +
                $"Gender: {e.Gender}\n" +
                $"Phone: {e.PhoneNumber}\n" +
                $"Email: {e.Email}\n" +
                $"Address: {e.Address.Street}, {e.Address.City}, {e.Address.State}, " +
                $"{e.Address.PostalCode}, {e.Address.Country}\n" +
                $"Employment Date: {e.EmploymentDate.ToShortDateString()}\n" +
                $"Salary: {e.Salary}\n" +
                "-----------------------------\n"
            );
        }
    }

    public static void Add(Employee e) => _employees.Add(e);
    public static void Remove(Employee e) => _employees.Remove(e);



    // ============================================================
    //                INSTANCE DATA
    // ============================================================

    private DateTime _employmentDate;
    private decimal _salary;

    // -------------------- EMPLOYEE FIELDS ----------------------

    public DateTime EmploymentDate
    {
        get => _employmentDate;
        set
        {
            if (value > DateTime.Today)
                throw new ArgumentException("Employment date cannot be in the future.");
            _employmentDate = value;
        }
    }

    public decimal Salary
    {
        get => _salary;
        set
        {
            if (value < 0)
                throw new ArgumentException("Salary cannot be negative.");
            _salary = value;
        }
    }


    // Not serialized (avoid circular structure)
    [XmlIgnore]
    public Employee? Manager { get; set; }

    [XmlIgnore]
    public List<Employee> Subordinates { get; set; } = new();


    // ============================================================
    //                 CONSTRUCTORS
    // ============================================================

    // IMPORTANT: parameterless constructor must NOT add to collection
    public Employee() { }

    public Employee(
        string name,
        string? middleName,
        string surname,
        DateTime birthDate,
        string gender,
        string phoneNumber,
        string email,
        Address address,
        DateTime employmentDate,
        decimal salary,
        Employee? manager = null
    ) : base(name, middleName, surname, birthDate, gender, phoneNumber, email, address)
    {
        EmploymentDate = employmentDate;
        Salary = salary;
        Manager = manager;

        Subordinates = new List<Employee>();

        // auto-registration
        _employees.Add(this);
    }


    // ============================================================
    //                 HELPERS
    // ============================================================

    public void AddSubordinate(Employee e)
    {
        if (e == null)
            throw new ArgumentNullException(nameof(e));

        Subordinates.Add(e);
        e.Manager = this;
    }
}
