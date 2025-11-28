namespace BYT_04;
using Reservations;
using Vehicles;

public class Program
{
    static void Main()
    {
        // //PEOPLE
        // // Load existing people
        Person.Load();
        
        // OPTIONAL: Add a new person to test persistence
        var person = new Person(
            "John", "A", "Doe",
            new DateTime(1990, 5, 12),
            "Male",
            "123456789",
            "john.doe@example.com",
            new Address("123 Street", "City", "State", "11111", "Country")
        );
        
        // Save to XML
        Person.Save();
        
        // Display loaded persons
        Person.DisplayAll();
        
        //--------------------------------------------------------------------------------------------------------------
        
        //RESERVATIONS
        // Load existing reservations
        Reservation.Load();
        
        var reservation = new Reservation(
            1,
            DateTime.Today, 
            DateTime.Today.AddDays(7),
            ReservationStatus.Pending,
            105
        );
        
        Reservation.Save();
        
        Reservation.DisplayAll();
        
        //-------------------------------------------------------------------------------------------------------
        
        //ACCOMODATIONS
        Accomodation.Load();
        
        var accomodation = new Accomodation(
            "A160",
            AccomodationType.Room,
            7);
        
        //Save to XML
        Accomodation.Save();
        
        //Display loaded Accomodations
        Accomodation.DisplayAll();
        

        //-------------------------------------------------------------------------------------------------------------

        //ReservationAccomodation
        ReservationAccomodation.Load();
        
        var reservationaccomodation = new ReservationAccomodation(
            reservation,
            accomodation,
            5,
            DateTime.Today, 
            DateTime.Today.AddDays(7),
            "Good",
            notes: "Heater needs to be fixed"
        );
        
        ReservationAccomodation.Save();
        ReservationAccomodation.DisplayAll();

    
        //VEHICLES
        Vehicles.Vehicle.Load();
        var vehicle = new SUV("1234567890", "Toyota", 5, true, new Fuel(100f), true);
        var vehicle2 = new ATV("GB QW491", "Mercedes", 4, true, new Electric(34.5f));
        Vehicles.Vehicle.Save();
        Vehicles.Vehicle.DisplayAll();

    }
    
    
}