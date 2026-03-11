using System.ComponentModel.DataAnnotations;

namespace Airport_Managment_SYS.CustomValidations
{
    public class NotEqualAttribute : ValidationAttribute
    {
        private readonly string _compare;
        public NotEqualAttribute(string Compare) { 
            _compare = Compare;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_compare);

            if (property == null)
                return new ValidationResult($"Unknown property: {_compare}");

            var comparisonValue = property.GetValue(validationContext.ObjectInstance);

            if (Object.Equals(value,comparisonValue))
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} date must not equal{_compare}.");
            }


            return ValidationResult.Success;
        }
    }
}
