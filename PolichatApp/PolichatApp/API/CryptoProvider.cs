using System;
using System.Security.Cryptography;
using System.Text;

namespace PolichatApp.API
{
    public static class CryptoProvider
    {
        public static string GetMD5Hash(string p)
        {
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] hashCode = md5Provider.ComputeHash(Encoding.Default.GetBytes(p));
            return BitConverter.ToString(hashCode).ToLower().Replace("-", "");
        }
    }
}
