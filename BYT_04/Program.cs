namespace BYT_04;

public class Program
{
    static void Main()
    {
        // Load existing people
        PersonExtent.Load();

        // OPTIONAL: Add a new person to test persistence
        var person = new Person(
            "John", "A", "Doe",
            new DateTime(1990, 5, 12),
            "Male",
            "123456789",
            "john.doe@example.com",
            new Address("123 Street", "City", "State", "11111", "Country")
        );

        PersonExtent.Persons.Add(person);

        // Save to XML
        PersonExtent.Save();

        // Display loaded persons
        PersonExtent.DisplayAll();
    }
}