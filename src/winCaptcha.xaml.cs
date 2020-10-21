using System.Windows;

namespace Ais.src
{
    public partial class winCaptcha : Window
    {
        event CaptchaResultTransferEventHandler CaptchaResultTransfer;

        public winCaptcha(CaptchaResultTransferEventHandler CaptchaResultTransfer) {
            InitializeComponent();

            this.CaptchaResultTransfer = CaptchaResultTransfer;
        }

        void Window_Loaded(object sender, RoutedEventArgs e) {
            this.lblInfo.Content = "Enter the captcha to continue";
            this.lblCaptcha.Content = new string(Generator.GetSequence(5, new string[]
                { "A-Z", "a-z", "0-9" }).ToArray());
        }

        void btnCaptcha_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(this.txtCaptcha.Text)) {
                MessageBox.Show("The captcha field is empty.", "Captcha", MessageBoxButton.OK);

                return;
            }

            if (this.txtCaptcha.Text == (string) this.lblCaptcha.Content) {
                Close();

                CaptchaResultTransfer();

                return;
            }

            MessageBox.Show("Incorrect captcha.", "Captcha", MessageBoxButton.OK);

            this.lblCaptcha.Content = new string(Generator.GetSequence(5, new string[]
                { "A-Z", "a-z", "0-9" }).ToArray());
        }
    }
}
