using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ais.model;

namespace Ais.src
{
    public delegate void CaptchaResultTransfer(bool result);

    public partial class winAuth : Window
    {
        string password = "";
        int selLen = 0;
        int fails = 0;
        readonly CaptchaResultTransfer transfer;
        readonly string color = "#ffa3a3a3";

        public winAuth() {
            InitializeComponent();

            this.transfer += new CaptchaResultTransfer(ReceiveCaptchaResult);
        }

        void Window_Loaded(object sender, RoutedEventArgs e) {
            this.txtLogin.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.color));
            this.txtPassw.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.color));

            this.txtLogin.GotFocus += new RoutedEventHandler(SetActive);
            this.txtPassw.GotFocus += new RoutedEventHandler(SetActive);
        }

        void btnLogin_Click(object sender, RoutedEventArgs e) {
            Employees empl;
            string login = this.txtLogin.Text;
            string passw = this.password;

            if (string.IsNullOrEmpty(login) || login == "login") {
                MessageBox.Show("Login entered is empty.", "Auth");

                return;
            }

            if (string.IsNullOrEmpty(passw) || passw == "password") {
                MessageBox.Show("Password entered is empty.", "Auth");

                return;
            }

            if (this.fails == 3) {
                new winCaptcha(this.transfer).ShowDialog();

                return;
            }

            empl = Context.ctx.Employees.Where(i => i.email == login).FirstOrDefault();
            if (empl == null) {
                MessageBox.Show("No such user exists.", "Auth");

                this.fails++;

                return;
            }

            if (!Utils.CompHash(passw, empl.password_hash)) {
                MessageBox.Show("Incorrect password.", "Auth");

                this.fails++;

                return;
            }

            MessageBox.Show("You are successfully logged in.", "Auth", MessageBoxButton.OK,
                MessageBoxImage.Information);

            this.txtLogin.GotFocus -= new RoutedEventHandler(SetActive);
            this.txtPassw.GotFocus -= new RoutedEventHandler(SetActive);
            this.txtPassw.TextChanged -= new TextChangedEventHandler(RedirectPassword);

            this.Visibility = Visibility.Collapsed;

            this.txtLogin.Text = "login";
            this.txtPassw.Text = "password";
            this.password = "";

            this.txtLogin.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.color));
            this.txtPassw.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.color));

            this.txtLogin.GotFocus += new RoutedEventHandler(SetActive);
            this.txtPassw.GotFocus += new RoutedEventHandler(SetActive);
            this.txtPassw.TextChanged += new TextChangedEventHandler(RedirectPassword);

            this.fails = 0;

            new winMain(empl.id).ShowDialog();

            this.Visibility = Visibility.Visible;
        }

        void SetActive(object sender, RoutedEventArgs args) {
            TextBox box = (TextBox) sender;

            if (box.Text == "password")
                /* Replace the password text box chars to the password chars, 
                 * that don't use the marginal PasswordBox. */
                this.txtPassw.TextChanged += new TextChangedEventHandler(RedirectPassword);

            box.Foreground = Brushes.Black;
            box.Text = "";

            box.GotFocus -= new RoutedEventHandler(SetActive);
        }

        void RedirectPassword(object sender, TextChangedEventArgs e) {
            TextBox box = (TextBox) sender;
            string buf = "";
            int selStart = box.SelectionStart;

            if (box.Text.All(c => c == '•')) {
                if (box.Text.Length < this.password.Length) {
                    this.password = this.password.Remove(selStart, this.selLen != 0
                        ? this.selLen : 1);

                    this.selLen = 0;
                }

                return;
            }

            if (box.Text.Length == 0)
                this.password = "";
            else {
                if (selStart > 0) {
                    if (this.selLen > 0)
                        this.password = this.password.Remove(selStart - 1, this.selLen);

                    this.password = this.password.Insert(selStart - 1, box.Text.Trim('•'));
                }
            }

            for (int i = 0; i < box.Text.Length; i++)
                buf += '•';

            box.Text = buf;
            box.SelectionStart = selStart;
            box.SelectionLength = 0;
        }

        void txtPassw_SelectionChanged(object sender, RoutedEventArgs e) {
            this.selLen = ((TextBox) sender).SelectionLength;
        }

        void ReceiveCaptchaResult(bool result) {
            if (result)
                this.fails--;
        }
    }
}
