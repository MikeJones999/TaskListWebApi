using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TaskList.Api.Domain.Users.Validation
{
    public class StrictEmailAddressAttribute : ValidationAttribute
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Email address is required.");
            }

            string email = value.ToString()!;

            if (!EmailRegex.IsMatch(email))
            {
                return new ValidationResult("Email address format is invalid.");
            }

            var parts = email.Split('@');
            if (parts.Length != 2)
            {
                return new ValidationResult("Email address must contain exactly one @ symbol.");
            }

            var domain = parts[1];
            var domainParts = domain.Split('.');
            
            if (domainParts.Length < 2)
            {
                return new ValidationResult("Email address must have a valid domain with a top-level domain (e.g., .com, .org).");
            }

            var topLevelDomain = domainParts[^1];
            if (topLevelDomain.Length < 2)
            {
                return new ValidationResult("Email address must have a valid top-level domain (e.g., .com, .org, .co.uk).");
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field must be a valid email address with a proper domain (e.g., user@example.com).";
        }
    }
}
