using System;

namespace BYT_04.Vehicles;

[Serializable]
public class Electric : VehiclePowerType
{
    private float _batteryCapacity;

    public float BatteryCapacity
    {
        get => _batteryCapacity;
        set => _batteryCapacity = value;
    }

    public Electric() { }

    public Electric(float batteryCapacity)
    {
        BatteryCapacity = batteryCapacity;
        RegisterPowerType(this);
    }

    public override string ToString()
    {
        return "Electric Power Type\n"
             + $"Battery capacity: {BatteryCapacity} kWh\n"
             + "-----------------------------";
    }
}