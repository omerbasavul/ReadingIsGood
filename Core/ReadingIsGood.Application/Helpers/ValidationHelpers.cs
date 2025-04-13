using System.Text.RegularExpressions;

namespace ReadingIsGood.Application.Helpers;

public static partial class ValidationHelpers
{
    public static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex().IsMatch(email);
    }

    public static bool IsValidPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && PasswordRegex().IsMatch(password);
    }

    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        return !string.IsNullOrWhiteSpace(phoneNumber) && PhoneNumberRegex().IsMatch(phoneNumber);
    }

    public static bool IsValidCountryCode(string countryCode)
    {
        return !string.IsNullOrWhiteSpace(countryCode) && CountryCodeRegex().IsMatch(countryCode);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", RegexOptions.Compiled)]
    private static partial Regex PasswordRegex();

    [GeneratedRegex(@"^5\d{9}$", RegexOptions.Compiled)]
    private static partial Regex PhoneNumberRegex();


    [GeneratedRegex(@"^\+\d{1,3}$", RegexOptions.Compiled)]
    private static partial Regex CountryCodeRegex();
}