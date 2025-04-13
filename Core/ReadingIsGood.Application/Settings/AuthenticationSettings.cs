namespace ReadingIsGood.Application.Settings;

public static class AuthenticationSettings
{
    public static List<string> ValidAudiences { get; set; }

    public static string ValidIssuer { get; set; }

    public static int TokenExpirationHour { get; set; }

    public static string Secret { get; set; }
}
