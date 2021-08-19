using System;
using System.Text;

namespace SUPJenCLI
{
    internal static class LoginSettings
    {
        static LoginSettings()
        {
            URL = Environment.GetEnvironmentVariable("SUPJENCLI_URL");
            Username = Environment.GetEnvironmentVariable("SUPJENCLI_USERNAME");
            Token = Environment.GetEnvironmentVariable("SUPJENCLI_TOKEN");
        }

        public static string URL { get; }

        public static string Username { get; }

        public static string Token { get; }

        public static string GetBasicAuthorizationEncoded() =>
            Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{Username}:{Token}"));
    }
}
