using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ais.src.model;

namespace Ais.src
{
    public delegate void CaptchaResultTransferEventHandler();

    public partial class winAuth : Window
    {
        int fails = 0;
        event CaptchaResultTransferEventHandler CaptchaResultTransfer;
        readonly string color = "#ffa3a3a3";
        PasswordRedirector redirector;

        public winAuth() {
            InitializeComponent();

            CaptchaResultTransfer += OnCaptchaResultTransfer;
        }

        void Window_Loaded(object sender, RoutedEventArgs e) {
            if (Context.ctx.Employees.Count() == 0)
                RedirectToFirstUse();

            this.redirector = new PasswordRedirector(this.txtPassw);

            this.txtLogin.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.color));
            this.txtPassw.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.color));

            this.txtLogin.GotFocus += SetActive;
            this.txtPassw.GotFocus += SetActive;
        }

        void btnLogin_Click(object sender, RoutedEventArgs e) {
            Employees empl;
            string login = this.txtLogin.Text;
            string passw = this.redirector.Passw;

            if (Context.ctx.Employees.Count() == 0) {
                RedirectToFirstUse();

                return;
            }

            if (string.IsNullOrEmpty(login) || login == "login") {
                MessageBox.Show("Login entered is empty.", "Auth");

                return;
            }

            if (string.IsNullOrEmpty(passw) || passw == "password") {
                MessageBox.Show("Password entered is empty.", "Auth");

                return;
            }

            if (this.fails == 3) {
                new winCaptcha(CaptchaResultTransfer).ShowDialog();

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

            this.txtLogin.GotFocus -= SetActive;
            this.txtPassw.GotFocus -= SetActive;
            this.redirector.Unbind(this.txtPassw);

            this.Visibility = Visibility.Collapsed;

            this.txtLogin.Text = "login";
            this.txtPassw.Text = "password";

            this.txtLogin.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.color));
            this.txtPassw.Foreground = new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.color));

            this.txtLogin.GotFocus += SetActive;
            this.txtPassw.GotFocus += SetActive;

            this.fails = 0;

            new winMain(empl.id).ShowDialog();

            this.Visibility = Visibility.Visible;
        }

        void SetActive(object sender, RoutedEventArgs args) {
            TextBox box = (TextBox) sender;

            this.redirector.Unbind(box);

            box.Foreground = Brushes.Black;
            box.Text = "";

            this.redirector.Bind(box);

            box.GotFocus -= SetActive;
        }

        void OnCaptchaResultTransfer() {
            this.fails--;
        }

        void RedirectToFirstUse() {
            this.Visibility = Visibility.Collapsed;

            MessageBox.Show("There are no employees in the database. " +
                "You have to register yourself as the agency director.", "Auth",
                MessageBoxButton.OK, MessageBoxImage.Information);

            new winEmployeesRowManipulator().ShowDialog();

            this.Visibility = Visibility.Visible;
        }
    }
}
