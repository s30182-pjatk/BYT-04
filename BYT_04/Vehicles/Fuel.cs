using System;

namespace BYT_04.Vehicles;

[Serializable]
public class Fuel : VehiclePowerType
{
    private float _tankCapacity;

    public float TankCapacity
    {
        get => _tankCapacity;
        set => _tankCapacity = value;
    }

    public Fuel() { }

    public Fuel(float tankCapacity)
    {
        TankCapacity = tankCapacity;
        RegisterPowerType(this);
    }

    public override string ToString()
    {
        return "Fuel Power Type\n"
             + $"Tank capacity: {TankCapacity} L\n";
    }
}