using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using BYT_04.Utility;

namespace BYT_04.Reservations;

[Serializable]
public class Trip
{
    // --------- Extent ----------
    private static readonly List<Trip> _trips = new();
    public static IReadOnlyList<Trip> Trips => _trips.AsReadOnly();

    private static string _directoryPath =
        Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reservations", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "trips.xml");

    
    private string _name = null!;
    private string _destination = null!;
    private string? _description;
    private DateTime _startDate;
    private DateTime _endDate;
    private decimal _pricePerPerson;
    
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

    public string Destination
    {
        get => _destination;
        set => _destination = value.ValidateRequiredString(nameof(Destination));
    }

   
    public string? Description
    {
        get => _description;
        set => _description = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    
    [XmlIgnore]
    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            if (value < DateTime.Today)
                throw new ArgumentException("Start date cannot be in the past.");
            _startDate = value;
        }
    }

    [XmlElement("StartDate")]
    public DateTime StartDateSerialized
    {
        get => _startDate;
        set => _startDate = value;  
    }

    [XmlIgnore]
    public DateTime EndDate
    {
        get => _endDate;
        set
        {
            if (value < StartDate)
                throw new ArgumentException("End date cannot be earlier than start date.");
            _endDate = value;
        }
    }

    [XmlElement("EndDate")]
    public DateTime EndDateSerialized
    {
        get => _endDate;
        set => _endDate = value;     
    }

    public decimal PricePerPerson
    {
        get => _pricePerPerson;
        set
        {
            if (value < 0)
                throw new ArgumentException("Price per person cannot be negative.");
            _pricePerPerson = value;
        }
    }
    
    public Trip() { }

    public Trip(
        string name,
        string destination,
        DateTime startDate,
        DateTime endDate,
        decimal pricePerPerson,
        string? description = null)
    {
        Name = name;
        Destination = destination;
        StartDate = startDate;
        EndDate = endDate;
        PricePerPerson = pricePerPerson;
        Description = description;

        AddTrip(this);
    }
    
    // Associations Methods
    public void AddTripEquipment(TripEquipment te)
    {
        if (te == null) return;

        // Prevent infinite recursion and duplicates
        if (!_tripEquipments.Contains(te))
        {
            _tripEquipments.Add(te);

            // Trigger Reverse Connection
            if (te.Trip != this)
            {
                te.Trip = this;
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
    private static void AddTrip(Trip trip)
    {
        if (trip == null)
            throw new ArgumentException("Trip cannot be null.");

        _trips.Add(trip);
    }

    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving trips to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<Trip>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, _trips);
    }

    public static void Load()
    {
        Console.WriteLine("Loading trips from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Trip>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Trip> loaded)
        {
            _trips.Clear();
            _trips.AddRange(loaded);
        }
    }

    public static void DisplayAll()
    {
        if (_trips.Count == 0)
        {
            Console.WriteLine("No trips found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Trips ---\n");

        foreach (var t in _trips)
        {
            Console.WriteLine(t);
        }
    }

    public override string ToString()
    {
        return $"Name: {Name}\n" +
               $"Destination: {Destination}\n" +
               $"Start: {StartDate:yyyy-MM-dd}\n" +
               $"End: {EndDate:yyyy-MM-dd}\n" +
               $"Price per person: {PricePerPerson}\n" +
               $"Description: {Description ?? "N/A"}\n" +
               "-----------------------------";
    }
}
