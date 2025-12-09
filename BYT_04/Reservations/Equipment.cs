using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using BYT_04.Utility;

namespace BYT_04.Reservations;

[Serializable]
public class Equipment
{
    // --------- Extent properties ----------
    private static readonly List<Equipment> _equipments = new();
    public static IReadOnlyList<Equipment> Equipments => _equipments.AsReadOnly();

    private static string _directoryPath =
        Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reservations", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "equipment.xml");
 
    private string _name = null!;
    private DateTime _lastMaintenanceDate;
    
    // Association
    [XmlIgnore]
    private HashSet<TripEquipment> _tripEquipments = new();

    [XmlIgnore]
    public IEnumerable<TripEquipment> TripEquipments => _tripEquipments.ToList();

    public string Name
    {
        get => _name;
        set => _name = value.ValidateRequiredString(nameof(Name));
    }

    public DateTime LastMaintenanceDate
    {
        get => _lastMaintenanceDate;
        set
        {
            if (value > DateTime.Today)
                throw new ArgumentException("Last maintenance date cannot be in the future.");
            _lastMaintenanceDate = value;
        }
    }
    public Equipment() { }

    public Equipment(string name, DateTime lastMaintenanceDate)
    {
        Name = name;
        LastMaintenanceDate = lastMaintenanceDate;

        AddEquipment(this);
    }
    
    // Association Methods
    public void AddTripEquipment(TripEquipment te)
    {
        if (te == null) return;

        // Prevent infinite recursion and duplicates
        if (!_tripEquipments.Contains(te))
        {
            _tripEquipments.Add(te);

            // Trigger Reverse Connection
            if (te.Equipment != this)
            {
                te.Equipment = this;
            }
        }
    }

    public void RemoveTripEquipment(TripEquipment te)
    {
        if (te != null && _tripEquipments.Contains(te))
        {
            _tripEquipments.Remove(te);
        }
    }

    // --------- Extent stuff ----------
    private static void AddEquipment(Equipment equipment)
    {
        if (equipment == null)
            throw new ArgumentException("Equipment cannot be null.");

        _equipments.Add(equipment);
    }

    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving equipment to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<Equipment>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, _equipments);
    }

    public static void Load()
    {
        Console.WriteLine("Loading equipment from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Equipment>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Equipment> loaded)
        {
            _equipments.Clear();
            _equipments.AddRange(loaded);
        }
    }

    public static void DisplayAll()
    {
        if (_equipments.Count == 0)
        {
            Console.WriteLine("No equipment found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Equipment ---\n");

        foreach (var e in _equipments)
        {
            Console.WriteLine(e);
        }
    }
    public override string ToString()
    {
        return $"Name: {Name}\n" +
               $"Last maintenance: {LastMaintenanceDate.ToShortDateString()}\n" +
               "-----------------------------";
    }
}
