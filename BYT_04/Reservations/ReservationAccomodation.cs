namespace BYT_04.Reservations;

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
        }
    }

    public DateTime CheckInDate
    {
        get => _checkInDate;
        set
        {
            if(value <= DateTime.Today)
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