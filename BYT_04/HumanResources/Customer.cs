using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using BYT_04.Reservations;

namespace BYT_04
{
    [Serializable]
    public class Customer
    {
        // ============================================================
        //                STATIC EXTENT & PERSISTENCE
        // ============================================================

        private static List<Customer> _customers = new();

        private static string _directoryPath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "HumanResources",
                "persistence"
            )
        );

        private static string FilePath => Path.Combine(_directoryPath, "customers.xml");

        public static IReadOnlyList<Customer> Customers => _customers;

        public static void SetDirectory(string newDirectory)
        {
            if (string.IsNullOrWhiteSpace(newDirectory))
                throw new ArgumentException("Directory path cannot be null or empty.");

            _directoryPath = newDirectory;
        }

        public static void Save()
        {
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            XmlSerializer serializer = new(typeof(List<Customer>));
            using FileStream fs = new(FilePath, FileMode.Create);
            serializer.Serialize(fs, _customers);
        }

        public static void Load()
        {
            if (!File.Exists(FilePath))
                return;

            XmlSerializer serializer = new(typeof(List<Customer>));
            using FileStream fs = new(FilePath, FileMode.Open);

            if (serializer.Deserialize(fs) is List<Customer> loaded)
                _customers = loaded;
        }

        public static void DisplayAll()
        {
            if (_customers.Count == 0)
            {
                Console.WriteLine("No customers found.");
                return;
            }

            Console.WriteLine("\n--- Loaded Customers ---\n");

            foreach (var c in _customers)
            {
                Console.WriteLine(
                    $"Name: {c.Name} {c.MiddleName} {c.Surname}\n" +
                    $"Birth Date: {c.BirthDate.ToShortDateString()}\n" +
                    $"Gender: {c.Gender}\n" +
                    $"Phone: {c.PhoneNumber}\n" +
                    $"Email: {c.Email}\n" +
                    $"VIP: {(c.IsVip ? "Yes" : "No")}\n" +
                    $"Loyalty Points: {c.LoyaltyPoints}\n" +
                    $"Address: {c.Address.Street}, {c.Address.City}, {c.Address.State}, " +
                    $"{c.Address.PostalCode}, {c.Address.Country}\n" +
                    "-----------------------------\n"
                );
            }
        }

        public static void Add(Customer c) => _customers.Add(c);

        public static void Remove(Customer c)
        {
            if (c == null)
                return;

            foreach (var r in c._reservations.ToList())
            {
                Reservation.InternalRemove(r);
            }

            c._reservations.Clear();
            _customers.Remove(c);
        }

        // ============================================================
        //                  PERSON (COMPOSITION)
        // ============================================================

        [XmlElement("Person")]
        private Person _person = null!;

        // Expose Person data via delegation
        [XmlIgnore] public string Name => _person.Name;
        [XmlIgnore] public string? MiddleName => _person.MiddleName;
        [XmlIgnore] public string Surname => _person.Surname;
        [XmlIgnore] public DateTime BirthDate => _person.BirthDate;
        [XmlIgnore] public string Gender => _person.Gender;
        [XmlIgnore] public string PhoneNumber => _person.PhoneNumber;
        [XmlIgnore] public string Email => _person.Email;
        [XmlIgnore] public Address Address => _person.Address;

        // Optional access if needed elsewhere
        public Person Person => _person;

        // ============================================================
        //                  RESERVATIONS
        // ============================================================

        [XmlIgnore]
        private HashSet<Reservation> _reservations = new();

        [XmlIgnore]
        public IEnumerable<Reservation> Reservations =>
            _reservations
                .OrderBy(m => -(m.StartDate - DateTime.Now))
                .ThenByDescending(m => m.StartDate)
                .ToList();

        public Reservation CreateReservation(
            int reservationId,
            DateTime startDate,
            DateTime endDate,
            ReservationStatus status,
            decimal totalPrice
        )
        {
            Reservation newReservation = new Reservation(
                this,
                reservationId,
                startDate,
                endDate,
                status,
                totalPrice
            );

            _reservations.Add(newReservation);
            return newReservation;
        }

        public void AddReservation(Reservation reservation)
        {
            if (reservation == null)
                return;

            if (reservation.Customer != null && reservation.Customer != this)
                throw new InvalidOperationException(
                    "Reservation is already associated with another customer."
                );

            if (reservation.Customer == null)
                reservation.SetCustomerInternal(this);

            _reservations.Add(reservation);
        }

        public bool RemoveReservation(int reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            if (reservation == null)
                return false;

            _reservations.Remove(reservation);
            return true;
        }

        // ============================================================
        //                  CUSTOMER DATA
        // ============================================================

        private bool _isVip;
        private int _loyaltyPoints;

        public bool IsVip
        {
            get => _isVip;
            set => _isVip = value;
        }

        public int LoyaltyPoints
        {
            get => _loyaltyPoints;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Loyalty points cannot be negative.");
                _loyaltyPoints = value;
            }
        }

        // ============================================================
        //                  CONSTRUCTORS
        // ============================================================

        // For XML
        public Customer() { }

        public Customer(
            string name,
            string? middleName,
            string surname,
            DateTime birthDate,
            string gender,
            string phoneNumber,
            string email,
            Address address,
            bool isVip,
            int loyaltyPoints
        )
        {
            _person = new Person(
                name,
                middleName,
                surname,
                birthDate,
                gender,
                phoneNumber,
                email,
                address
            );

            IsVip = isVip;
            LoyaltyPoints = loyaltyPoints;

            _customers.Add(this);
        }

        // ============================================================
        //                  METHODS
        // ============================================================

        public int CheckLoyaltyPoints() => LoyaltyPoints;

        public void MakeVip() => IsVip = true;
    }
}
