using System;
using System.Text;
using System.Security.Cryptography;

namespace PlayerIOConnect
{
    public class PlayerIOAuth
    {
        public static string Create(string userId, string sharedSecret, string time)
        {
            int unixTime = (int)(DateTime.Parse(time) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(sharedSecret)).ComputeHash(Encoding.UTF8.GetBytes(unixTime + ":" + userId));
            return unixTime + ":" + toHexString(hmac);
        }

        private static string toHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}