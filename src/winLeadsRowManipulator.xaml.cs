using System;
using System.Windows;
using System.Windows.Controls;
using Ais.src.model;

namespace Ais.src
{
    public partial class winLeadsRowManipulator : Window
    {
        readonly string action;
        readonly Leads l;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;
        event DataGridChangedEventHandler DataGridChanged;
        readonly Button btnSave;

        public winLeadsRowManipulator(RowManipulatorContainer container) {
            InitializeComponent();

            this.action = container.action;
            this.l = (Leads) container.itemSel;
            DataGridMergeVirtual = container.DataGridMergeVirtual;
            DataGridChanged = container.DataGridChanged;
            this.btnSave = container.btnSave;

            this.Title = "lead";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (this.action == Actions.Addition) {
                this.dtpAppealDate.SelectedDate = DateTime.Now;

                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (this.action == Actions.Modification) {
                if (this.l == null)
                    throw new NullReferenceException();

                this.txtFirstName.Text = this.l.name_first;
                this.txtLastName.Text = this.l.name_last;
                this.txtPatronymic.Text = this.l.patronymic;
                this.txtEmail.Text = this.l.email;
                this.txtPhone.Text = this.l.phone;
                this.txtPromTime.Text = this.l.prom_time + "";
                this.dtpAppealDate.SelectedDate = this.l.appeal_date;

                this.Title = this.Title.Insert(0, "Modification of the ");
                this.btnDone.Content = "Modify";
                this.btnDone.Width = 65;
            }

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.CanMinimize;
        }

        void btnDone_Click(object sender, RoutedEventArgs e) {
            if (!Utils.CheckName(this.txtFirstName, "first name", true))
                return;

            if (!Utils.CheckName(this.txtLastName, "last name"))
                return;

            if (!Utils.CheckName(this.txtPatronymic, "patronymic"))
                return;

            if (!Utils.CheckEmailOrPhone(this.txtEmail, this.txtPhone))
                return;

            if (!Utils.CheckNumeric(this.txtPromTime, "promotional time", 5, 300, true))
                return;

            this.txtLastName.Text = Utils.Null(this.txtLastName.Text);
            this.txtPatronymic.Text = Utils.Null(this.txtPatronymic.Text);
            this.txtEmail.Text = Utils.Null(this.txtEmail.Text);
            this.txtPhone.Text = Utils.Null(this.txtPhone.Text);

            if (this.action == Actions.Addition) {
                try {
                    Context.ctx.Leads.Add(new Leads {
                        name_first = this.txtFirstName.Text,
                        name_last = this.txtLastName.Text,
                        patronymic = this.txtPatronymic.Text,
                        email = this.txtEmail.Text,
                        phone = this.txtPhone.Text,
                        prom_time = short.Parse(this.txtPromTime.Text),
                        appeal_date = DateTime.Parse(this.dtpAppealDate.Text)
                    });
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Save the changes", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }
            }
            else if (this.action == Actions.Modification) {
                this.l.name_first = this.txtFirstName.Text;
                this.l.name_last = this.txtLastName.Text;
                this.l.patronymic = this.txtPatronymic.Text;
                this.l.email = this.txtEmail.Text;
                this.l.phone = this.txtPhone.Text;
                this.l.appeal_date = DateTime.Parse(this.dtpAppealDate.Text);

                DataGridMergeVirtual();
            }

            DataGridChanged();
            this.btnSave.Visibility = Visibility.Visible;

            Close();
        }
    }
}
