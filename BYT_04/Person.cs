namespace BYT_04;

public class Person
{
    public readonly string Name;
    public readonly string? MiddleName;
    public readonly string Surname;
    public readonly DateTime BirthDate;
    public readonly string Gender;
    public readonly int Age;
    public readonly string PhoneNumber;
    public readonly string Email;
    public readonly Address address;

    public Person(string name, string? middleName, string surname, DateTime birthDate, string gender, string phoneNumber, string email, Address address)
    {
        
        if (birthDate > DateTime.Today)
        {
            throw new ArgumentException("Birth date cannot be in the future.");
        }

        Name = name;
        MiddleName = middleName;
        Surname = surname;
        BirthDate = birthDate;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Email = email;

        Age = CalculateAge(birthDate);
        this.address = address;

    }

    private int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        int age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
    
}

public class Address
{
    public readonly string Street;
    public readonly string City;
    public readonly string State;
    public readonly string PostalCode;
    public readonly string Country;

    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
}