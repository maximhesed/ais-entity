using System;
using System.Windows;
using System.Windows.Controls;
using Ais.src.model;

namespace Ais.src
{
    public partial class winContractorsProductionRowManipulator : Window
    {
        readonly string action;
        readonly ContractorsProduction cp;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;
        event DataGridChangedEventHandler DataGridChanged;
        readonly Button btnSave;

        public winContractorsProductionRowManipulator(RowManipulatorContainer container) {
            InitializeComponent();

            this.action = container.action;
            this.cp = (ContractorsProduction) container.itemSel;
            DataGridMergeVirtual = container.DataGridMergeVirtual;
            DataGridChanged = container.DataGridChanged;
            this.btnSave = container.btnSave;

            foreach (OrdReqs r in Context.ctx.OrdReqs)
                this.cmbOrdReqs.Items.Add(string.Format("[{0}] {1} {2} {3}",
                    r.id, r.prod_name, r.prod_quantity, r.period_date));

            this.cmbOrdReqs.SelectedIndex = 0;
            this.Title = "production contractor";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (this.action == Actions.Addition) {
                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (this.action == Actions.Modification) {
                if (this.cp == null)
                    throw new NullReferenceException();

                this.txtFirstName.Text = this.cp.name_first;
                this.txtLastName.Text = this.cp.name_last;
                this.txtPatronymic.Text = this.cp.patronymic;
                this.txtEmail.Text = this.cp.email;
                this.txtPhone.Text = this.cp.phone;
                this.txtPrice.Text = this.cp.price + "";
                Utils.SetComboBoxItem(this.cmbOrdReqs, this.cp.ordid);

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
                    Context.ctx.ContractorsProduction.Add(new ContractorsProduction {
                        name_first = this.txtFirstName.Text,
                        name_last = this.txtLastName.Text,
                        patronymic = this.txtPatronymic.Text,
                        email = this.txtEmail.Text,
                        phone = this.txtPhone.Text,
                        price = decimal.Parse(this.txtPrice.Text),
                        ordid = long.Parse(this.cmbOrdReqs.Text[1] + "")
                    });
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Save the changes", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }
            }
            else if (this.action == Actions.Modification) {
                this.cp.name_first = this.txtFirstName.Text;
                this.cp.name_last = this.txtLastName.Text;
                this.cp.patronymic = this.txtPatronymic.Text;
                this.cp.email = this.txtEmail.Text;
                this.cp.phone = this.txtPhone.Text;
                this.cp.price = decimal.Parse(this.txtPrice.Text);
                this.cp.ordid = long.Parse(this.cmbOrdReqs.Text[1] + "");

                DataGridMergeVirtual();
            }

            DataGridChanged();
            this.btnSave.Visibility = Visibility.Visible;

            Close();
        }
    }
}
