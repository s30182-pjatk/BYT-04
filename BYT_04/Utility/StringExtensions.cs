using System;
namespace BYT_04.Utility
{
    public static class StringExtensions
    {
        public static string ValidateRequiredString(this string value, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{propertyName} cannot be null, empty, or whitespace.");

            return value;
        }
    }
    
}

