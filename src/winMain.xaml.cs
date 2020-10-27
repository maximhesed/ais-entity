using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        ConstraintContainer constraint;
        Dictionary<string, ConstraintContainer> constraints;

        /* This is needed, to get the specific employees by an department. */
        string conDep;

        enum Changes
        {
            Saved,
            Discarded,
            Leaved
        };

        /*
        // Test

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
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (this.isDataGridChanged) {
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

        PageTableContainer<T> InitContainer<T>(IQueryable<T> entity, string entityInstanceName) {
            PageTableContainer<T> container = new PageTableContainer<T> {
                entity = entity,
                entityInstanceName = entityInstanceName,
                DataGridChanged = DataGridChanged,
                DbChanged = DbChanged
            };

            if (this.constraint.items != null) {
                foreach (MenuItem item in this.constraint.items) {
                    if (item.Parent != null) {
                        /* Detach the items from the old logical parent, to attach
                         * it to an other data grid's context menu. */
                        (this.FrameMain.Content as pageTable).dataGrid.ContextMenu.Items.Clear();

                        break;
                    }
                }
            }

            container.constraint = this.constraint = this.constraints[entityInstanceName];

            return container;
        }

        int PrepareNavigation<T>(IQueryable<T> entity, string entityInstanceName,
                string title, ref PageTableContainer<T> container) {
            if (this.isDataGridChanged && ConfirmSaveChanges() == Changes.Leaved)
                return -1;

            if (this.conDep != null) {
                IQueryable<Employees> e = entity as IQueryable<Employees>;

                container = InitContainer((IQueryable<T>) e.Where(
                    empl => empl.Positions.department == this.conDep), entityInstanceName);
            }
            else
                container = InitContainer(entity, entityInstanceName);

            this.Title = this.title + " -> " + title;

            return 0;
        }

        void btnEmployees_Click(object sender, RoutedEventArgs e) {
            PageTableContainer<Employees> container = new PageTableContainer<Employees>();

            if (PrepareNavigation(Context.ctx.Employees, EntityInstanceNames.Employee,
                    (string) this.btnEmployees.Content, ref container) == -1)
                return;

            this.FrameMain.Navigate(new pageTable(container));
        }

        void btnLeads_Click(object sender, RoutedEventArgs e) {
            PageTableContainer<Leads> container = new PageTableContainer<Leads>();

            if (PrepareNavigation(Context.ctx.Leads, EntityInstanceNames.Lead,
                    (string) this.btnLeads.Content, ref container) == -1)
                return;

            this.FrameMain.Navigate(new pageTable(container));
        }

        void btnGroups_Click(object sender, RoutedEventArgs e) {
            PageTableContainer<Groups> container = new PageTableContainer<Groups>();

            if (PrepareNavigation(Context.ctx.Groups, EntityInstanceNames.Group,
                    (string) this.btnGroups.Content, ref container) == -1)
                return;

            this.FrameMain.Navigate(new pageTable(container));
        }

        void btnCampaigns_Click(object sender, RoutedEventArgs e) {
            PageTableContainer<Campaigns> container = new PageTableContainer<Campaigns>();

            if (PrepareNavigation(Context.ctx.Campaigns, EntityInstanceNames.Campaign,
                    (string) this.btnCampaigns.Content, ref container) == -1)
                return;

            this.FrameMain.Navigate(new pageTable(container));
        }

        void btnOrdReqs_Click(object sender, RoutedEventArgs e) {
            PageTableContainer<OrdReqs> container = new PageTableContainer<OrdReqs>();

            if (PrepareNavigation(Context.ctx.OrdReqs, EntityInstanceNames.OrdReq,
                    (string) this.btnOrdReqs.Content, ref container) == -1)
                return;

            this.FrameMain.Navigate(new pageTable(container));
        }

        void btnContractorsMedia_Click(object sender, RoutedEventArgs e) {
            PageTableContainer<ContractorsMedia> container =
                new PageTableContainer<ContractorsMedia>();

            if (PrepareNavigation(Context.ctx.ContractorsMedia,
                    EntityInstanceNames.ContractorMedia,
                    (string) this.btnContractorsMedia.Content, ref container) == -1)
                return;

            this.FrameMain.Navigate(new pageTable(container));
        }

        void btnContractorsProduction_Click(object sender, RoutedEventArgs e) {
            PageTableContainer<ContractorsProduction> container =
                new PageTableContainer<ContractorsProduction>();

            if (PrepareNavigation(Context.ctx.ContractorsProduction,
                    EntityInstanceNames.ContractorProduction,
                    (string) this.btnContractorsProduction.Content, ref container) == -1)
                return;

            this.FrameMain.Navigate(new pageTable(container));
        }

        void btnStock_Click(object sender, RoutedEventArgs e) {
            PageTableContainer<Stock> container = new PageTableContainer<Stock>();

            if (PrepareNavigation(Context.ctx.Stock, EntityInstanceNames.Stock,
                    (string) this.btnStock.Content, ref container) == -1)
                return;

            this.FrameMain.Navigate(new pageTable(container));
        }

        void ConstraintAccess() {
            Positions p = Context.ctx.Positions.FirstOrDefault(i => i.eid == this.eid);

            this.constraints = new Dictionary<string, ConstraintContainer>();
            foreach (string f in EntityInstanceNames.ToList())
                this.constraints.Add(f, new ConstraintContainer(
                    new bool[4] { false, false, false, false }));

            switch (p.department) {
                case "Administrative":
                    switch (p.position) {
                        case "Director":
                            /*
                            foreach (string f in EntityInstanceNames.ToList())
                                this.constraints[f] = new ConstraintContainer(
                                    new bool[4] { true, true, false, false });
                            //*/

                            //*
                            foreach (string f in EntityInstanceNames.ToList())
                                this.constraints[f] = new ConstraintContainer(
                                    new bool[4] { true, true, true, true }, true);
                            //*/

                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnLeads.Visibility = Visibility.Visible;
                            this.btnGroups.Visibility = Visibility.Visible;
                            this.btnCampaigns.Visibility = Visibility.Visible;
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
                            this.conDep = "Leads service";

                            this.constraints[EntityInstanceNames.Employee] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });
                            this.constraints[EntityInstanceNames.Lead] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });

                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnLeads.Visibility = Visibility.Visible;

                            break;

                        case "Senior manager":
                            this.constraints[EntityInstanceNames.Lead] = new ConstraintContainer(
                                new bool[4] { true, true, true, true }, true);

                            this.btnLeads.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Creative":
                    switch (p.position) {
                        case "Director":
                            this.conDep = "Creative";

                            this.constraints[EntityInstanceNames.Employee] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });
                            this.constraints[EntityInstanceNames.Group] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });
                            this.constraints[EntityInstanceNames.Campaign] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });
                            this.constraints[EntityInstanceNames.OrdReq] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });

                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnGroups.Visibility = Visibility.Visible;
                            this.btnCampaigns.Visibility = Visibility.Visible;
                            this.btnOrdReqs.Visibility = Visibility.Visible;

                            break;

                        case "General producer":
                            this.constraints[EntityInstanceNames.Group] = new ConstraintContainer(
                                new bool[4] { true, true, true, true }, true);
                            this.constraints[EntityInstanceNames.OrdReq] = new ConstraintContainer(
                                new bool[4] { true, true, true, true }, true);

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
                            this.conDep = "Media";

                            this.constraints[EntityInstanceNames.Employee] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });
                            this.constraints[EntityInstanceNames.ContractorMedia] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });

                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnContractorsMedia.Visibility = Visibility.Visible;

                            break;

                        case "Media buyer specialist":
                            this.constraints[EntityInstanceNames.ContractorMedia] = new ConstraintContainer(
                                new bool[4] { true, true, true, true }, true);

                            this.btnContractorsMedia.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Production":
                    switch (p.position) {
                        case "Director":
                            this.conDep = "Production";

                            this.constraints[EntityInstanceNames.Employee] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });
                            this.constraints[EntityInstanceNames.ContractorProduction] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });

                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnContractorsProduction.Visibility = Visibility.Visible;

                            break;

                        case "Senior manager":
                            this.constraints[EntityInstanceNames.ContractorProduction] = new ConstraintContainer(
                                new bool[4] { true, true, true, true }, true);

                            this.btnContractorsProduction.Visibility = Visibility.Visible;

                            break;

                        case "Manager":
                            this.constraints[EntityInstanceNames.OrdReq] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });

                            this.btnOrdReqs.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Courier":
                    switch (p.position) {
                        case "Director":
                            this.conDep = "Courier";

                            this.constraints[EntityInstanceNames.Employee] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });
                            this.constraints[EntityInstanceNames.Stock] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });

                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnStock.Visibility = Visibility.Visible;

                            break;

                        case "Senior manager":
                            this.constraints[EntityInstanceNames.Stock] = new ConstraintContainer(
                                new bool[4] { true, true, true, true }, true);

                            this.btnStock.Visibility = Visibility.Visible;

                            break;

                        case "Manager":
                            this.constraints[EntityInstanceNames.Stock] = new ConstraintContainer(
                                new bool[4] { true, true, false, false });

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
