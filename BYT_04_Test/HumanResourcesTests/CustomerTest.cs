using BYT_04;

namespace BYT_04_Test;

public class CustomerTests
{
    [Test]
    public void TestCustomerCheckBalance()
    {
        DateTime date1 = new DateTime(2015, 8, 28);

        var address = new Address("some", "another", "dom", "somecode", "someplace");
        var person = new Customer("Gleb", null, "Denisov", date1, "male", "+48999999999", "email@gmail.com", address, true, 20);
        
        Assert.That(person.LoyaltyPoints, Is.EqualTo(20));

    }
    
    [Test]
    public void TestCustomerIsVip()
    {
        DateTime date1 = new DateTime(2015, 8, 28);

        var address = new Address("some", "another", "dom", "somecode", "someplace");
        var person = new Customer("Gleb", null, "Denisov", date1, "male", "+48999999999", "email@gmail.com", address, true, 20);
        
        Assert.That(person.IsVip, Is.EqualTo(true));
    }
}