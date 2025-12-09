using System.Xml.Serialization;
using BYT_04.Utility;
namespace BYT_04.Reservations;
public enum AccomodationType
{
    Room,
    Cabin
}

[Serializable]
public class Accomodation
{
    private static readonly List<Accomodation> _accomodations = new();
    public static IReadOnlyList<Accomodation> Accomodations => _accomodations.AsReadOnly();
    
    private string _number = null!;
    private AccomodationType _type;
    private int _capacity;
    
    // Association
    [XmlIgnore]
    private HashSet<ReservationAccomodation> _reservationAccomodations = new();
    
    [XmlIgnore]
    public IEnumerable<ReservationAccomodation> ReservationAccomodations => _reservationAccomodations.ToList();

    public string Number
    {
        get => _number;
        set => _number = value.ValidateRequiredString(nameof(Number));
    }
    
    public AccomodationType Type
    {
        get => _type;
        set => _type = value;
    }

    public int Capacity
    {
        get => _capacity;
        set
        {
            if (value <= 0) 
                throw new ArgumentException("Capacity must be greater than zero."); 
                _capacity = value;
        }
    }
    
    public Accomodation(){}

    public Accomodation(string number, AccomodationType type, int capacity)
    {
        Number = number;
        Type = type;
        Capacity = capacity;
        
        AddAccomodation(this);
    }
    
    // Association Methods
    public void AddReservationAccomodation(ReservationAccomodation ra)
    {
        if (ra == null) return;

        // Prevent infinite recursion and duplicates
        if (!_reservationAccomodations.Contains(ra))
        {
            _reservationAccomodations.Add(ra);

            // Trigger Reverse Connection
            if (ra.Accomodation != this)
            {
                ra.Accomodation = this;
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

    private static void AddAccomodation(Accomodation accomodation)
    {
        if (accomodation == null)
        {
            throw new ArgumentException("Accomodation cannot be null.");
        }
        _accomodations.Add(accomodation);
    }

    public override string ToString()
    {
        return $"Number: {Number}\n" +
               $"Type: {Type}\n" +
               $"Capacity: {Capacity}\n" +
               "-----------------------------";
    }
    
    //Persistence
    
    private static string _directoryPath =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reservations", "persistence"));

    private static string FilePath => Path.Combine(_directoryPath, "accomodations.xml");
    
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

        XmlSerializer serializer = new(typeof(List<Accomodation>));

        using FileStream fs = new(FilePath, FileMode.Create);
        serializer.Serialize(fs, _accomodations);
    }

    public static void Load()
    {
        Console.WriteLine("Loading from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Accomodation>));

        using FileStream fs = new(FilePath, FileMode.Open);

        var loaded = serializer.Deserialize(fs) as List<Accomodation>;

        
        if (loaded != null)
        {
            _accomodations.Clear();
            _accomodations.AddRange(loaded);
        }
    }

    public static void DisplayAll()
    {
        if (_accomodations.Count == 0)
        {
            Console.WriteLine("No Accomodations found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Accomodations ---\n");

        foreach (var a in _accomodations)
        {
            Console.WriteLine(a);
        }
    }
}

