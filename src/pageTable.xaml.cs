using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Ais.src.model;

namespace Ais.src
{
    public delegate void DataGridMergeVirtualEventHandler();

    /* TODO: Pass a generic entity. */
    public partial class pageTable : Page
    {
        List<object> lstEntities;
        readonly List<object> lstAdded = new List<object>();
        readonly List<object> lstRemoved = new List<object>();
        readonly string entityInstanceName;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;
        event DataGridChangedEventHandler DataGridChanged;
        event DbChangedEventHandler DbChanged;
        RowManipulatorContainer container;

        public pageTable(PageTableContainer container) {
            InitializeComponent();

            this.lstEntities = container.lstEntities;
            this.entityInstanceName = container.entityInstanceName;
            DataGridChanged = container.DataGridChanged;
            DbChanged = container.DbChanged;

            if (this.entityInstanceName == EntityInstanceNames.Employee) {
                GetColumnNames(new Employees());
                Context.ctx.Employees.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityInstanceName == EntityInstanceNames.Lead) {
                GetColumnNames(new Leads());
                Context.ctx.Leads.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityInstanceName == EntityInstanceNames.Group) {
                GetColumnNames(new Groups());
                Context.ctx.Groups.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityInstanceName == EntityInstanceNames.OrdReq) {
                GetColumnNames(new OrdReqs());
                Context.ctx.OrdReqs.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityInstanceName == EntityInstanceNames.ContractorMedia) {
                GetColumnNames(new ContractorsMedia());
                Context.ctx.ContractorsMedia.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityInstanceName == EntityInstanceNames.ContractorProduction) {
                GetColumnNames(new ContractorsProduction());
                Context.ctx.ContractorsProduction.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityInstanceName == EntityInstanceNames.Stock) {
                GetColumnNames(new Stock());
                Context.ctx.Stock.Local.CollectionChanged += OnCollectionChanged;
            }

            this.dataGrid.ItemsSource = this.lstEntities;
            this.dataGrid.IsReadOnly = true;
            this.cmbAttributes.SelectedIndex = 0;

            DataGridMergeVirtual += OnDataGridMergeVirtual;
            Unloaded += OnUnloaded;
        }

        void txtSearch_TextChanged(object sender, TextChangedEventArgs e) {
            List<object> lstResults = new List<object>();

            foreach (object item in this.lstEntities) {
                if (Utils.DecomposedSearch(item, this.txtSearch.Text,
                        (string) this.cmbAttributes.SelectedItem))
                    lstResults.Add(item);
            }

            if (string.IsNullOrWhiteSpace(this.txtSearch.Text))
                this.dataGrid.ItemsSource = this.lstEntities;
            else
                this.dataGrid.ItemsSource = lstResults;
        }

        void btnAdd_Click(object sender, RoutedEventArgs e) {
            this.container.action = Actions.Addition;
            this.container.DataGridMergeVirtual = DataGridMergeVirtual;
            this.container.DataGridChanged = DataGridChanged;
            this.container.btnSave = this.btnSave;

            if (this.entityInstanceName == EntityInstanceNames.Employee)
                new winEmployeesRowManipulator(this.container).Show();
            else if (this.entityInstanceName == EntityInstanceNames.Lead)
                new winLeadsRowManipulator(this.container).Show();
            else if (this.entityInstanceName == EntityInstanceNames.Group 
                    && Context.CheckGroupsDependencies())
                new winGroupsRowManipulator(this.container).Show();
            else if (this.entityInstanceName == EntityInstanceNames.OrdReq 
                    && Context.CheckOrdReqsDependencies())
                new winOrdReqsRowManipulator(this.container).Show();
            else if (this.entityInstanceName == EntityInstanceNames.ContractorMedia 
                    && Context.CheckContractorsMediaDependencies())
                new winContractorsMediaRowManipulator(this.container).Show();
            else if (this.entityInstanceName == EntityInstanceNames.ContractorProduction 
                    && Context.CheckContractorsProductionDependencies())
                new winContractorsProductionRowManipulator(this.container).Show();
            else if (this.entityInstanceName == EntityInstanceNames.Stock 
                    && Context.CheckStockDependencies())
                new winStockRowManipulator(this.container).Show();
        }

