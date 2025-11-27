using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using BYT_04.Utility;

namespace BYT_04.Vehicles;

[Serializable]
public abstract class Vehicle
{
    private string _plateNumber;
    private string _model;
    private int _capacity;
    private bool _containMedKit;
    private VehiclePowerType _powerType = new GenericVehiclePowerType();

    public string PlateNumber
    {
        get => _plateNumber;
        set => value.ValidateRequiredString(nameof(_plateNumber));
    }
    public string Model
    {
        get => _model;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Model is required.", nameof(_model));
            _model = value;
        }
    }

    public int Capacity
    {
        get => _capacity;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(_capacity),
                    "Capacity must be greater than zero."
                );
            _capacity = value;
        }
    }

    public bool ContainMedKit
    {
        get => _containMedKit;
        set => _containMedKit = value;
    }

    public VehiclePowerType PowerType
    {
        get => _powerType;
        set => _powerType = value ?? throw new ArgumentNullException(nameof(PowerType));
    }
    // --------- Extent properties ----------
    private static readonly List<Vehicle> _vehicles = new();
    public static IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();

    private static string _directoryPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Vehicles", "persistence")
    );

    private static string FilePath => Path.Combine(_directoryPath, "vehicles.xml");

    public Vehicle() { }

    public Vehicle(string plateNumber, string model, int capacity, bool containMedKit)
        : this(plateNumber, model, capacity, containMedKit, new GenericVehiclePowerType())
    {
    }

    public Vehicle(
        string plateNumber,
        string model,
        int capacity,
        bool containMedKit,
        VehiclePowerType powerType
    )
    {
        PlateNumber = plateNumber;
        Model = model;
        Capacity = capacity;
        ContainMedKit = containMedKit;
        PowerType = powerType;

        AddVehicle(this);
    }

    // --------- Extent stuff ----------
    private static void AddVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new ArgumentException("Vehicle cannot be null.");

        _vehicles.Add(vehicle);
    }

    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving vehicles to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Vehicle>));
        using var fs = new FileStream(FilePath, FileMode.Create);
        serializer.Serialize(fs, _vehicles);
    }

    public static void Load()
    {
        Console.WriteLine("Loading vehicles from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Vehicle>));
        using var fs = new FileStream(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Vehicle> loaded)
        {
            _vehicles.Clear();
            _vehicles.AddRange(loaded);
        }
    }

    public static void DisplayAll()
    {
        if (_vehicles.Count == 0)
        {
            Console.WriteLine("No vehicles found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Vehicles ---\n");

        foreach (var v in _vehicles)
        {
            Console.WriteLine(v);
        }
    }

    public override string ToString()
    {
        return $"Plate Number: {PlateNumber}\n"
             + $"Model: {Model}\n"
             + $"Capacity: {Capacity}\n"
             + $"Contains MedKit: {ContainMedKit}\n"
             + $"Power Type: {PowerType}\n"
             + "-----------------------------";
    }
}


// --------- Vehicle Power Types extent ----------
[Serializable]
[XmlInclude(typeof(Fuel))]
[XmlInclude(typeof(Electric))]
public abstract class VehiclePowerType
{
    private static readonly List<VehiclePowerType> _powerTypes = new();
    public static IReadOnlyList<VehiclePowerType> PowerTypes => _powerTypes.AsReadOnly();

    private static string _directoryPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Vehicles", "persistence")
    );

    private static string FilePath => Path.Combine(_directoryPath, "powerTypes.xml");

    protected static void RegisterPowerType(VehiclePowerType powerType)
    {
        if (powerType == null)
            throw new ArgumentException("Power type cannot be null.");

        _powerTypes.Add(powerType);
    }

    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving power types to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<VehiclePowerType>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, _powerTypes);
    }

    public static void Load()
    {
        Console.WriteLine("Loading power types from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<VehiclePowerType>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<VehiclePowerType> loaded)
        {
            _powerTypes.Clear();
            _powerTypes.AddRange(loaded);
        }
    }

    public static void DisplayAll()
    {
        if (_powerTypes.Count == 0)
        {
            Console.WriteLine("No power types found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Power Types ---\n");

        foreach (var powerType in _powerTypes)
        {
            Console.WriteLine(powerType);
        }
    }
}

[Serializable]
internal sealed class GenericVehiclePowerType : VehiclePowerType
{
    public override string ToString()
    {
        return "Generic vehicle power type\n"
             + "-----------------------------";
    }
}