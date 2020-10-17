using System;
using System.Linq;
using System.Windows;
using Ais.model;

namespace Ais.src
{
    public partial class winLeadsRowManipulator : Window
    {
        readonly string action;
        readonly Leads l;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;

        public winLeadsRowManipulator(string action, DataGridMergeVirtualEventHandler DataGridMergeVirtual,
                Leads l = null) {
            InitializeComponent();

            this.Title = "lead";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (action == Actions.Addition) {
                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (action == Actions.Modification) {
                if (l == null)
                    throw new NullReferenceException();

                this.txtFirstName.Text = l.name_first;
                this.txtLastName.Text = l.name_last;
                this.txtPatronymic.Text = l.patronymic;
                this.txtEmail.Text = l.email;
                this.txtPhone.Text = l.phone;
                this.txtPromTime.Text = l.prom_time + "";

                this.l = l;
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

                if (Context.ctx.Leads.Local.Count > 0)
                    id = Context.ctx.Leads.Local.Last().id;
                else if (Context.ctx.Leads.ToList().Count > 0)
                    id = Context.ctx.Leads.ToList().Last().id;

                Context.ctx.Leads.Add(new Leads {
                    id = id + 1,
                    name_first = this.txtFirstName.Text,
                    name_last = this.txtLastName.Text,
                    patronymic = this.txtPatronymic.Text,
                    email = this.txtEmail.Text,
                    phone = this.txtPhone.Text,
                    prom_time = Convert.ToInt16(this.txtPromTime.Text)
                });
            }
            else if (this.action == Actions.Modification) {
                this.l.name_first = this.txtFirstName.Text;
                this.l.name_last = this.txtLastName.Text;
                this.l.patronymic = this.txtPatronymic.Text;
                this.l.email = this.txtEmail.Text;
                this.l.phone = this.txtPhone.Text;

                DataGridMergeVirtual?.Invoke();
            }

            Close();
        }
    }
}
