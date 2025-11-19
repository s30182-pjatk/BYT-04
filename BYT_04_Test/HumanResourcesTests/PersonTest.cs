using BYT_04;

namespace BYT_04_Test;

public class PersonTests
{
    [Test]
    public void TestPersonInvalidDate()
    {
        var invalidDate = DateTime.Today.AddDays(1);

        var address = new Address("some", "another", "dom", "somecode", "someplace");
        Assert.Throws<ArgumentException>(() => new Person("Gleb", null, "Denisov", invalidDate, "male", "+48999999999", "email@gmail.com", address));
    }
    
    [Test]
    public void TestPersonCorrectAge()
    {
        DateTime date1 = new DateTime(2015, 8, 28);

        var address = new Address("some", "another", "dom", "somecode", "someplace");
        var person = new Person("Gleb", null, "Denisov", date1, "male", "+48999999999", "email@gmail.com", address);
        
        Assert.That(person.Age, Is.EqualTo(10));
    }
}