using GIL_Agent_Portal.Services;
using System;
using System.Security.Cryptography;
using System.Text;
namespace GIL_Agent_Portal.Utlity
{
    public class NsdlSignCsHelper
    {
        public static string GenerateSignCs(string key, string checksum)
        {
            try
            {
                var secretKeyBytes = Encoding.UTF8.GetBytes(key);
                var checksumBytes = Encoding.UTF8.GetBytes(checksum);

                using var hmac = new HMACSHA512(secretKeyBytes);
                var hashBytes = hmac.ComputeHash(checksumBytes);
                return Convert.ToBase64String(hashBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignCS Generation Error: {ex.Message}");
                return null!;
            }
        }

        public static string ExtractKey(string tokenKey)
        {
            if (tokenKey.Length != 256) throw new ArgumentException("Invalid tokenKey length.");
            return tokenKey.Substring(128); // Second half is key
        }

        public static string ExtractToken(string tokenKey)
        {
            if (tokenKey.Length != 256) throw new ArgumentException("Invalid tokenKey length.");
            return tokenKey.Substring(0, 128); // First half is token
        }

        public async Task<string> RunTestAsync()
        {
            try
            {
                var tokenService = new SessionTokenService();
                var result = await tokenService.FetchNsdlSessionTokenAsync();

                if (result?.Sessiontokendtls == null)
                {
                    Console.WriteLine("❌ Failed to fetch session token.");
                    return null;
                }

                string tokenKey = result.Sessiontokendtls.TokenKey;

                string token = NsdlSignCsHelper.ExtractToken(tokenKey);
                string key = NsdlSignCsHelper.ExtractKey(tokenKey);

                Console.WriteLine("🔐 TOKEN SPLIT:");
                Console.WriteLine($"Token: {token}");
                Console.WriteLine($"Key:   {key}");

                // Dummy checksum for testing – replace with actual checksum logic per API
                string checksum = $"channelid=lfbpWjegXHwnnirQOlYP|partnerid=wpemmjhKus|appid=com.jarviswebbc.nsdlpb|timestamp={DateTime.UtcNow:yyyyMMddHHmmss}";

                string signcs = NsdlSignCsHelper.GenerateSignCs(key, checksum);

                Console.WriteLine("\n✅ TEST OUTPUT:");
                Console.WriteLine($"Checksum String: {checksum}");
                Console.WriteLine($"SignCS: {signcs}");

                return signcs;
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"❗ Null reference error: {ex.Message}");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"❗ Format error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Unexpected error: {ex.Message}");
            }

            return null;
        }
    }
}
