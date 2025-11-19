namespace BYT_04;

public class Customer : Person
{
    public bool IsVip { get; set; }
    public int LoyaltyPoints { get; set; }

    public Customer()
    {
    }

    public Customer(string name, string? middleName, string surname, DateTime birthDate, string gender, string phoneNumber, string email, Address address, bool isVip, int loyaltyPoints) : base(name, middleName, surname, birthDate, gender, phoneNumber, email, address)
    {
        IsVip = isVip;
        LoyaltyPoints = loyaltyPoints;
    }

    public int CheckLoyaltyPoints()
    {
        return LoyaltyPoints;
    }
}