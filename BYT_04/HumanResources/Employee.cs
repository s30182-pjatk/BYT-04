using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using BYT_04.Utility;

namespace BYT_04
{
    [Serializable]
    public class Employee
    {
        // ============================================================
        //                STATIC EXTENT & PERSISTENCE
        // ============================================================

        private static List<Employee> _employees = new();

        private static string _directoryPath =
            Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..",
                "HumanResources", "persistence"
            ));

        private static string FilePath => Path.Combine(_directoryPath, "employees.xml");

        public static IReadOnlyList<Employee> Employees => _employees;

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

            XmlSerializer serializer = new(typeof(List<Employee>));
            using FileStream fs = new(FilePath, FileMode.Create);
            serializer.Serialize(fs, _employees);
        }

        public static void Load()
        {
            if (!File.Exists(FilePath))
                return;

            XmlSerializer serializer = new(typeof(List<Employee>));
            using FileStream fs = new(FilePath, FileMode.Open);

            if (serializer.Deserialize(fs) is List<Employee> loaded)
            {
                _employees.Clear();
                _employees.AddRange(loaded);
            }
        }

        public static void Add(Employee e) => _employees.Add(e);
        public static void Remove(Employee e) => _employees.Remove(e);

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
        //                EMPLOYEE DATA
        // ============================================================

        private DateTime _employmentDate;
        private decimal _salary;

        public DateTime EmploymentDate
        {
            get => _employmentDate;
            set
            {
                if (value > DateTime.Today)
                    throw new ArgumentException("Employment date cannot be in the future.");
                _employmentDate = value;
            }
        }

        public decimal Salary
        {
            get => _salary;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Salary cannot be negative.");
                _salary = value;
            }
        }

        // ============================================================
        //                HIERARCHY (NOT SERIALIZED)
        // ============================================================

        [XmlIgnore]
        public Employee? Manager { get; set; }

        [XmlIgnore]
        public List<Employee> Subordinates { get; private set; } = new();

        // ============================================================
        //                 CONSTRUCTORS
        // ============================================================

        // For XML
        public Employee() { }

        public Employee(
            string name,
            string? middleName,
            string surname,
            DateTime birthDate,
            string gender,
            string phoneNumber,
            string email,
            Address address,
            DateTime employmentDate,
            decimal salary,
            Employee? manager = null
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

            EmploymentDate = employmentDate;
            Salary = salary;

            Manager = manager;
            Subordinates = new List<Employee>();

            _employees.Add(this);
        }

        // ============================================================
        //                 HELPERS
        // ============================================================

        public void AddSubordinate(Employee e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (!Subordinates.Contains(e))
            {
                Subordinates.Add(e);
                e.Manager = this;
            }
        }
    }
}
