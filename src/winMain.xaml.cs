using System.Linq;
using System.Windows;
using Ais.src.model;

namespace Ais.src
{
    public delegate void DataGridChangedEventHandler();
    public delegate void DbChangedEventHandler();

    public partial class winMain : Window
    {
        readonly int eid;
        readonly string title;
        event DataGridChangedEventHandler DataGridChanged;
        event DbChangedEventHandler DbChanged;
        bool isDataGridChanged;
        PageTableContainer container;

        enum Changes
        {
            Saved,
            Discarded,
            Leaved
        };

        //*
        public winMain() {
            Employees e = Context.ctx.Employees.First(i => i.id == 1);

            InitializeComponent();

            this.eid = 1;
            this.title = this.Title + string.Format(" ({0} {1} {2}{3} {4})",
                e.name_last,
                e.name_first,
                Utils.Denull(e.patronymic),
                e.Positions.department,
                e.Positions.position);
            this.Title = this.title;

            ConstraintAccess();

            DataGridChanged += OnDataGridChanged;
            DbChanged += OnDbChanged;

            this.container.DataGridChanged = DataGridChanged;
            this.container.DbChanged = DbChanged;
        }
        //*/

        public winMain(int eid) {
            Employees e = Context.ctx.Employees.First(i => i.id == eid);

            InitializeComponent();

            this.eid = eid;
            this.title = this.Title + string.Format(" ({0} {1} {2}{3} {4})",
                e.name_last,
                e.name_first,
                Utils.Denull(e.patronymic),
                e.Positions.department,
                e.Positions.position);
            this.Title = this.title;

            ConstraintAccess();

            DataGridChanged += OnDataGridChanged;
            DbChanged += OnDbChanged;

            this.container.DataGridChanged = DataGridChanged;
            this.container.DbChanged = DbChanged;
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (isDataGridChanged) {
                MessageBoxResult result = MessageBox.Show("Do you want to save the changes?",
                    "Save the changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) {
                    if (Context.TrySaveChanges())
                        return;
                }
                else if (result == MessageBoxResult.No)
                    return;

                e.Cancel = true;
            }
        }

        void btnEmployees_Click(object sender, RoutedEventArgs e) {
            if (this.isDataGridChanged && ConfirmSaveChanges() == Changes.Leaved)
                return;

            this.container.lstEntities = Context.ctx.Employees.ToList<object>();
            this.container.entityInstanceName = EntityInstanceNames.Employee;

            this.FrameMain.Navigate(new pageTable(this.container));

            this.Title = this.title + " -> " + this.btnEmployees.Content;
        }

        void btnLeads_Click(object sender, RoutedEventArgs e) {
            if (this.isDataGridChanged && ConfirmSaveChanges() == Changes.Leaved)
                return;

            this.container.lstEntities = Context.ctx.Leads.ToList<object>();
            this.container.entityInstanceName = EntityInstanceNames.Lead;

            this.FrameMain.Navigate(new pageTable(this.container));

            this.Title = this.title + " -> " + this.btnLeads.Content;
        }

        void btnGroups_Click(object sender, RoutedEventArgs e) {
            if (this.isDataGridChanged && ConfirmSaveChanges() == Changes.Leaved)
                return;

            this.container.lstEntities = Context.ctx.Groups.ToList<object>();
            this.container.entityInstanceName = EntityInstanceNames.Group;

            this.FrameMain.Navigate(new pageTable(this.container));

            this.Title = this.title + " -> " + this.btnGroups.Content;
        }

        void btnOrdReqs_Click(object sender, RoutedEventArgs e) {
            if (this.isDataGridChanged && ConfirmSaveChanges() == Changes.Leaved)
                return;

            this.container.lstEntities = Context.ctx.OrdReqs.ToList<object>();
            this.container.entityInstanceName = EntityInstanceNames.OrdReq;

            this.FrameMain.Navigate(new pageTable(this.container));

            this.Title = this.title + " -> " + this.btnOrdReqs.Content;
        }

        void btnContractorsMedia_Click(object sender, RoutedEventArgs e) {
            if (this.isDataGridChanged && ConfirmSaveChanges() == Changes.Leaved)
                return;

            this.container.lstEntities = Context.ctx.ContractorsMedia.ToList<object>();
            this.container.entityInstanceName = EntityInstanceNames.ContractorMedia;

            this.FrameMain.Navigate(new pageTable(this.container));

            this.Title = this.title + " -> " + this.btnContractorsMedia.Content;
        }

