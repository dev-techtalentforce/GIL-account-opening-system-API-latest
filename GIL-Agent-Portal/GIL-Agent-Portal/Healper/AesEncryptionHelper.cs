using System.Security.Cryptography;

namespace GIL_Agent_Portal.E
{
    public class AesEncryptionHelper
    {
        private static string key = "yYxeaKPojTwCJdFM";
        private static string initVector = "yYxeaKPojTwCJdFM";
        private static AesManaged CreateAes()
        {
            var aes = new AesManaged();
            aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
            aes.IV = System.Text.Encoding.UTF8.GetBytes(initVector);
            return aes;
        }
        public static string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            using (AesManaged aes = CreateAes())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(text);
                    sw.Flush();
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }

    public static class SignCsHelper
    {
        public static string GenerateSignCs(string data, string secretKey)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(System.Text.Encoding.UTF8.GetBytes(secretKey)))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
