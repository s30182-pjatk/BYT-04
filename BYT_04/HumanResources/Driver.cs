using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using BYT_04.Utility;
using BYT_04.Vehicles;

namespace BYT_04
{
    [Serializable]
    public class Driver : Person
    {
        // ============================================================
        //                STATIC EXTENT & PERSISTENCE
        // ============================================================

        private static List<Driver> _drivers = new();
        
        [XmlIgnore]
        private HashSet<Vehicle> _assignedVehicles = new();

        [XmlIgnore]
        public IEnumerable<Vehicle> AssignedVehicles => _assignedVehicles.ToList();

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
                _drivers = loaded;
        }


        public static void DisplayAll()
        {
            if (_drivers.Count == 0)
            {
                Console.WriteLine("No drivers found.");
                return;
            }

            Console.WriteLine("\n--- Loaded Drivers ---\n");

            foreach (var d in _drivers)
            {
                Console.WriteLine(
                    $"Name: {d.Name} {d.MiddleName} {d.Surname}\n" +
                    $"Birth Date: {d.BirthDate.ToShortDateString()}\n" +
                    $"Gender: {d.Gender}\n" +
                    $"Phone: {d.PhoneNumber}\n" +
                    $"Email: {d.Email}\n" +
                    $"Address: {d.Address.Street}, {d.Address.City}, {d.Address.State}, {d.Address.PostalCode}, {d.Address.Country}\n" +
                    $"License Number: {d.LicenseNumber}\n" +
                    $"License Expiry: {d.LicenseExpiry.ToShortDateString()}\n" +
                    $"License Valid: {(d.IsLicenseValid() ? "Yes" : "No")}\n" +
                    "-----------------------------\n"
                );
            }
        }


        public static void Add(Driver d) => _drivers.Add(d);
        public static void Remove(Driver d) => _drivers.Remove(d);



        // ============================================================
        //                  INSTANCE DATA
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

        // Parameterless ctor for XML – must NOT add to extent
        public Driver() : base() { }

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
        ) : base(name, middleName, surname, birthDate, gender, phoneNumber, email, address)
        {
            LicenseNumber = licenseNumber;
            LicenseExpiry = licenseExpiry;

            // Auto-register into static extent
            _drivers.Add(this);
        }


        // ============================================================
        //                  METHODS
        // ============================================================

        public bool IsLicenseValid() => LicenseExpiry >= DateTime.Today;
        
        public void AddAssignedVehicle(Vehicle v)
        {
            if (v == null) return;

            // Prevent infinite recursion and duplicates
            if (!_assignedVehicles.Contains(v))
            {
                _assignedVehicles.Add(v);

                // Reverse Connection
                if (v.AssignedDriver != this)
                {
                    v.AssignedDriver = this;
                }
            }
        }

        public void RemoveAssignedVehicle(Vehicle v)
        {
            if (v != null && _assignedVehicles.Contains(v))
            {
                _assignedVehicles.Remove(v);
                
                if (v.AssignedDriver == this)
                {
                    v.AssignedDriver = null; 
                }
            }
        }
    }
}
