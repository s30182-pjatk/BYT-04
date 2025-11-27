namespace BYT_04.Reservations;

using BYT_04.Utility;
using BYT_04.Vehicles;

public class ReservationVehicle
{
    private Vehicle _vehicle = null!;
    public Vehicle Vehicle
    {
        get => _vehicle;
        set => _vehicle = value ?? throw new ArgumentException("Vehicle cannot be null.");
    }
    private Reservation _reservation = null!;
    public Reservation Reservation
    {
        get => _reservation;
        set => _reservation = value ?? throw new ArgumentException("Reservation cannot be null.");
    }
    private string _usgagePurpose;
    public string UsgagePurpose
    {
        get => _usgagePurpose;
        set => _usgagePurpose = value.ValidateRequiredString(nameof(UsgagePurpose));
    }
    private string _conditionBefore;
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
    
}
