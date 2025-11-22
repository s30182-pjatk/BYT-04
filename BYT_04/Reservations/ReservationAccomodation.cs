using System.Xml.Serialization;

namespace BYT_04.Reservations;

[Serializable]
public class ReservationAccomodation
{
    private Reservation _reservation = null!;
    private Accomodation _accomodation = null!;
    private int _numberOfGuests;
    private DateTime _checkInDate;
    private DateTime _checkOutDate;
    private string _conditionBefore = null!;
    private string? _conditionAfter;
    private string? _notes;

    public Reservation Reservation
    {
        get => _reservation;
        set => _reservation = value ?? throw new ArgumentException("Reservation cannot be null.");
    }

    public Accomodation Accomodation
    {
        get => _accomodation;
        set => _accomodation = value ?? throw new ArgumentException("Accomodation cannot be null.");
    }

    public int NumberOfGuests
    {
        get => _numberOfGuests;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Number of guests must be greater than zero.");
            _numberOfGuests = value;
        }
    }

    public DateTime CheckInDate
    {
        get => _checkInDate;
        set
        {
            if(value > DateTime.Today)
                throw new ArgumentException("Check-In date cannot be in the future.");
            _checkInDate = value;
        }
    }
    
    public DateTime CheckOutDate
    {
        get => _checkOutDate;
        set
        {
            if(value < CheckInDate)
                throw new ArgumentException("Check-out date cannot be earlier than Check-In-Date.");
        }
    }

    public string ConditionBefore
    {
        get => _conditionBefore;
        set => _conditionBefore = ValidateRequiredString(value, nameof(ConditionBefore));
    }

    public string? ConditionAfter
    {
        get => _conditionAfter;
        set => _conditionAfter = ValidateRequiredString(value, nameof(ConditionAfter));
    }

    public string? Notes
    {
        get => _notes;
        set => _notes = ValidateRequiredString(value, nameof(Notes));
    }
    
    public ReservationAccomodation(){}

    public ReservationAccomodation(
        Reservation reservation,
        Accomodation accomodation,
        int numberOfGuests,
        DateTime checkInDate,
        DateTime checkOutDate,
        string conditionBefore,
        string? conditionAfter,
        string? notes)
    {
        Reservation = reservation;
        Accomodation = accomodation;
        NumberOfGuests = numberOfGuests;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        ConditionBefore = conditionBefore;
        ConditionAfter = conditionAfter;
        Notes = notes;
    }
    
    
    private static string ValidateRequiredString(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{propertyName} cannot be null, empty, or whitespace.");

        return value;
    }
    
}

public static class ReservationAccomodationExtent
{
    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reservations", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "reservationaccomodation.xml");

    public static List<ReservationAccomodation> ReservationAccomodations { get; private set; } = new();

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

        XmlSerializer serializer = new(typeof(List<ReservationAccomodation>));

        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, ReservationAccomodations);
    }

    public static void Load()
    {
        Console.WriteLine("Loading from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<ReservationAccomodation>));

        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<ReservationAccomodation> loaded)
            ReservationAccomodations = loaded;
    }

    public static void DisplayAll()
    {
        if (ReservationAccomodations.Count == 0)
        {
            Console.WriteLine("No reservation-accommodation links found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Reservation-Accommodation ---\n");

        foreach (var ra in ReservationAccomodations)
        {
            Console.WriteLine(
                $"Reservation ID: {ra.Reservation.ReservationId}\n" +
                $"Accommodation Number: {ra.Accomodation.Number}\n" +
                $"Type: {ra.Accomodation.Type}\n" +
                $"Capacity: {ra.Accomodation.Capacity}\n" +
                $"Number of Guests: {ra.NumberOfGuests}\n" +
                $"Check-In: {ra.CheckInDate.ToShortDateString()}\n" +
                $"Check-Out: {ra.CheckOutDate.ToShortDateString()}\n" +
                $"Condition Before: {ra.ConditionBefore}\n" +
                $"Condition After: {ra.ConditionAfter ?? "N/A"}\n" +
                $"Notes: {ra.Notes ?? "N/A"}\n" +
                "-----------------------------\n"
            );
        }
    }
}