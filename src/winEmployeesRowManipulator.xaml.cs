using System;
using System.Windows;
using System.Windows.Controls;
using Ais.src.model;

namespace Ais.src
{
    public partial class winEmployeesRowManipulator : Window
    {
        readonly string action;
        readonly Employees e;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;
        event DataGridChangedEventHandler DataGridChanged;
        readonly Button btnSave;
        readonly PasswordRedirector redirector;

        public winEmployeesRowManipulator(RowManipulatorContainer container) {
            InitializeComponent();

            this.action = container.action;
            this.e = (Employees) container.itemSel;
            DataGridMergeVirtual = container.DataGridMergeVirtual;
            DataGridChanged = container.DataGridChanged;
            this.btnSave = container.btnSave;

            this.cmbDepartment.Items.Add("Leads service");
            this.cmbDepartment.Items.Add("Creative");
            this.cmbDepartment.Items.Add("Media");
            this.cmbDepartment.Items.Add("Production");
            this.cmbDepartment.Items.Add("Courier");
            this.cmbDepartment.SelectedIndex = 0;
            UpdateDepartmentList();
            this.cmbDepartment.DropDownClosed += cmbDepartment_DropDownClosed;

            /* A passwords redirection setup. */
            this.redirector = new PasswordRedirector(this.txtPassw, this.txtPasswRepeat);
            this.redirector.Bind(this.txtPassw);
            this.redirector.Bind(this.txtPasswRepeat);

            this.Title = "employee";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (this.action == Actions.Addition) {
                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (this.action == Actions.Modification) {
                this.lblPassw.Visibility = Visibility.Collapsed;
                this.txtPassw.Visibility = Visibility.Collapsed;
                this.lblPasswRepeat.Visibility = Visibility.Collapsed;
                this.txtPasswRepeat.Visibility = Visibility.Collapsed;

                if (this.e == null)
                    throw new NullReferenceException();

                this.txtFirstName.Text = this.e.name_first;
                this.txtLastName.Text = this.e.name_last;
                this.txtPatronymic.Text = this.e.patronymic;
                this.txtEmail.Text = this.e.email;
                this.txtPhone.Text = this.e.phone;

                this.Title = this.Title.Insert(0, "Modification of the ");
                this.btnDone.Content = "Modify";
                this.btnDone.Width = 65;
            }

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ResizeMode = ResizeMode.CanMinimize;
        }

        void btnDone_Click(object sender, RoutedEventArgs _) {
            if (!Utils.CheckName(this.txtFirstName, "first name", true))
                return;

            if (!Utils.CheckName(this.txtLastName, "last name", true))
                return;

            if (!Utils.CheckName(this.txtPatronymic, "patronymic"))
                return;

            if (!Utils.CheckEmail(this.txtEmail))
                return;

            if (!Utils.CheckPhone(this.txtPhone))
                return;

            this.txtPatronymic.Text = Utils.Null(this.txtPatronymic.Text);
            this.txtPhone.Text = Utils.Null(this.txtPhone.Text);

            if (this.action == Actions.Addition) {
                if (this.redirector.Passw.Length < 6) {
                    MessageBox.Show("The password length must be greater than 6.", "",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    this.txtPassw.Focus();

                    return;
                }
                else if (this.redirector.Passw != this.redirector.PasswRepeat) {
                    MessageBox.Show("The passwords don't match.", "", MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    this.txtPassw.Focus();

                    return;
                }

                try {
                    Context.ctx.Employees.Add(new Employees {
                        name_first = this.txtFirstName.Text,
                        name_last = this.txtLastName.Text,
                        patronymic = this.txtPatronymic.Text,
                        email = this.txtEmail.Text,
                        password_hash = Utils.Hash(this.redirector.Passw),
                        phone = this.txtPhone.Text,
                        Positions = new Positions {
                            department = this.cmbDepartment.Text,
                            position = this.cmbPosition.Text
                        },
                        reg_date = DateTime.Now.Date
                    });
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Save the changes", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }
            }
            else if (this.action == Actions.Modification) {
                this.e.name_first = this.txtFirstName.Text;
                this.e.name_last = this.txtLastName.Text;
                this.e.patronymic = this.txtPatronymic.Text;
                this.e.email = this.txtEmail.Text;
                this.e.phone = this.txtPhone.Text;

                DataGridMergeVirtual();
            }

            DataGridChanged();
            this.btnSave.Visibility = Visibility.Visible;

            Close();
        }

        private void cmbDepartment_DropDownClosed(object sender, EventArgs e) {
            UpdateDepartmentList();
        }

        private void UpdateDepartmentList() {
            this.cmbPosition.Items.Clear();

            switch (this.cmbDepartment.Text) {
                case "Leads service":
                    this.cmbPosition.Items.Add("Director");
                    this.cmbPosition.Items.Add("Senior manager");
                    this.cmbPosition.Items.Add("Manager");

                    break;

                case "Creative":
                    this.cmbPosition.Items.Add("Director");
                    this.cmbPosition.Items.Add("General producer");
                    this.cmbPosition.Items.Add("Producer");
                    this.cmbPosition.Items.Add("Artist designer");
                    this.cmbPosition.Items.Add("Graphics specialist");
                    this.cmbPosition.Items.Add("Copywriter");

                    break;

                case "Media":
                    this.cmbPosition.Items.Add("Director");
                    this.cmbPosition.Items.Add("Media planner specialist");
                    this.cmbPosition.Items.Add("Media buyer specialist");
                    this.cmbPosition.Items.Add("Monitoring specialist");

                    break;

                case "Production":
                    this.cmbPosition.Items.Add("Director");
                    this.cmbPosition.Items.Add("Senior manager");
                    this.cmbPosition.Items.Add("Manager");

                    break;

                case "Courier":
                    this.cmbPosition.Items.Add("Director");
                    this.cmbPosition.Items.Add("Senior manager");
                    this.cmbPosition.Items.Add("Manager");
                    this.cmbPosition.Items.Add("Courier");

                    break;
            }
            
            this.cmbPosition.SelectedIndex = 0;
        }
    }
}
