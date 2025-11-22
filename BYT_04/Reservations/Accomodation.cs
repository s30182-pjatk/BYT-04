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
        set => _number = ValidateRequiredString(value, nameof(Number));
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
            {
                throw new ArgumentException("Capacity must be positive.");
            }
        }
    }
    
    public Accomodation(){}

    public Accomodation(string number, AccomodationType type, int capacity)
    {
        Number = number;
        Type = type;
        Capacity = capacity;
    }
    
    private static string ValidateRequiredString(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{propertyName} cannot be null, empty, or whitespace.");

        return value;
    }
}