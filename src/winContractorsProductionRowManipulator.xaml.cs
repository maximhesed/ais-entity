using System;
using System.Windows;
using Ais.model;

namespace Ais.src
{
    public partial class winContractorsProductionRowManipulator : Window
    {
        readonly string action;
        readonly ContractorsProduction cp;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;

        public winContractorsProductionRowManipulator(string action, DataGridMergeVirtualEventHandler DataGridMergeVirtual,
                ContractorsProduction cp = null) {
            InitializeComponent();

            foreach (OrdReqs r in Context.ctx.OrdReqs)
                this.cmbOrdReqs.Items.Add(string.Format("[{0}] {1} {2} {3}",
                    r.id, r.prod_name, r.prod_quantity, r.period_date));

            this.cmbOrdReqs.SelectedIndex = 0;
            this.Title = "production contractor";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (action == Actions.Addition) {
                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (action == Actions.Modification) {
                if (cp == null)
                    throw new NullReferenceException();

                this.txtFirstName.Text = cp.name_first;
                this.txtLastName.Text = cp.name_last;
                this.txtPatronymic.Text = cp.patronymic;
                this.txtEmail.Text = cp.email;
                this.txtPhone.Text = cp.phone;
                this.txtPrice.Text = cp.price + "";
                Utils.SetComboBoxItem(this.cmbOrdReqs, cp.ordid);

                this.cp = cp;
                this.Title = this.Title.Insert(0, "Modification the ");
                this.btnDone.Content = "Modify";
                this.btnDone.Width = 65;
            }

            this.action = action;
            this.DataGridMergeVirtual = DataGridMergeVirtual;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        void btnDone_Click(object sender, RoutedEventArgs e) {
            if (this.action == Actions.Addition) {
                Context.ctx.ContractorsProduction.Add(new ContractorsProduction {
                    name_first = this.txtFirstName.Text,
                    name_last = this.txtLastName.Text,
                    patronymic = this.txtPatronymic.Text,
                    email = this.txtEmail.Text,
                    phone = this.txtPhone.Text,
                    price = decimal.Parse(this.txtPrice.Text),
                    ordid = long.Parse(this.cmbOrdReqs.Text[1] + "")
                });
            }
            else if (this.action == Actions.Modification) {
                this.cp.name_first = this.txtFirstName.Text;
                this.cp.name_last = this.txtLastName.Text;
                this.cp.patronymic = this.txtPatronymic.Text;
                this.cp.email = this.txtEmail.Text;
                this.cp.phone = this.txtPhone.Text;
                this.cp.price = decimal.Parse(this.txtPrice.Text);
                this.cp.ordid = long.Parse(this.cmbOrdReqs.Text[1] + "");

                DataGridMergeVirtual?.Invoke();
            }

            Close();
        }
    }
}
