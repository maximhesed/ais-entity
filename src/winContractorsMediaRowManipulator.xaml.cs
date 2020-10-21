using System;
using System.Windows;
using System.Windows.Controls;
using Ais.src.model;

namespace Ais.src
{
    public partial class winContractorsMediaRowManipulator : Window
    {
        readonly string action;
        readonly ContractorsMedia cm;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;
        event DataGridChangedEventHandler DataGridChanged;
        readonly Button btnSave;

        public winContractorsMediaRowManipulator(RowManipulatorContainer container) {
            InitializeComponent();

            this.action = container.action;
            this.cm = (ContractorsMedia) container.itemSel;
            DataGridMergeVirtual = container.DataGridMergeVirtual;
            DataGridChanged = container.DataGridChanged;
            this.btnSave = container.btnSave;

            foreach (Leads l in Context.ctx.Leads)
                this.cmbLeads.Items.Add(string.Format("[{0}] {1}{2} {3}",
                    l.id,
                    Utils.Denull(l.name_last),
                    l.name_first,
                    Utils.Denull(l.patronymic)));

            this.cmbLeads.SelectedIndex = 0;
            this.Title = "media contractor";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (this.action == Actions.Addition) {
                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (this.action == Actions.Modification) {
                if (this.cm == null)
                    throw new NullReferenceException();

                this.txtFirstName.Text = this.cm.name_first;
                this.txtLastName.Text = this.cm.name_last;
                this.txtPatronymic.Text = this.cm.patronymic;
                this.txtEmail.Text = this.cm.email;
                this.txtPhone.Text = this.cm.phone;
                this.txtPrice.Text = this.cm.price + "";
                Utils.SetComboBoxItem(this.cmbLeads, (long) this.cm.lid);

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

            if (!Utils.CheckName(this.txtFirstName, "last name"))
                return;

            if (!Utils.CheckName(this.txtFirstName, "patronymic"))
                return;

            if (!Utils.CheckEmailOrPhone(this.txtEmail, this.txtPhone))
                return;

            if (!Utils.CheckPrice(this.txtPrice))
                return;

            this.txtLastName.Text = Utils.Null(this.txtLastName.Text);
            this.txtPatronymic.Text = Utils.Null(this.txtPatronymic.Text);
            this.txtEmail.Text = Utils.Null(this.txtEmail.Text);
            this.txtPhone.Text = Utils.Null(this.txtPhone.Text);

            if (this.action == Actions.Addition) {
                try {
                    Context.ctx.ContractorsMedia.Add(new ContractorsMedia {
                        name_first = this.txtFirstName.Text,
                        name_last = this.txtLastName.Text,
                        patronymic = this.txtPatronymic.Text,
                        email = this.txtEmail.Text,
                        phone = this.txtPhone.Text,
                        price = decimal.Parse(this.txtPrice.Text + ""),
                        lid = int.Parse(this.cmbLeads.Text[1] + "")
                    });
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Save the changes", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }
            }
            else if (this.action == Actions.Modification) {
                this.cm.name_first = this.txtFirstName.Text;
                this.cm.name_last = this.txtLastName.Text;
                this.cm.patronymic = this.txtPatronymic.Text;
                this.cm.email = this.txtEmail.Text;
                this.cm.phone = this.txtPhone.Text;
                this.cm.price = decimal.Parse(this.txtPrice.Text + "");
                this.cm.lid = int.Parse(this.cmbLeads.Text[1] + "");

                DataGridMergeVirtual();
            }

            DataGridChanged();
            this.btnSave.Visibility = Visibility.Visible;

            Close();
        }
    }
}
