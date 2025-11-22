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
        ReservationExtent.Load();
        
        var reservation = new Reservation(
            1,
            new DateTime(2025, 11,22 ),
            new DateTime(2025, 12,2 ),
            ReservationStatus.Pending,
            105
        );
        
        ReservationExtent.Reservations.Add(reservation);
        
        //Save to XML
        ReservationExtent.Save();
        
        //Display loaded reservations
        ReservationExtent.DisplayAll();
        
        //-------------------------------------------------------------------------------------------------------
        
        //ACCOMODATIONS
        AccomodationExtent.Load();

        var accomodation = new Accomodation(
            "A160",
            AccomodationType.Room,
            7);
        
        AccomodationExtent.Accomodations.Add(accomodation);
        
        //Save to XML
        AccomodationExtent.Save();
        
        //Display loaded Accomodations
        AccomodationExtent.DisplayAll();
        
        
        //-------------------------------------------------------------------------------------------------------------
    
        //ReservationAccomodation
        ReservationAccomodationExtent.Load();

        var reservationaccomodation = new ReservationAccomodation(
            reservation,
            accomodation,
            5,
            new DateTime(2025, 11, 22),
            new DateTime(2025, 12, 2),
            "Good",
            "Poor",
            "Heater needs to be fixed"
        );
        
        ReservationAccomodationExtent.ReservationAccomodations.Add(reservationaccomodation);
        
        ReservationAccomodationExtent.Save();
        ReservationAccomodationExtent.DisplayAll();
    }
    
    
}