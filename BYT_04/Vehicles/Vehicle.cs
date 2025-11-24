using BYT_04.Utility;

namespace BYT_04.Vehicles;

[Serializable]
public class Vehicle
{
    private string _plateNumber;
    private string _model;
    private int _capacity;
    private bool _containMedKit;

    public string PlateNumber
    {
        get => _plateNumber;
        set => value.ValidateRequiredString(nameof(_plateNumber));
    }
    
}