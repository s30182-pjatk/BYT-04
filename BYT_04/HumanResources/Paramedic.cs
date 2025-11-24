namespace BYT_04;

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using BYT_04.Utility;

[Serializable]
public class Paramedic : Person
{
    private string _cprCertificationNumber = null!;

    public string CPRCertificationNumber
    {
        get => _cprCertificationNumber;
        set => value.ValidateRequiredString("CPR Certificate Number");
    }

    public Paramedic() : base() { }

    public Paramedic(
        string name,
        string? middleName,
        string surname,
        DateTime birthDate,
        string gender,
        string phoneNumber,
        string email,
        Address address,
        string cprCertificationNumber
    ) : base(name, middleName, surname, birthDate, gender, phoneNumber, email, address)
    {
        CPRCertificationNumber = cprCertificationNumber;
    }

    public bool IsCertified() => !string.IsNullOrWhiteSpace(CPRCertificationNumber);
}

public static class ParamedicExtent
{
    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..",
            "HumanResources", "persistence"
        ));

    private static string FilePath => Path.Combine(_directoryPath, "paramedics.xml");

    public static List<Paramedic> Paramedics { get; private set; } = new();


    // ============================================================
    // Set custom directory
    // ============================================================
    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory path cannot be null or empty.");

        _directoryPath = newDirectory;
    }


    // ============================================================
    // Save extent to XML
    // ============================================================
    public static void Save()
    {
        Console.WriteLine("Saving paramedics to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<Paramedic>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, Paramedics);
    }


    // ============================================================
    // Load extent from XML
    // ============================================================
    public static void Load()
    {
        Console.WriteLine("Loading paramedics from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Paramedic>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Paramedic> loaded)
            Paramedics = loaded;
    }


    // ============================================================
    // Display all paramedics
    // ============================================================
    public static void DisplayAll()
    {
        if (Paramedics.Count == 0)
        {
            Console.WriteLine("No paramedics found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Paramedics ---\n");

        foreach (var p in Paramedics)
        {
            Console.WriteLine(
                $"Name: {p.Name} {p.MiddleName} {p.Surname}\n" +
                $"Birth Date: {p.BirthDate.ToShortDateString()}\n" +
                $"Gender: {p.Gender}\n" +
                $"Phone: {p.PhoneNumber}\n" +
                $"Email: {p.Email}\n" +
                $"Address: {p.Address.Street}, {p.Address.City}, {p.Address.State}, {p.Address.PostalCode}, {p.Address.Country}\n" +
                $"CPR Certification Number: {p.CPRCertificationNumber}\n" +
                "-----------------------------\n"
            );
        }
    }
}