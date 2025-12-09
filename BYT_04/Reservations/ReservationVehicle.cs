namespace BYT_04.Reservations;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using BYT_04.Utility;
using BYT_04.Vehicles;

[Serializable]
public class ReservationVehicle
{
    private static readonly List<ReservationVehicle> _reservationVehicles = new();
    public static IReadOnlyList<ReservationVehicle> ReservationVehicles => _reservationVehicles.AsReadOnly();

    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reservations", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "reservationvehicles.xml");

    private Vehicle _vehicle = null!;

    public Vehicle Vehicle
    {
        get => _vehicle;
        set
        {
            _vehicle = value ?? throw new ArgumentException("Vehicle cannot be null.");
            // Trigger Reverse Connection
            _vehicle.AddReservationVehicle(this);
        }
    }

    private Reservation _reservation = null!;

    public Reservation Reservation
    {
        get => _reservation;
        set
        {
            _reservation = value ?? throw new ArgumentException("Reservation cannot be null.");
            // Trigger Reverse Connection
            _reservation.AddReservationVehicle(this);
        }
    }

    private string _usgagePurpose = null!;

    public string UsgagePurpose
    {
        get => _usgagePurpose;
        set => _usgagePurpose = value.ValidateRequiredString(nameof(UsgagePurpose));
    }

    private string _conditionBefore = null!;

    public string ConditionBefore
    {
        get => _conditionBefore;
        set => _conditionBefore = value.ValidateRequiredString(nameof(ConditionBefore));
    }

    private string? _conditionAfter;

    public string? ConditionAfter
    {
        get => _conditionAfter;
        set => _conditionAfter = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private float _fuelLevelBefore;
    private float _fuelLevelAfter;

    public float FuelLevelBefore
    {
        get => _fuelLevelBefore;
        set
        {
            if (value < 0)
                throw new ArgumentException("Fuel level before usage cannot be negative.");
            _fuelLevelBefore = value;
        }
    }

    public float FuelLevelAfter
    {
        get => _fuelLevelAfter;
        set
        {
            if (value < 0)
                throw new ArgumentException("Fuel level after usage cannot be negative.");
            _fuelLevelAfter = value;
        }
    }

    private string? _notes;

    public string? Notes
    {
        get => _notes;
        set => _notes = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public ReservationVehicle()
    {
    }

    public ReservationVehicle(
        Reservation reservation,
        Vehicle vehicle,
        string usgagePurpose,
        string conditionBefore,
        float fuelLevelBefore,
        float fuelLevelAfter,
        string? conditionAfter = null,
        string? notes = null)
    {
        Reservation = reservation;
        Vehicle = vehicle;
        UsgagePurpose = usgagePurpose;
        ConditionBefore = conditionBefore;
        FuelLevelBefore = fuelLevelBefore;
        FuelLevelAfter = fuelLevelAfter;
        ConditionAfter = conditionAfter;
        Notes = notes;

        AddReservationVehicle(this);
    }

    public override string ToString()
    {
        return $"Reservation ID: {Reservation.ReservationId}\n" +
               $"Vehicle Plate: {Vehicle.PlateNumber}\n" +
               $"Vehicle Model: {Vehicle.Model}\n" +
               $"Usage Purpose: {UsgagePurpose}\n" +
               $"Fuel Before: {FuelLevelBefore}\n" +
               $"Fuel After: {FuelLevelAfter}\n" +
               $"Condition Before: {ConditionBefore}\n" +
               $"Condition After: {ConditionAfter ?? "N/A"}\n" +
               $"Notes: {Notes ?? "N/A"}\n" +
               "-----------------------------";
    }

    private static void AddReservationVehicle(ReservationVehicle reservationVehicle)
    {
        if (reservationVehicle == null)
            throw new ArgumentException("ReservationVehicle cannot be null.");

        _reservationVehicles.Add(reservationVehicle);
    }

    public static void SetDirectory(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
            throw new ArgumentException("Directory cannot be null or empty.");

        _directoryPath = newDirectory;
    }

    public static void Save()
    {
        Console.WriteLine("Saving reservation vehicles to: " + FilePath);

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        XmlSerializer serializer = new(typeof(List<ReservationVehicle>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, _reservationVehicles);
    }

    public static void Load()
    {
        Console.WriteLine("Loading reservation vehicles from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<ReservationVehicle>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<ReservationVehicle> loaded)
        {
            _reservationVehicles.Clear();
            _reservationVehicles.AddRange(loaded);
        }
    }

    public static void DisplayAll()
    {
        if (_reservationVehicles.Count == 0)
        {
            Console.WriteLine("No reservation-vehicle links found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Reservation-Vehicles ---\n");

        foreach (var rv in _reservationVehicles)
        {
            Console.WriteLine(rv);
        }
    }
}
