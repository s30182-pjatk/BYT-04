using System.Xml.Serialization;

namespace BYT_04;

public enum ReservationStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}

[Serializable]
public class Reservation
{
    private int _reservationId;
    private DateTime _startDate;
    private DateTime _endDate;
    private ReservationStatus _status;
    private decimal _totalPrice;
    
    
    public int ReservationId
    {
        get => _reservationId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Reservation ID must be positive.");
            _reservationId = value;
        }
    }
    
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
    
    public ReservationStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public decimal TotalPrice
    {
        get => _totalPrice;
        set
        {
            if (value < 0)
                throw new ArgumentException("Total price cannot be negative.");
            _totalPrice = value;
        }
    }
    
    public Reservation() { }

    public Reservation(int reservationId,
        DateTime startDate,
        DateTime endDate,
        ReservationStatus status,
        decimal totalPrice)
    {
        ReservationId = reservationId;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        TotalPrice = totalPrice;
    }
    
    // Methods
    public void FinalizeReservation()
    {
        if (Status == ReservationStatus.Pending)
        {
            Status = ReservationStatus.Confirmed;
        }
        else
        {
            throw new InvalidOperationException("Only pending reservations can be finalized.");
        }
    }
    
    
    public void ChangeReservationStatus(ReservationStatus newStatus)
    {
        Status = newStatus;
    }
}

public static class ReservationExtent
{
    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reservations", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "reservations.xml");

    public static List<Reservation> Reservations { get; private set; } = new();

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

        XmlSerializer serializer = new(typeof(List<Reservation>));
        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, Reservations);
    }

    public static void Load()
    {
        Console.WriteLine("Loading from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Reservation>));
        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Reservation> loaded)
            Reservations = loaded;
    }

    public static List<Reservation> CheckPendingReservations()
    {
        return Reservations.FindAll(r => r.Status == ReservationStatus.Pending);
    }

    public static void RemoveCompletedReservations()
    {
        Reservations.RemoveAll(r => r.Status == ReservationStatus.Completed);
    }

    public static void DisplayAll()
    {
        if (Reservations.Count == 0)
        {
            Console.WriteLine("No reservations found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Reservations ---\n");

        foreach (var r in Reservations)
        {
            Console.WriteLine(
                $"ID: {r.ReservationId}\n" +
                $"Start: {r.StartDate.ToShortDateString()}\n" +
                $"End: {r.EndDate.ToShortDateString()}\n" +
                $"Status: {r.Status}\n" +
                $"Price: {r.TotalPrice}\n" +
                "-----------------------------\n"
            );
        }
    }
}