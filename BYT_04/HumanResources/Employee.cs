namespace BYT_04;

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[Serializable]
public class Employee
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

    private string _name = null!;
    private string? _middleName;
    private string _surname = null!;
    private DateTime _birthDate;
    private string _gender = null!;
    private string _phoneNumber = null!;
    private string _email = null!;
    private Address _address = null!;

    private DateTime _employmentDate;
    private decimal _salary;


    // --------------------  PERSON FIELDS  ----------------------

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
    )
    {
        Name = name;
        MiddleName = middleName;
        Surname = surname;
        BirthDate = birthDate;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;

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


    private static string ValidateRequiredString(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{field} cannot be empty.");
        return value;
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
