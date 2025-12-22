using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using BYT_04.Utility;
using BYT_04.Vehicles;

namespace BYT_04
{
    [Serializable]
    public class Driver
    {
        // ============================================================
        //                STATIC EXTENT & PERSISTENCE
        // ============================================================

        private static List<Driver> _drivers = new();

        private static string _directoryPath =
            Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..",
                "HumanResources", "persistence"
            ));

        private static string FilePath => Path.Combine(_directoryPath, "drivers.xml");

        public static IReadOnlyList<Driver> Drivers => _drivers;

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

            XmlSerializer serializer = new(typeof(List<Driver>));
            using FileStream fs = new(FilePath, FileMode.Create);
            serializer.Serialize(fs, _drivers);
        }

        public static void Load()
        {
            if (!File.Exists(FilePath))
                return;

            XmlSerializer serializer = new(typeof(List<Driver>));
            using FileStream fs = new(FilePath, FileMode.Open);

            if (serializer.Deserialize(fs) is List<Driver> loaded)
            {
                _drivers.Clear();
                _drivers.AddRange(loaded);
            }
        }

        public static void Add(Driver d) => _drivers.Add(d);
        public static void Remove(Driver d) => _drivers.Remove(d);

        // ============================================================
        //                  PERSON (COMPOSITION)
        // ============================================================

        private Person _person = null!;

        [XmlElement("Person")]
        public Person Person
        {
            get => _person;
            set => _person = value ?? throw new ArgumentNullException(nameof(Person));
        }

        // Delegated Person properties
        [XmlIgnore] public string Name => _person.Name;
        [XmlIgnore] public string? MiddleName => _person.MiddleName;
        [XmlIgnore] public string Surname => _person.Surname;
        [XmlIgnore] public DateTime BirthDate => _person.BirthDate;
        [XmlIgnore] public string Gender => _person.Gender;
        [XmlIgnore] public string PhoneNumber => _person.PhoneNumber;
        [XmlIgnore] public string Email => _person.Email;
        [XmlIgnore] public Address Address => _person.Address;

        // ============================================================
        //                  VEHICLE ASSOCIATION
        // ============================================================

        [XmlIgnore]
        private HashSet<Vehicle> _assignedVehicles = new();

        [XmlIgnore]
        public IEnumerable<Vehicle> AssignedVehicles => _assignedVehicles.ToList();

        public void AddAssignedVehicle(Vehicle v)
        {
            if (v == null)
                return;

            if (_assignedVehicles.Add(v))
            {
                if (v.AssignedDriver != this)
                    v.AssignedDriver = this;
            }
        }

        public void RemoveAssignedVehicle(Vehicle v)
        {
            if (v == null)
                return;

            if (_assignedVehicles.Remove(v))
            {
                if (v.AssignedDriver == this)
                    v.AssignedDriver = null;
            }
        }

        // ============================================================
        //                  DRIVER DATA
        // ============================================================

        private string _licenseNumber = null!;
        private DateTime _licenseExpiry;

        public string LicenseNumber
        {
            get => _licenseNumber;
            set => _licenseNumber = value.ValidateRequiredString(nameof(LicenseNumber));
        }

        public DateTime LicenseExpiry
        {
            get => _licenseExpiry;
            set
            {
                if (value < DateTime.Today)
                    throw new ArgumentException("License expiry date cannot be in the past.");
                _licenseExpiry = value;
            }
        }

        // ============================================================
        //                  CONSTRUCTORS
        // ============================================================

        // Parameterless constructor for XML
        public Driver() { }

        public Driver(
            string name,
            string? middleName,
            string surname,
            DateTime birthDate,
            string gender,
            string phoneNumber,
            string email,
            Address address,
            string licenseNumber,
            DateTime licenseExpiry
        )
        {
            Person = new Person(
                name,
                middleName,
                surname,
                birthDate,
                gender,
                phoneNumber,
                email,
                address
            );

            LicenseNumber = licenseNumber;
            LicenseExpiry = licenseExpiry;

            _drivers.Add(this);
        }

        // ============================================================
        //                  METHODS
        // ============================================================

        public bool IsLicenseValid() => LicenseExpiry >= DateTime.Today;
    }
}