        void btnContractorsProduction_Click(object sender, RoutedEventArgs e) {
            if (this.isDataGridChanged && ConfirmSaveChanges() == Changes.Leaved)
                return;

            this.container.lstEntities = Context.ctx.ContractorsProduction.ToList<object>();
            this.container.entityInstanceName = EntityInstanceNames.ContractorProduction;

            this.FrameMain.Navigate(new pageTable(this.container));

            this.Title = this.title + " -> " + this.btnContractorsProduction.Content;
        }

        void btnStock_Click(object sender, RoutedEventArgs e) {
            if (this.isDataGridChanged && ConfirmSaveChanges() == Changes.Leaved)
                return;

            this.container.lstEntities = Context.ctx.Stock.ToList<object>();
            this.container.entityInstanceName = EntityInstanceNames.Stock;

            this.FrameMain.Navigate(new pageTable(this.container));

            this.Title = this.title + " -> " + this.btnStock.Content;
        }

        void ConstraintAccess() {
            Positions p = Context.ctx.Positions.FirstOrDefault(i => i.uid == this.eid);

            switch (p.department) {
                case "Administrative":
                    switch (p.position) {
                        case "Director":
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnLeads.Visibility = Visibility.Visible;
                            this.btnGroups.Visibility = Visibility.Visible;
                            this.btnOrdReqs.Visibility = Visibility.Visible;
                            this.btnContractorsMedia.Visibility = Visibility.Visible;
                            this.btnContractorsProduction.Visibility = Visibility.Visible;
                            this.btnStock.Visibility = Visibility.Visible;

                            break;
                    }

                    break;

                case "Leads service":
                    switch (p.position) {
                        case "Director":
                            /* Can {info} on Employees its employees. 
                             * Can {info} on Leads. */
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnLeads.Visibility = Visibility.Visible;

                            break;

                        case "Senior manager":
                            /* Can {add, info, remove} on Leads. */
                            this.btnLeads.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Creative":
                    switch (p.position) {
                        case "Director":
                            /* Can {info} on Employees its employees. 
                             * Can {info} on Groups. 
                             * Can {info} on OrdReqs. */
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnGroups.Visibility = Visibility.Visible;
                            this.btnOrdReqs.Visibility = Visibility.Visible;

                            break;

                        case "General producer":
                            /* Can {add, info, remove} on Groups.
                             * Can {add, info, remove} on OrdReqs. */
                            this.btnGroups.Visibility = Visibility.Visible;
                            this.btnOrdReqs.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Media":
                    switch (p.position) {
                        case "Director":
                            /* Can {info} on Employees its employees.
                             * Can {info} on ContractorsMedia. */
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnContractorsMedia.Visibility = Visibility.Visible;

                            break;

                        case "Media buyer specialist":
                            /* Can {add, info, remove} on ContractorsMedia. */
                            this.btnContractorsMedia.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Production":
                    switch (p.position) {
                        case "Director":
                            /* Can {info} on Employees its employees.
                             * Can {info} on ContractorsProduction. */
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnContractorsProduction.Visibility = Visibility.Visible;

                            break;

                        case "Senior manager":
                            /* Can {add, info, remove} on ContractorsProduction. */
                            this.btnContractorsProduction.Visibility = Visibility.Visible;

                            break;

                        case "Manager":
                            /* Can {info} on OrdReqs. */
                            this.btnOrdReqs.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Courier":
                    switch (p.position) {
                        case "Director":
                            /* Can {info} on Employees its employees.
                             * Can {info} on Stock. */
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnStock.Visibility = Visibility.Visible;

                            break;

                        case "Senior manager":
                            /* Can {add, info, remove} on Stock. */
                            this.btnStock.Visibility = Visibility.Visible;

                            break;

                        case "Manager":
                            /* Can {info} on Stock. */
                            this.btnStock.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;
            }
        }

        void OnDataGridChanged() {
            this.isDataGridChanged = true;
            this.Title += this.Title.Contains("(Changed)") ? "" : " (Changed)";

            return;
        }

        /* When the database has changed, the data on the grid considered 
         * current. */
        void OnDbChanged() {
            this.isDataGridChanged = false;

            (this.FrameMain.Content as pageTable).dataGrid.Items.Refresh();
        }

        Changes ConfirmSaveChanges() {
            MessageBoxResult result = MessageBox.Show("Do you want to save the changes?",
                "Save the changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                if (Context.TrySaveChanges()) {
                    this.isDataGridChanged = false;

                    return Changes.Saved;
                }
            }
            else if (result == MessageBoxResult.No) {
                Context.ctx = new AgencyEntities();

                this.isDataGridChanged = false;

                return Changes.Discarded;
            }

            return Changes.Leaved;
        }
    }
}
