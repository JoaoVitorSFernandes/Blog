namespace Blog;

public static class Configuration
{
    public static string JwtKey { get; set; } = "xQ58hwZqpkm7V3yQbG0eqwi6WD52uKEEq7Np9K3azTVg==";
    public static string ApiKeyName { get; set; }
    public static string ApiKey { get; set; }
    public static StmpConfiguration Smtp = new();

    public class StmpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }
}
