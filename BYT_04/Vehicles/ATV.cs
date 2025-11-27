using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace BYT_04.Vehicles;

[Serializable]
public class ATV : Vehicle
{
    private static bool _hasWinterTires = true;
    private static bool _isFourWheelDrive = true;
    private float _maxSpeedInKpH;
    private bool _hasGargoWrack;

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

    public bool HasGargoWrack
    {
        get => _hasGargoWrack;
        set => _hasGargoWrack = value;
    }

    // --------- Extent properties ----------
    private static readonly List<ATV> _atvs = new();
    public static IReadOnlyList<ATV> ATVs => _atvs.AsReadOnly();

    private static string _directoryPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Vehicles", "persistence")
    );

    private static string FilePath => Path.Combine(_directoryPath, "atvs.xml");

    public ATV() { }

    public ATV(string plateNumber, string model, int capacity, bool containMedKit, VehiclePowerType powerType)
        : base(plateNumber, model, capacity, containMedKit, powerType)
    {
        AddATV(this);
    }

    // --------- Extent stuff ----------
    private static void AddATV(ATV atv)
    {
        if (atv == null)
            throw new ArgumentException("ATV cannot be null.");

        _atvs.Add(atv);
    }

    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving ATVs to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<ATV>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, _atvs);
    }

    public static void Load()
    {
        Console.WriteLine("Loading ATVs from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<ATV>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<ATV> loaded)
        {
            _atvs.Clear();
            _atvs.AddRange(loaded);
        }
    }

    public static void DisplayAll()
    {
        if (_atvs.Count == 0)
        {
            Console.WriteLine("No ATVs found.");
            return;
        }

        Console.WriteLine("\n--- Loaded ATVs ---\n");

        foreach (var a in _atvs)
        {
            Console.WriteLine(a);
        }
    }
}