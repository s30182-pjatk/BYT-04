using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace BYT_04.Vehicles;

[Serializable]
public class SUV : Vehicle
{
    private static bool _hasWinterTires;
    private static bool _isFourWheelDrive;
    private float _maxSpeedInKpH;

    public static bool HasWinterTires
    {
        get => _hasWinterTires;
        set => _hasWinterTires = value;
    }

    public static bool IsFourWheelDrive
    {
        get => _isFourWheelDrive;
        set => _isFourWheelDrive = value;
    }

    public float MaxSpeedInKpH
    {
        get => _maxSpeedInKpH;
        set => _maxSpeedInKpH = value;
    }

    // --------- Extent properties ----------
    private static readonly List<SUV> _suvs = new();
    public static IReadOnlyList<SUV> SUVs => _suvs.AsReadOnly();

    private static string _directoryPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Vehicles", "persistence")
    );

    private static string FilePath => Path.Combine(_directoryPath, "suvs.xml");

    public SUV() { }

    public SUV(string plateNumber, string model, int capacity, bool containMedKit)
        : base(plateNumber, model, capacity, containMedKit)
    {
        AddSUV(this);
    }

    // --------- Extent stuff ----------
    private static void AddSUV(SUV suv)
    {
        if (suv == null)
            throw new ArgumentException("SUV cannot be null.");

        _suvs.Add(suv);
    }

    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving SUVs to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<SUV>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, _suvs);
    }

    public static void Load()
    {
        Console.WriteLine("Loading SUVs from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<SUV>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<SUV> loaded)
        {
            _suvs.Clear();
            _suvs.AddRange(loaded);
        }
    }

    public static void DisplayAll()
    {
        if (_suvs.Count == 0)
        {
            Console.WriteLine("No SUVs found.");
            return;
        }

        Console.WriteLine("\n--- Loaded SUVs ---\n");

        foreach (var s in _suvs)
        {
            Console.WriteLine(s);
        }
    }
}