        void btnSave_Click(object sender, RoutedEventArgs _) {
            if (MessageBox.Show("Are you sure you want to save the changes?",
                    "Save the changes", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes) {
                if (Context.TrySaveChanges()) {
                    DbChanged();

                    this.btnSave.Visibility = Visibility.Collapsed;
                }
            }
        }

        void InfoMenuItem_Click(object sender, RoutedEventArgs _) {
            foreach (object item in this.dataGrid.SelectedItems) {
                Dictionary<string, object> names = new Dictionary<string, object>();

                if (this.entityInstanceName == EntityInstanceNames.Employee) {
                    Employees e = item as Employees;

                    names = new Dictionary<string, object>() {
                        {"Id", e.id},
                        {"Full name", string.Format("{0} {1} {2}",
                            e.name_last,
                            e.name_first,
                            Utils.Denull(e.patronymic))},
                        {"Email", e.email},
                        {"Phone", e.phone},
                        {"Department", e.Positions.department},
                        {"Position", e.Positions.position},
                        {"Date of registration", e._reg_date}
                    };
                }
                else if (this.entityInstanceName == EntityInstanceNames.Lead) {
                    Leads l = item as Leads;

                    names = new Dictionary<string, object>() {
                        {"Id", l.id},
                        {"Full name", string.Format("{0}{1} {2}",
                            Utils.Denull(l.name_last),
                            l.name_first,
                            Utils.Denull(l.patronymic))},
                        {"Email", l.email},
                        {"Phone", l.phone},
                        {"Promotional time", l.prom_time},
                        {"Date of completion", l._appeal_date}
                    };
                }
                else if (this.entityInstanceName == EntityInstanceNames.Group) {
                    Groups g = item as Groups;
                    Employees p = Context.ctx.Employees.FirstOrDefault(i => i.id == g.pid);
                    Employees ad = Context.ctx.Employees.FirstOrDefault(i => i.id == g.adid);
                    Employees gs = Context.ctx.Employees.FirstOrDefault(i => i.id == g.gsid);
                    Employees c = Context.ctx.Employees.FirstOrDefault(i => i.id == g.cid);
                    Leads l = Context.ctx.Leads.FirstOrDefault(i => i.id == g.lid);

                    names = new Dictionary<string, object>() {
                        {"Id", g.id},
                        {"Producer", string.Format("[{0}] {1} {2} {3}",
                            p.id,
                            p.name_last,
                            p.name_first,
                            Utils.Denull(p.patronymic))},
                        {"Artist designer", string.Format("[{0}] {1} {2} {3}",
                            ad.id,
                            ad.name_last,
                            ad.name_first,
                            Utils.Denull(ad.patronymic))},
                        {"Graphics specialist", string.Format("[{0}] {1} {2} {3}",
                            gs.id,
                            gs.name_last,
                            gs.name_first,
                            Utils.Denull(gs.patronymic))},
                        {"Copywriter", string.Format("[{0}] {1} {2} {3}",
                            c.id,
                            c.name_last,
                            c.name_first,
                            Utils.Denull(c.patronymic))},
                        {"Lead", string.Format("[{0}] {1}{2} {3}",
                            l.id,
                            Utils.Denull(l.name_last),
                            l.name_first,
                            Utils.Denull(l.patronymic))},
                        {"Date of completion", g._comp_date}
                    };
                }
                else if (this.entityInstanceName == EntityInstanceNames.OrdReq) {
                    OrdReqs r = item as OrdReqs;
                    Employees p = Context.ctx.Employees.FirstOrDefault(i => i.id == r.pid);
                    Leads l = Context.ctx.Leads.FirstOrDefault(i => i.id == r.lid);

                    names = new Dictionary<string, object>() {
                        {"Id", r.id},
                        {"Product name", r.prod_name},
                        {"Product quantity", r.prod_quantity},
                        {"Period date", r._period_date},
                        {"Producer", string.Format("[{0}] {1} {2} {3}",
                            p.id,
                            p.name_last,
                            p.name_first,
                            Utils.Denull(p.patronymic))},
                        {"Lead", string.Format("[{0}] {1}{2} {3}",
                            l.id,
                            Utils.Denull(l.name_last),
                            l.name_first,
                            Utils.Denull(l.patronymic))},
                    };
                }
                else if (this.entityInstanceName == EntityInstanceNames.ContractorMedia) {
                    ContractorsMedia cm = item as ContractorsMedia;
                    Leads l = Context.ctx.Leads.FirstOrDefault(i => i.id == cm.lid);

                    names = new Dictionary<string, object>() {
                        {"Id", cm.id},
                        {"Full name", string.Format("{0}{1} {2}",
                            Utils.Denull(cm.name_last),
                            cm.name_first,
                            Utils.Denull(cm.patronymic))},
                        {"Email", cm.email},
                        {"Phone", cm.phone},
                        {"Price", cm.price},
                        {"Lead", string.Format("[{0}] {1}{2} {3}",
                            l.id,
                            Utils.Denull(l.name_last),
                            l.name_first,
                            Utils.Denull(l.patronymic))}
                    };
                }
                else if (this.entityInstanceName == EntityInstanceNames.ContractorProduction) {
                    ContractorsProduction cp = item as ContractorsProduction;
                    OrdReqs r = Context.ctx.OrdReqs.FirstOrDefault(i => i.id == cp.ordid);
                    Employees p = Context.ctx.Employees.FirstOrDefault(i => i.id == r.pid);
                    Leads l = Context.ctx.Leads.FirstOrDefault(i => i.id == r.lid);

                    names = new Dictionary<string, object>() {
                        {"Id", cp.ordid},
                        {"Full name", string.Format("{0}{1} {2}",
                            Utils.Denull(p.name_last),
                            p.name_first,
                            Utils.Denull(p.patronymic))},
                        {"Email", cp.email},
                        {"Phone", cp.phone},
                        {"Product name", r.prod_name},
                        {"Product quantity", r.prod_quantity},
                        {"Price", cp.price},
                        {"Period date", r._period_date},
                        {"Producer", string.Format("[{0}] {1} {2} {3}",
                            p.id,
                            p.name_last,
                            p.name_first,
                            Utils.Denull(p.patronymic))},
                        {"Lead", string.Format("[{0}] {1}{2} {3}",
                            l.id,
                            Utils.Denull(l.name_last),
                            l.name_first,
                            Utils.Denull(l.patronymic))}
                    };
                }
                else if (this.entityInstanceName == EntityInstanceNames.Stock) {
                    Stock s = item as Stock;
                    OrdReqs r = Context.ctx.OrdReqs.FirstOrDefault(i => i.id == s.ordid);
                    Employees p = Context.ctx.Employees.FirstOrDefault(i => i.id == r.pid);
                    Leads l = Context.ctx.Leads.FirstOrDefault(i => i.id == r.lid);

                    names = new Dictionary<string, object>() {
                        {"Id", s.ordid},
                        {"Product name", r.prod_name},
                        {"Product quantity", r.prod_quantity},
                        {"Period date", r._period_date},
                        {"Producer", string.Format("[{0}] {1} {2} {3}",
                            p.id,
                            p.name_last,
                            p.name_first,
                            Utils.Denull(p.patronymic))},
                        {"Lead", string.Format("{0}{1} {2}",
                            Utils.Denull(l.name_last),
                            l.name_first,
                            Utils.Denull(l.patronymic))},
                        {"Date of receiving", s._rec_date}
                    };
                }

                new winInfoTable(names, this.entityInstanceName).Show();
            }
        }

        void CopyMenuItem_Click(object sender, RoutedEventArgs e) {
            DataObject data = new DataObject();
            StringBuilder sb = new StringBuilder();

            foreach (object item in this.dataGrid.SelectedItems)
                sb.Append(Utils.RowToStr(item));

            data.SetData(DataFormats.Text, (sb + "").Trim('\n'));
            Clipboard.SetDataObject(data);
        }

        void ModifyMenuItem_Click(object sender, RoutedEventArgs e) {
            this.container.action = Actions.Modification;
            this.container.DataGridMergeVirtual = DataGridMergeVirtual;
            this.container.DataGridChanged = DataGridChanged;
            this.container.btnSave = this.btnSave;

            if (this.entityInstanceName == EntityInstanceNames.Employee) {
                this.container.itemSel = (Employees) this.dataGrid.SelectedItem;

                new winEmployeesRowManipulator(this.container).Show();
            }
            else if (this.entityInstanceName == EntityInstanceNames.Lead) {
                this.container.itemSel = (Leads) this.dataGrid.SelectedItem;

                new winLeadsRowManipulator(this.container).Show();
            }
            else if (this.entityInstanceName == EntityInstanceNames.Group) {
                this.container.itemSel = (Groups) this.dataGrid.SelectedItem;

                new winGroupsRowManipulator(this.container).Show();
            }
            else if (this.entityInstanceName == EntityInstanceNames.OrdReq) {
                this.container.itemSel = (OrdReqs) this.dataGrid.SelectedItem;

                new winOrdReqsRowManipulator(this.container).Show();
            }
            else if (this.entityInstanceName == EntityInstanceNames.ContractorMedia) {
                this.container.itemSel = (ContractorsMedia) this.dataGrid.SelectedItem;

                new winContractorsMediaRowManipulator(this.container).Show();
            }
            else if (this.entityInstanceName == EntityInstanceNames.ContractorProduction) {
                this.container.itemSel = (ContractorsProduction) this.dataGrid.SelectedItem;

                new winContractorsProductionRowManipulator(this.container).Show();
            }
            else if (this.entityInstanceName == EntityInstanceNames.Stock) {
                this.container.itemSel = (Stock) this.dataGrid.SelectedItem;

                new winStockRowManipulator(this.container).Show();
            }
        }

        void RemoveMenuItem_Click(object sender, RoutedEventArgs e) {
            List<object> itemsSel = new List<object>();

            foreach (object item in this.dataGrid.SelectedItems)
                itemsSel.Add(item);

            if (MessageBox.Show("Are you sure you want to delete this data?", "Remove",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                foreach (object item in itemsSel) {
                    if (this.entityInstanceName == EntityInstanceNames.Employee)
                        Context.ctx.Employees.Remove((Employees) item);
                    else if (this.entityInstanceName == EntityInstanceNames.Lead)
                        Context.ctx.Leads.Remove((Leads) item);
                    else if (this.entityInstanceName == EntityInstanceNames.Group)
                        Context.ctx.Groups.Remove((Groups) item);
                    else if (this.entityInstanceName == EntityInstanceNames.OrdReq)
                        Context.ctx.OrdReqs.Remove((OrdReqs) item);
                    else if (this.entityInstanceName == EntityInstanceNames.ContractorMedia)
                        Context.ctx.ContractorsMedia.Remove((ContractorsMedia) item);
                    else if (this.entityInstanceName == EntityInstanceNames.ContractorProduction)
                        Context.ctx.ContractorsProduction.Remove((ContractorsProduction) item);
                    else if (this.entityInstanceName == EntityInstanceNames.Stock)
                        Context.ctx.Stock.Remove((Stock) item);
                }

                DataGridChanged();
                this.btnSave.Visibility = Visibility.Visible;
            }
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (object item in e.NewItems) {
                    this.lstAdded.Add(item);
                    this.lstEntities.Add(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (object item in e.OldItems) {
                    this.lstRemoved.Add(item);
                    this.lstEntities.Remove(item);
                }
            }

            ReloadDataGrid();
        }

        /* The context needs to be updated here. The added and removed 
         * virtual records will be lost. Therefore, need to merge the 
         * updated context with the buffered virtual records. */
        void OnDataGridMergeVirtual() {
            if (this.entityInstanceName == EntityInstanceNames.Employee)
                this.lstEntities = Context.ctx.Employees.ToList<object>();
            else if (this.entityInstanceName == EntityInstanceNames.Lead)
                this.lstEntities = Context.ctx.Leads.ToList<object>();
            else if (this.entityInstanceName == EntityInstanceNames.Group)
                this.lstEntities = Context.ctx.Groups.ToList<object>();
            else if (this.entityInstanceName == EntityInstanceNames.OrdReq)
                this.lstEntities = Context.ctx.OrdReqs.ToList<object>();
            else if (this.entityInstanceName == EntityInstanceNames.ContractorMedia)
                this.lstEntities = Context.ctx.ContractorsMedia.ToList<object>();
            else if (this.entityInstanceName == EntityInstanceNames.ContractorProduction)
                this.lstEntities = Context.ctx.ContractorsProduction.ToList<object>();
            else if (this.entityInstanceName == EntityInstanceNames.Stock)
                this.lstEntities = Context.ctx.Stock.ToList<object>();

            if (this.lstAdded.Count > 0) {
                foreach (object item in this.lstAdded)
                    this.lstEntities.Add(item);
            }

            if (this.lstRemoved.Count > 0) {
                foreach (object item in this.lstRemoved)
                    this.lstEntities.Remove(item);
            }

            ReloadDataGrid();
        }

        void ReloadDataGrid() {
            this.dataGrid.ItemsSource = null;
            this.dataGrid.ItemsSource = this.lstEntities;
        }

        void GetColumnNames(object entity) {
            string[] rowFields = Utils.RowToStr(entity, true, endBreak: false).Split('\t');
            List<string> ignFields = rowFields.Where(f => f[0] == '_').Select(
                f => f.Substring(1)).ToList();
            
            string DoubleScore(string str) {
                string result = str;

                for (int i = 0; i < str.Length; i++) {
                    if (i > 0 && str[i] == '_')
                        result = str.Insert(i, '_' + "");
                }

                return result;
            }
            
            for (int i = 0; i < rowFields.Length; i++) {
                string header, f = header = rowFields[i];
                DataGridTextColumn col = new DataGridTextColumn {
                    Header = DoubleScore(header)
                };

                if (f[0] != '_') {
                    col.Binding = new Binding(ignFields.Contains(f) ? '_' + f : f);

                    this.dataGrid.Columns.Add(col);
                    this.cmbAttributes.Items.Add(f);
                }
            }
        }

        void OnUnloaded(object sender, RoutedEventArgs e) {
            if (this.entityInstanceName == EntityInstanceNames.Employee)
                Context.ctx.Employees.Local.Clear();
            else if (this.entityInstanceName == EntityInstanceNames.Lead)
                Context.ctx.Leads.Local.Clear();
            else if (this.entityInstanceName == EntityInstanceNames.Group)
                Context.ctx.Groups.Local.Clear();
            else if (this.entityInstanceName == EntityInstanceNames.OrdReq)
                Context.ctx.OrdReqs.Local.Clear();
            else if (this.entityInstanceName == EntityInstanceNames.ContractorMedia)
                Context.ctx.ContractorsMedia.Local.Clear();
            else if (this.entityInstanceName == EntityInstanceNames.ContractorProduction)
                Context.ctx.ContractorsProduction.Local.Clear();
            else if (this.entityInstanceName == EntityInstanceNames.Stock)
                Context.ctx.Stock.Local.Clear();

            foreach (DbEntityEntry entry in Context.ctx.ChangeTracker.Entries()) {
                switch (entry.State) {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;

                        break;

                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.State = EntityState.Unchanged;

                        break;
                }
            }

            this.btnSave.Visibility = Visibility.Collapsed;
        }
    }
}
