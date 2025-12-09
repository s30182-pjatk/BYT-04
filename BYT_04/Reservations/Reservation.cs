using System.Xml.Serialization;

namespace BYT_04.Reservations;

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
    private static readonly List<Reservation> _reservations = new(); 
    public static IReadOnlyList<Reservation> Reservations => _reservations.AsReadOnly();
    
    private int _reservationId;
    private DateTime _startDate;
    private DateTime _endDate;
    private ReservationStatus _status;
    private decimal _totalPrice;
    
    // Association
    [XmlIgnore]
    private HashSet<ReservationAccomodation> _reservationAccomodations = new();
    [XmlIgnore]
    public IEnumerable<ReservationAccomodation> ReservationAccomodations => _reservationAccomodations.ToList();
    
    [XmlIgnore]
    private HashSet<ReservationVehicle> _reservationVehicles = new();
    [XmlIgnore]
    public IEnumerable<ReservationVehicle> ReservationVehicles => _reservationVehicles.ToList();
    
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
    
    //We ignore this so the Serializer doesn't crash on the validation logic
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
    
    //used to avoid validating StartDate when loading from xml file
    [XmlElement("StartDate")]
    public DateTime StartDateSerialized
    {
        get => _startDate;
        set => _startDate = value; //runs when loading the file, sets the field directly which fixes validation errors when loading
    }
    
    //We ignore this so the Serializer doesn't crash on the validation logic
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
    
    //used to avoid validating StartDate when loading from xml file and
    [XmlElement("EndDate")]
    public DateTime EndDateSerialized
    {
        get => _endDate;
        set => _endDate = value; //runs when loading the file, sets the field directly which fixes validation errors when loading
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
        
        AddReservation(this);
    }
    
    // Association Methods
    public void AddReservationAccomodation(ReservationAccomodation ra)
    {
        if (ra == null) return;

        if (!_reservationAccomodations.Contains(ra))
        {
            _reservationAccomodations.Add(ra);
            
            // Trigger Reverse Connection
            if (ra.Reservation != this)
            {
                ra.Reservation = this;
            }
        }
    }

    public void RemoveReservationAccomodation(ReservationAccomodation ra)
    {
        if (ra != null && _reservationAccomodations.Contains(ra))
        {
            _reservationAccomodations.Remove(ra);
        }
    }
    
    public void AddReservationVehicle(ReservationVehicle rv)
    {
        if (rv == null) return;

        // Prevent infinite recursion and duplicates
        if (!_reservationVehicles.Contains(rv))
        {
            _reservationVehicles.Add(rv);

            // Trigger Reverse Connection
            if (rv.Reservation != this)
            {
                rv.Reservation = this;
            }
        }
    }

    public void RemoveReservationVehicle(ReservationVehicle rv)
    {
        if (rv != null && _reservationVehicles.Contains(rv))
        {
            _reservationVehicles.Remove(rv);
        }
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
    
    private static void AddReservation(Reservation reservation)
    {
        if (reservation == null)
        {
            throw new ArgumentException("Reservation cannot be null");
        }
        _reservations.Add(reservation);
    }
    
    public static List<Reservation> CheckPendingReservations()
    {
        return _reservations.FindAll(r => r.Status == ReservationStatus.Pending);
    }
    
    public static void RemoveCompletedReservations()
    {
        _reservations.RemoveAll(r => r.Status == ReservationStatus.Completed);
    }
    
    public void ChangeReservationStatus(ReservationStatus newStatus)
    {
        Status = newStatus;
    }

    public override string ToString()
    {
        return $"ID: {ReservationId}\n" +
               $"Start: {StartDate.ToShortDateString()}\n" +
               $"End: {EndDate.ToShortDateString()}\n" +
               $"Status: {Status}\n" +
               $"Price: {TotalPrice}\n" +
               "-----------------------------" ;
    }
    
    //Persistence
    
    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reservations", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "reservations.xml");
    
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
        serializer.Serialize(fs, _reservations);
    }

    public static void Load()
    {
        Console.WriteLine("Loading from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Reservation>));
        using FileStream fs = new(FilePath, FileMode.Open);

        var loaded = serializer.Deserialize(fs) as List<Reservation>;

        
        if (loaded != null)
        {
            _reservations.Clear();
            _reservations.AddRange(loaded);
        }
    }
    
    public static void DisplayAll()
    {
        if (_reservations.Count == 0)
        {
            Console.WriteLine("No reservations found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Reservations ---\n");

        foreach (var r in _reservations)
        {
            Console.WriteLine(r);
        }
    }
}
