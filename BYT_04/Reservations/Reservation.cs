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