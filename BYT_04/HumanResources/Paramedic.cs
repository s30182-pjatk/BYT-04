using BYT_04.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using BYT_04.Utility;

namespace BYT_04
{
    [Serializable]
    public class Paramedic
    {
        // ============================================================
        //                STATIC PERSISTENCE MEMBERS
        // ============================================================

        private static List<Paramedic> _paramedics = new();

        private static string _directoryPath =
            Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..",
                "HumanResources", "persistence"
            ));

        private static string FilePath => Path.Combine(_directoryPath, "paramedics.xml");

        public static IReadOnlyList<Paramedic> Paramedics => _paramedics;

        // -- Set custom directory ------------------------------------
        public static void SetDirectory(string newDirectory)
        {
            if (string.IsNullOrWhiteSpace(newDirectory))
                throw new ArgumentException("Directory path cannot be null or empty.");

            _directoryPath = newDirectory;
        }

        // -- Add / Remove --------------------------------------------
        public static void Add(Paramedic p) => _paramedics.Add(p);
        public static void Remove(Paramedic p) => _paramedics.Remove(p);

        // -- Save -----------------------------------------------------
        public static void Save()
        {
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            XmlSerializer serializer = new(typeof(List<Paramedic>));
            using FileStream fs = new(FilePath, FileMode.Create);
            serializer.Serialize(fs, _paramedics);
        }

        // -- Load -----------------------------------------------------
        public static void Load()
        {
            if (!File.Exists(FilePath))
                return;

            XmlSerializer serializer = new(typeof(List<Paramedic>));
            using FileStream fs = new(FilePath, FileMode.Open);

            if (serializer.Deserialize(fs) is List<Paramedic> loaded)
            {
                _paramedics.Clear();
                _paramedics.AddRange(loaded);
            }
        }

        // -- Display --------------------------------------------------
        public static void DisplayAll()
        {
            if (_paramedics.Count == 0)
            {
                Console.WriteLine("No paramedics found.");
                return;
            }

            Console.WriteLine("\n--- Loaded Paramedics ---\n");

            foreach (var p in _paramedics)
            {
                Console.WriteLine(
                    $"Name: {p.Name} {p.MiddleName} {p.Surname}\n" +
                    $"Birth Date: {p.BirthDate.ToShortDateString()}\n" +
                    $"Gender: {p.Gender}\n" +
                    $"Phone: {p.PhoneNumber}\n" +
                    $"Email: {p.Email}\n" +
                    $"Address: {p.Address.Street}, {p.Address.City}, {p.Address.State}, {p.Address.PostalCode}, {p.Address.Country}\n" +
                    $"CPR Certification Number: {p.CPRCertificationNumber}\n" +
                    "-----------------------------\n"
                );
            }
        }

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
        private HashSet<Vehicle> _vehicles = new();

        [XmlIgnore]
        public IEnumerable<Vehicle> Vehicles => _vehicles.ToList();

        public void AddVehicle(Vehicle v)
        {
            if (v == null)
                return;

            if (_vehicles.Add(v))
            {
                if (!v.Paramedics.Contains(this))
                    v.AddParamedic(this);
            }
        }

        public void RemoveVehicle(Vehicle v)
        {
            if (v == null)
                return;

            if (_vehicles.Remove(v))
            {
                if (v.Paramedics.Contains(this))
                    v.RemoveParamedic(this);
            }
        }

        // ============================================================
        //                  PARAMEDIC DATA
        // ============================================================

        private string _cprCertificationNumber = null!;

        public string CPRCertificationNumber
        {
            get => _cprCertificationNumber;
            set => _cprCertificationNumber =
                value.ValidateRequiredString(nameof(CPRCertificationNumber));
        }

        // ============================================================
        //                  CONSTRUCTORS
        // ============================================================

        // For XML
        public Paramedic() { }

        public Paramedic(
            string name,
            string? middleName,
            string surname,
            DateTime birthDate,
            string gender,
            string phoneNumber,
            string email,
            Address address,
            string cprCertificationNumber
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

            CPRCertificationNumber = cprCertificationNumber;

            _paramedics.Add(this);
        }

        // ============================================================
        //                  METHODS
        // ============================================================

        public bool IsCertified() =>
            !string.IsNullOrWhiteSpace(CPRCertificationNumber);
    }
}
