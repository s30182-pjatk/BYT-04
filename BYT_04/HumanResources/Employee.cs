namespace BYT_04;

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[Serializable]
public class Employee
{
    private DateTime _employmentDate;
    private decimal _salary;

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

    // Nullable manager
    [XmlIgnore]
    public Employee? Manager { get; set; }

    [XmlIgnore]
    public List<Employee> Subordinates { get; set; } = new();


    public Employee() { }

    public Employee(DateTime employmentDate, decimal salary, Employee? manager = null)
    {
        EmploymentDate = employmentDate;
        Salary = salary;
        Manager = manager;
        Subordinates = new List<Employee>(); // guarantee empty
    }

    public void AddSubordinate(Employee e)
    {
        Subordinates.Add(e);
        e.Manager = this;
    }
}


// ================== EXTENT ======================

public static class EmployeeExtent
{
    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "HumanResources", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "employees.xml");

    public static List<Employee> Employees { get; private set; } = new();

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

        XmlSerializer serializer = new(typeof(List<Employee>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, Employees);
    }

    public static void Load()
    {
        Console.WriteLine("Loading from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Employee>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Employee> loaded)
            Employees = loaded;
    }

    public static void DisplayAll()
    {
        if (Employees.Count == 0)
        {
            Console.WriteLine("No employees found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Employees ---\n");

        foreach (var e in Employees)
        {
            Console.WriteLine(
                $"Employment Date: {e.EmploymentDate.ToShortDateString()}\n" +
                $"Salary: {e.Salary}\n" +
                $"Manager: {(e.Manager == null ? "None" : "Exists")}\n" +
                $"Subordinates: {e.Subordinates.Count}\n" +
                "-----------------------------\n"
            );
        }
    }
}
