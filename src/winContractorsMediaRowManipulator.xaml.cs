using System;
using System.Linq;
using System.Windows;
using Ais.model;

namespace Ais.src
{
    public partial class winContractorsMediaRowManipulator : Window
    {
        readonly string action;
        readonly ContractorsMedia cm;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;

        public winContractorsMediaRowManipulator(string action, DataGridMergeVirtualEventHandler DataGridMergeVirtual,
                ContractorsMedia cm = null) {
            InitializeComponent();

            foreach (Leads l in Context.ctx.Leads)
                this.cmbLeads.Items.Add(string.Format("[{0}] {1}{2}{3}",
                    l.id,
                    Utils.Denull(l.name_last),
                    Utils.Denull(l.name_first),
                    Utils.Denull(l.patronymic)));

            this.cmbLeads.SelectedIndex = 0;
            this.Title = "media contractor";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (action == Actions.Addition) {
                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (action == Actions.Modification) {
                if (cm == null)
                    throw new NullReferenceException();

                this.txtFirstName.Text = cm.name_first;
                this.txtLastName.Text = cm.name_last;
                this.txtPatronymic.Text = cm.patronymic;
                this.txtEmail.Text = cm.email;
                this.txtPhone.Text = cm.phone;
                this.txtPrice.Text = cm.price + "";
                Utils.SetComboBoxItem(this.cmbLeads, (long) cm.lid);

                this.cm = cm;
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
                int id = 0;

                if (Context.ctx.ContractorsMedia.Local.Count > 0)
                    id = Context.ctx.ContractorsMedia.Local.Last().id;
                else if (Context.ctx.ContractorsMedia.ToList().Count > 0)
                    id = Context.ctx.ContractorsMedia.ToList().Last().id;

                Context.ctx.ContractorsMedia.Add(new ContractorsMedia {
                    id = id + 1,
                    name_first = this.txtFirstName.Text,
                    name_last = this.txtLastName.Text,
                    patronymic = this.txtPatronymic.Text,
                    email = this.txtEmail.Text,
                    phone = this.txtPhone.Text,
                    price = Convert.ToInt16(this.txtPrice.Text),
                    lid = Convert.ToInt32(this.cmbLeads.Text[1])
                });
            }
            else if (this.action == Actions.Modification) {
                this.cm.name_first = this.txtFirstName.Text;
                this.cm.name_last = this.txtLastName.Text;
                this.cm.patronymic = this.txtPatronymic.Text;
                this.cm.email = this.txtEmail.Text;
                this.cm.phone = this.txtPhone.Text;
                this.cm.price = Convert.ToInt16(this.txtPrice.Text);
                this.cm.lid = Convert.ToInt32(this.cmbLeads.Text[1]);

                DataGridMergeVirtual?.Invoke();
            }

            Close();
        }
    }
}
