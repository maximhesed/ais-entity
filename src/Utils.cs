using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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

        /* It is used for the non-required fields. If a such field is empty 
         * and it has passed to the saving procedure, then this field willn't 
         * be a null, but an empty string, in the database. */
        internal static string Null(string str) {
            return string.IsNullOrWhiteSpace(str) ? "null" : str;
        }

        internal static string Denull(string str) {
            if (new List<object> { null, "", "null" }.Contains(str.ToLower()))
                return "";

            return str + " ";
        }

        static List<PropertyInfo> ProjectLowerOnly(object entity, bool intOverlap = false) {
            List<PropertyInfo> props = entity.GetType().GetProperties().Where(
                f => (char.IsLower(f.Name[0]) || f.Name[0] == '_')
                    && f.Name != "id" && f.Name != "password_hash").ToList();

            if (intOverlap) {
                List<string> intFields = props.Select(p => p.Name).Where(
                    name => name[0] == '_').ToList();

                props = props.Where(p => !intFields.Contains('_' + p.Name)).ToList();
            }

            return props;
        }

        internal static bool DecomposedSearch(object entity, string src, string itemStr) {
            string value = "";
            List<PropertyInfo> proj = ProjectLowerOnly(entity, true);

            foreach (PropertyInfo p in proj) {
                if (p.Name == itemStr || p.Name == '_' + itemStr) {
                    value = p.GetValue(entity) + "";

                    break;
                }
            }

            return value.Contains(src);
        }

        internal static string RowToStr(object entity, bool namesOnly = false, char delim = '\t',
                bool endBreak = true) {
            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo p in ProjectLowerOnly(entity))
                sb.Append((namesOnly ? p.Name : p.GetValue(entity)) + (delim + ""));

            return (sb + "").Trim(delim).Insert((sb + "").Length - 1, endBreak ? "\n" : "");
        }

        internal static void SetComboBoxItem(ComboBox cmb, long id) {
            foreach (string item in cmb.Items) {
                if (item[1] == id)
                    cmb.Text = item;
            }
        }

        internal static string ToUpperFirst(string str) {
            return char.ToUpper(str[0]) + str.ToLower().Substring(1);
        }

        internal static bool CheckEmailRegex(string email) {
            return new Regex(@"^([a-z\d]+)((\.|\-|_)([a-z\d]+))*@([a-z])([a-z\d]*)((\.(\w){2,3})+)$",
                RegexOptions.IgnoreCase).IsMatch(email);
        }

        internal static bool CheckNumeric(TextBox box, string fieldName, long? min = null,
                long? max = null, bool isReq = false) {
            if (!string.IsNullOrEmpty(box.Text)) {
                box.Text = box.Text.Trim();

                if (!box.Text.All(char.IsDigit)) {
                    MessageBox.Show($"The {fieldName} must contain only numbers.", "",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    box.Focus();

                    return false;
                }

                if (min != null) {
                    if (long.Parse(box.Text) < min) {
                        MessageBox.Show($"Minimum {fieldName} value is {min}.", "",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        box.Focus();

                        return false;
                    }
                }

                if (max != null) {
                    if (long.Parse(box.Text) > max) {
                        MessageBox.Show($"Maximum {fieldName} value is {max}.", "",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        box.Focus();

                        return false;
                    }
                }
            }
            else {
                if (isReq) {
                    MessageBox.Show($"The {fieldName} is required.", "",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    box.Focus();

                    return false;
                }
            }

            return true;
        }

        internal static bool CheckName(TextBox box, string fieldName = null, bool isReq = false) {
            if (!string.IsNullOrEmpty(box.Text)) {
                box.Text = box.Text.Trim();

                if (!box.Text.ToLower().All(c => c >= 'а' && c <= 'я')
                        && !box.Text.ToLower().All(c => c >= 'a' && c <= 'z')) {
                    MessageBox.Show($"The {fieldName} must contain only Russian or only English letters.",
                        "", MessageBoxButton.OK, MessageBoxImage.Information);

                    box.Focus();

                    return false;
                }

                box.Text = ToUpperFirst(box.Text);
            }
            else {
                if (isReq) {
                    MessageBox.Show($"The {fieldName} is required.", "", MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    box.Focus();

                    return false;
                }
            }

            return true;
        }

        internal static bool CheckEmail(TextBox box) {
            if (!string.IsNullOrEmpty(box.Text)) {
                box.Text = box.Text.Trim().ToLower();

                if (!CheckEmailRegex(box.Text)) {
                    MessageBox.Show("The email format is invalid.", "", MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    box.Focus();

                    return false;
                }
            }
            else {
                MessageBox.Show("Enter email.", "", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                box.Focus();

                return false;
            }

            return true;
        }

        internal static bool CheckPhone(TextBox box) {
            return CheckNumeric(box, "phone");
        }

        internal static bool CheckEmailOrPhone(TextBox txtEmail, TextBox txtPhone) {
            if (string.IsNullOrEmpty(txtEmail.Text) && string.IsNullOrEmpty(txtPhone.Text)) {
                MessageBox.Show("Enter email or phone.", "", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                txtEmail.Focus();

                return false;
            }

            if (!string.IsNullOrEmpty(txtEmail.Text)) {
                txtEmail.Text = txtEmail.Text.Trim().ToLower();

                if (!CheckEmailRegex(txtEmail.Text)) {
                    MessageBox.Show("The email format is invalid.", "", MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    txtEmail.Focus();

                    return false;
                }
            }

            if (!CheckPhone(txtPhone))
                return false;

            return true;
        }

        internal static bool CheckPrice(TextBox box) {
            if (!string.IsNullOrEmpty(box.Text)) {
                if (!decimal.TryParse(box.Text, out _)) {
                    MessageBox.Show("The price format is invalid.", "", MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    box.Focus();

                    return false;
                }
            }
            else {
                MessageBox.Show("The price is required.", "", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                box.Focus();

                return false;
            }

            return true;
        }
    }
}
