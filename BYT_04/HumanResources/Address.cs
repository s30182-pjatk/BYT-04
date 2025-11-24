using System;

namespace BYT_04
{
    [Serializable]
    public class Address
    {
        private string _street = null!;
        private string _city = null!;
        private string _state = null!;
        private string _postalCode = null!;
        private string _country = null!;

        public string Street
        {
            get => _street;
            set => _street = ValidateRequiredString(value, nameof(Street));
        }

        public string City
        {
            get => _city;
            set => _city = ValidateRequiredString(value, nameof(City));
        }

        public string State
        {
            get => _state;
            set => _state = ValidateRequiredString(value, nameof(State));
        }

        public string PostalCode
        {
            get => _postalCode;
            set => _postalCode = ValidateRequiredString(value, nameof(PostalCode));
        }

        public string Country
        {
            get => _country;
            set => _country = ValidateRequiredString(value, nameof(Country));
        }

        public Address() { }

        public Address(string street, string city, string state, string postalCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
        }

        private static string ValidateRequiredString(string value, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{propertyName} cannot be null, empty, or whitespace.");
            return value;
        }
    }
}