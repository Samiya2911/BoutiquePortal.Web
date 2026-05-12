using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace BoutiquePortal.Model.Helpers
{
    public static class PasswordHelper
    {
        // ✅ Hash a plain text password using SHA-256
        public static string Hash(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                return string.Empty;

            using var sha256 = SHA256.Create();

            byte[] bytes = sha256.ComputeHash(
                Encoding.UTF8.GetBytes(plainText));

            // Convert to hex string
            var sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        // ✅ Verify plain text against stored hash
        public static bool Verify(string plainText, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(plainText) ||
                string.IsNullOrWhiteSpace(storedHash))
                return false;

            // ✅ Handle migration: if stored is plain text (64 chars = SHA256)
            // Plain text passwords are shorter than 64 chars in most cases
            if (storedHash.Length != 64)
            {
                // Still plain text — compare directly for backward compat
                return plainText == storedHash;
            }

            // Hash the input and compare
            string hashed = Hash(plainText);
            return hashed == storedHash;
        }

        // ✅ Check if a password is already hashed
        public static bool IsHashed(string password)
        {
            // SHA-256 hex is always exactly 64 characters
            return !string.IsNullOrEmpty(password)
                && password.Length == 64
                && password.All(c => "0123456789abcdef".Contains(c));
        }
    }
}
