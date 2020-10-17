using System;
using System.Linq;
using System.Windows;
using Ais.model;

namespace Ais.src
{
    public partial class winEmployeesRowManipulator : Window
    {
        readonly string action;
        readonly Employees e;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;

        public winEmployeesRowManipulator(string action, DataGridMergeVirtualEventHandler DataGridMergeVirtual,
                Employees e = null) {
            InitializeComponent();

            this.Title = "employee";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (action == Actions.Addition) {
                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (action == Actions.Modification) {
                if (e == null)
                    throw new NullReferenceException();

                this.txtFirstName.Text = e.name_first;
                this.txtLastName.Text = e.name_last;
                this.txtPatronymic.Text = e.patronymic;
                this.txtEmail.Text = e.email;
                this.txtPhone.Text = e.phone;

                this.e = e;
                this.Title = this.Title.Insert(0, "Modification the ");
                this.btnDone.Content = "Modify";
                this.btnDone.Width = 65;
            }

            this.action = action;
            this.DataGridMergeVirtual = DataGridMergeVirtual;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        void btnDone_Click(object sender, RoutedEventArgs _) {
            if (this.action == Actions.Addition) {
                byte id = 0;

                if (Context.ctx.Employees.Local.Count > 0)
                    id = Context.ctx.Employees.Local.Last().id;
                else if (Context.ctx.Employees.ToList().Count > 0)
                    id = Context.ctx.Employees.ToList().Last().id;

                Context.ctx.Employees.Add(new Employees {
                    id = (byte) (id + 1),
                    name_first = this.txtFirstName.Text,
                    name_last = this.txtLastName.Text,
                    patronymic = this.txtPatronymic.Text,
                    email = this.txtEmail.Text,
                    phone = this.txtPhone.Text,
                    reg_date = DateTime.Now.Date
                });
            }
            else if (this.action == Actions.Modification) {
                this.e.name_first = this.txtFirstName.Text;
                this.e.name_last = this.txtLastName.Text;
                this.e.patronymic = this.txtPatronymic.Text;
                this.e.email = this.txtEmail.Text;
                this.e.phone = this.txtPhone.Text;

                DataGridMergeVirtual?.Invoke();
            }

            Close();
        }
    }
}
