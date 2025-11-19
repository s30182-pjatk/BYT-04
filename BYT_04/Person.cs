namespace BYT_04;

public class Person
{
    public string Name { get; set; }
    public string? MiddleName { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public int Age { get; private set; }  // Age setter private so it's only changed internally
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; }

    public Person(
        string name,
        string? middleName,
        string surname,
        DateTime birthDate,
        string gender,
        string phoneNumber,
        string email,
        Address address)
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
        Address = address;

        Age = CalculateAge(birthDate);
    }

    public Person()
    {
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

    // If someone changes BirthDate later via setter, update Age accordingly:
    public void UpdateAge()
    {
        Age = CalculateAge(BirthDate);
    }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
}