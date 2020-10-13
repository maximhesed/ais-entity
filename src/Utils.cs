using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Ais.src
{
    static class Utils
    {
        internal static string Hash(string passw) {
            byte[] hashBytes = new byte[36];
            byte[] salt = new byte[16];

            using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider()) {
                cryptoServiceProvider.GetBytes(salt);

                using (Rfc2898DeriveBytes bytesDeriver = new Rfc2898DeriveBytes(passw, salt, 40000)) {
                    byte[] hash = bytesDeriver.GetBytes(20);

                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 20);
                }
            }

            return Convert.ToBase64String(hashBytes);
        }

        internal static bool CompHash(string passw, string savedPassw) {
            byte[] hashBytes = Convert.FromBase64String(savedPassw);
            byte[] salt = new byte[16];

            Array.Copy(hashBytes, 0, salt, 0, 16);

            using (Rfc2898DeriveBytes bytesDeriver = new Rfc2898DeriveBytes(passw, salt, 40000)) {
                byte[] hash = bytesDeriver.GetBytes(20);

                for (int i = 0; i < 20; i++) {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
            }

            return true;
        }

        internal static string Denull(string str) {
            if (str == "null")
                return "";

            return str + " ";
        }

        internal static bool DecomposedSearch<T>(T item, string src, int selIdx) {
            string value = "";
            List<PropertyInfo> proj = ProjectLowerOnly(item);

            for (int i = 0; i < proj.Count; i++) {
                if (i == selIdx) {
                    value = proj[i].GetValue(item) + "";

                    break;
                }
            }

            return value.Contains(src);
        }

        internal static string RowToStr<T>(T item, bool namesOnly = false, char delim = '\t',
                bool endBreak = true) {
            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo p in ProjectLowerOnly(item))
                sb.Append((namesOnly ? p.Name : p.GetValue(item)) + (delim + ""));

            return (sb + "").Trim(delim).Insert((sb + "").Length - 1, endBreak ? "\n" : "");
        }

        static List<PropertyInfo> ProjectLowerOnly<T>(T item) {
            return item.GetType().GetProperties().Where(
                f => char.IsLower(f.Name[0]) &&
                /* Workaround. */
                f.Name != "password_hash").ToList();
        }
    }
}
