using System;
using BYT_04.Utility;
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
            set => _street = value.ValidateRequiredString(nameof(Street));
        }

        public string City
        {
            get => _city;
            set => _city = value.ValidateRequiredString(nameof(City));
        }

        public string State
        {
            get => _state;
            set => _state = value.ValidateRequiredString(nameof(State));
        }

        public string PostalCode
        {
            get => _postalCode;
            set => _postalCode = value.ValidateRequiredString(nameof(PostalCode));
        }

        public string Country
        {
            get => _country;
            set => _country = value.ValidateRequiredString(nameof(Country));
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

    }
}