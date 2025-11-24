namespace BYT_04.Reservations;

public class Program
{
    static void Main()
    {
        //PEOPLE
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
        // ReservationAccomodationExtent.Load();
        //
        // var reservationaccomodation = new ReservationAccomodation(
        //     reservation,
        //     accomodation,
        //     5,
        //     DateTime.Today, 
        //     DateTime.Today.AddDays(7),
        //     "Good",
        //     notes: "Heater needs to be fixed"
        // );
        //
        // ReservationAccomodationExtent.ReservationAccomodations.Add(reservationaccomodation);
        //
        // ReservationAccomodationExtent.Save();
        // ReservationAccomodationExtent.DisplayAll();
    }
    
    
}