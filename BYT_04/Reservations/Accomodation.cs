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
    private string _number = null!;
    private AccomodationType _type;
    private int _capacity;


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
    }

    public override string ToString()
    {
        return $"Number: {Number}\n" +
               $"Type: {Type}\n" +
               $"Capacity: {Capacity}\n" +
               "-----------------------------";
    }
}

public static class AccomodationExtent
{ 
    public static List<Accomodation> Accomodations { get; private set; } = new();
    
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
        serializer.Serialize(fs, Accomodations);
    }

    public static void Load()
    {
        Console.WriteLine("Loading from: " + FilePath);

        if (!File.Exists(FilePath))
            return;

        XmlSerializer serializer = new(typeof(List<Accomodation>));

        using FileStream fs = new(FilePath, FileMode.Open);

        if (serializer.Deserialize(fs) is List<Accomodation> loaded)
            Accomodations = loaded;
    }

    public static void DisplayAll()
    {
        if (Accomodations.Count == 0)
        {
            Console.WriteLine("No Accomodations found.");
            return;
        }

        Console.WriteLine("\n--- Loaded Accomodations ---\n");

        foreach (var a in Accomodations)
        {
            Console.WriteLine(a);
        }
    }
}