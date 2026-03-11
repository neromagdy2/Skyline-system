using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.CustomValidations
{

    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // 1. Get the property to compare against
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                return new ValidationResult($"Unknown property: {_comparisonProperty}");

            // 2. Get the values of both properties
            var comparisonValue = property.GetValue(validationContext.ObjectInstance);

            // Ensure both are DateTime objects
            if (value is DateTime arrivalDate && comparisonValue is DateTime departureDate)
            {
                // 3. Perform the comparison logic
                if (arrivalDate <= departureDate)
                {
                    return new ValidationResult(ErrorMessage ?? $"Arrival date must be later than {_comparisonProperty}.");
                }
            }

            return ValidationResult.Success;
        }
    }

}