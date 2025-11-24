using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace BYT_04.Reservations;

[Serializable]
public class TripEquipment
{
    [XmlIgnore] private Trip _trip = null!;

    [XmlIgnore] private Equipment _equipment = null!;

    private int _quantity;
    private string? _notes;

    // -------- extent properties --------
    public static List<TripEquipment> TripEquipments { get; private set; } = new();

    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reservations", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "tripequipment.xml");

    public Trip Trip
    {
        get => _trip;
        set => _trip = value ?? throw new ArgumentException("Trip cannot be null.");
    }

    public Equipment Equipment
    {
        get => _equipment;
        set => _equipment = value ?? throw new ArgumentException("Equipment cannot be null.");
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
            _quantity = value;
        }
    }


    public string? Notes
    {
        get => _notes;
        set => _notes = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public TripEquipment()
    {
    }

    public TripEquipment(Trip trip, Equipment equipment, int quantity, string? notes = null)
    {
        Trip = trip;
        Equipment = equipment;
        Quantity = quantity;
        Notes = notes;

        TripEquipments.Add(this);
    }

    // -------- extent stuff --------
    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<TripEquipment>));

        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, TripEquipments);
    }

    public static void Load()
    {
        Console.WriteLine("Loading from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<TripEquipment>));

        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<TripEquipment> loaded)
            TripEquipments = loaded;
    }

    public static void DisplayAll()
    {
        if (TripEquipments.Count == 0)
        {
            Console.WriteLine("No trip-equipment links found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Trip-Equipment ---\n");

        foreach (var te in TripEquipments)
            Console.WriteLine(te);
    }

    public override string ToString()
    {
        return
            $"Trip: {Trip.Name}\n" +
            $"Destination: {Trip.Destination}\n" +
            $"Equipment: {Equipment.Name}\n" +
            $"Quantity: {Quantity}\n" +
            $"Notes: {Notes ?? "N/A"}\n" +
            "-----------------------------\n";
    }
}