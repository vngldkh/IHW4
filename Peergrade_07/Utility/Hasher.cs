using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using IHW4.Models;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace IHW4
{
    public static class Hasher
    {
        private static SHA256 sha256Hash = SHA256.Create();

        public static String HashPassword(String password)
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            return hash;
        }

        public static bool CheckPassword(String password, String hashedPassword)
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            return hashedPassword.Equals(hash);
        }
    }
}
