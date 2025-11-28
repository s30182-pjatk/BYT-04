using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace BYT_04.Vehicles;

[Serializable]
public class Helicoper : Vehicle
{
    // --------- Extent properties ----------
    private static readonly List<Helicoper> _helicopters = new();
    public static IReadOnlyList<Helicoper> Helicopters => _helicopters.AsReadOnly();

    private static string _directoryPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Vehicles", "persistence")
    );

    private static string FilePath => Path.Combine(_directoryPath, "helicopters.xml");

    public Helicoper() { }

    public Helicoper(string plateNumber, string model, int capacity, bool containMedKit, VehiclePowerType powerType)
        : base(plateNumber, model, capacity, containMedKit, powerType)
    {
        AddHelicopter(this);
    }

    // --------- Extent stuff ----------
    private static void AddHelicopter(Helicoper helicopter)
    {
        if (helicopter == null)
            throw new ArgumentException("Helicopter cannot be null.");

        _helicopters.Add(helicopter);
    }

    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving helicopters to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<Helicoper>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, _helicopters);
    }

    public static void Load()
    {
        Console.WriteLine("Loading helicopters from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Helicoper>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Helicoper> loaded)
        {
            _helicopters.Clear();
            _helicopters.AddRange(loaded);
        }
    }

    public static void DisplayAll()
    {
        if (_helicopters.Count == 0)
        {
            Console.WriteLine("No helicopters found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Helicopters ---\n");

        foreach (var h in _helicopters)
        {
            Console.WriteLine(h);
        }
    }

    public override string ToString()
    {
        return base.ToString()
               + "Aircraft type: Helicopter\n"
               + "-------------------------";
    }
}