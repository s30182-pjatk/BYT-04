using BYT_04;

namespace BYT_04_Test;

public class Tests
{
    [Test]
    public void TestPersonInvalidDate()
    {
        var invalidDate = DateTime.Today.AddDays(1);
        Assert.Throws<ArgumentException>(() => new Person("Gleb", null, "Denisov", invalidDate, "male", "+48999999999", "email@gmail.com"));
    }
}