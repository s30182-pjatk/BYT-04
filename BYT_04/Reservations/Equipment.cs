using System.Xml.Serialization;

namespace BYT_04.Reservations
{
    [Serializable]
    public class Equipment
    {
        private string _name = null!;
        private DateTime _lastMaintenanceDate;

        public string Name
        {
            get => _name;
            set => _name = ValidateRequiredString(value, nameof(Name));
        }

        public DateTime LastMaintenanceDate
        {
            get => _lastMaintenanceDate;
            set
            {
                if (value > DateTime.Today)
                    throw new ArgumentException("Last maintenance date cannot be in the future.");
                _lastMaintenanceDate = value;
            }
        }

        public Equipment() { }

        public Equipment(string name, DateTime lastMaintenanceDate)
        {
            Name = name;
            LastMaintenanceDate = lastMaintenanceDate;
        }

        private static string ValidateRequiredString(string value, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{propertyName} cannot be null, empty, or whitespace.");

            return value;
        }

        public override string ToString()
        {
            return $"Name: {Name}\n" +
                   $"Last maintenance: {LastMaintenanceDate.ToShortDateString()}\n";
        }
    }

    public static class EquipmentExtent
    {
        public static List<Equipment> Equipments { get; private set; } = new();

        private static string _directoryPath =
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reservations", "persistence"));

        private static string FilePath => Path.Combine(_directoryPath, "equipment.xml");

        public static void SetDirectory(string newDirectory)
        {
            if (string.IsNullOrWhiteSpace(newDirectory))
                throw new ArgumentException("Directory cannot be null or empty.");

            _directoryPath = newDirectory;
        }

        public static void Save()
        {
            Console.WriteLine("Saving to: " + FilePath);

            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            XmlSerializer serializer = new(typeof(List<Equipment>));

            using FileStream fs = new(FilePath, FileMode.Create);
            serializer.Serialize(fs, Equipments);
        }

        public static void Load()
        {
            Console.WriteLine("Loading from: " + FilePath);

            if (!File.Exists(FilePath))
                return;

            XmlSerializer serializer = new(typeof(List<Equipment>));

            using FileStream fs = new(FilePath, FileMode.Open);

            if (serializer.Deserialize(fs) is List<Equipment> loaded)
                Equipments = loaded;
        }

        public static void DisplayAll()
        {
            if (Equipments.Count == 0)
            {
                Console.WriteLine("No equipment found.");
                return;
            }

            Console.WriteLine("\n--- Loaded Equipment ---\n");

            foreach (var e in Equipments)
            {
                Console.WriteLine(e);
            }
        }
    }
}
