using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Ais.model;

namespace Ais.src
{
    public delegate void DataGridMergeVirtualEventHandler();

    public partial class pageTable : Page
    {
        List<object> lstEntities;
        readonly List<object> lstAdded = new List<object>();
        readonly List<object> lstRemoved = new List<object>();
        readonly string entityName;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;

        public pageTable(List<object> lstEntities, string entityName) {
            InitializeComponent();

            this.lstEntities = lstEntities;
            this.entityName = entityName;

            if (this.entityName == EntitiesNames.Employee) {
                GetColumnNames(new Employees());
                Context.ctx.Employees.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityName == EntitiesNames.Lead) {
                GetColumnNames(new Leads());
                Context.ctx.Leads.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityName == EntitiesNames.Group) {
                GetColumnNames(new Groups());
                Context.ctx.Groups.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityName == EntitiesNames.OrdReq) {
                GetColumnNames(new OrdReqs());
                Context.ctx.OrdReqs.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityName == EntitiesNames.ContractorMedia) {
                GetColumnNames(new ContractorsMedia());
                Context.ctx.ContractorsMedia.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityName == EntitiesNames.ContractorProduction) {
                GetColumnNames(new ContractorsProduction());
                Context.ctx.ContractorsProduction.Local.CollectionChanged += OnCollectionChanged;
            }
            else if (this.entityName == EntitiesNames.Stock) {
                GetColumnNames(new Stock());
                Context.ctx.Stock.Local.CollectionChanged += OnCollectionChanged;
            }

            this.dataGrid.ItemsSource = lstEntities;
            this.dataGrid.IsReadOnly = true;
            this.cmbAttributes.SelectedIndex = 0;

            DataGridMergeVirtual += OnDataGridMergeVirtual;
            Unloaded += OnUnloaded;
        }

        void txtSearch_TextChanged(object sender, TextChangedEventArgs e) {
            List<object> lstResults = new List<object>();

            foreach (object item in this.lstEntities) {
                if (Utils.DecomposedSearch(item, this.txtSearch.Text,
                        this.cmbAttributes.SelectedIndex))
                    lstResults.Add(item);
            }

            if (string.IsNullOrWhiteSpace(this.txtSearch.Text))
                this.dataGrid.ItemsSource = this.lstEntities;
            else
                this.dataGrid.ItemsSource = lstResults;
        }

        void btnAdd_Click(object sender, RoutedEventArgs e) {
            if (this.entityName == EntitiesNames.Employee)
                new winEmployeesRowManipulator(Actions.Addition, DataGridMergeVirtual).Show();
            else if (this.entityName == EntitiesNames.Lead)
                new winLeadsRowManipulator(Actions.Addition, DataGridMergeVirtual).Show();
            else if (this.entityName == EntitiesNames.Group && Context.CheckGroupsDependencies())
                new winGroupsRowManipulator(Actions.Addition, DataGridMergeVirtual).Show();
            else if (this.entityName == EntitiesNames.OrdReq && Context.CheckOrdReqsDependencies())
                new winOrdReqsRowManipulator(Actions.Addition, DataGridMergeVirtual).Show();
            else if (this.entityName == EntitiesNames.ContractorMedia && Context.CheckContractorsMediaDependencies())
                new winContractorsMediaRowManipulator(Actions.Addition, DataGridMergeVirtual).Show();
            else if (this.entityName == EntitiesNames.ContractorProduction && Context.CheckContractorsProductionDependencies())
                new winContractorsProductionRowManipulator(Actions.Addition, DataGridMergeVirtual).Show();
            else if (this.entityName == EntitiesNames.Stock && Context.CheckStockDependencies())
                new winStockRowManipulator(Actions.Addition, DataGridMergeVirtual).Show();
        }

        void btnSave_Click(object sender, RoutedEventArgs e) {

        }

        void InfoMenuItem_Click(object sender, RoutedEventArgs e) {
            foreach (object item in this.dataGrid.SelectedItems) {
                Dictionary<string, string> names = new Dictionary<string, string>();

                if (this.entityName == "Employee") {
                    Employees empl = item as Employees;
                    Positions p = Context.ctx.Positions.FirstOrDefault(i => i.uid == empl.id);

                    names = new Dictionary<string, string>() {
                        {"Employee", string.Format("{0}{1}{2}",
                            Utils.Denull(empl.name_last),
                            Utils.Denull(empl.name_first),
                            Utils.Denull(empl.patronymic))},
                        {"Email", empl.email + ""},
                        {"Phone", empl.phone + ""},
                        {"Department", p.department + ""},
                        {"Position", p.position + ""},
                        {"Date of registration", empl.reg_date + ""}
                    };
                }
                else if (this.entityName == "Lead") {
                    Leads lead = item as Leads;

                    names = new Dictionary<string, string>() {
                        {"Lead", string.Format("{0}{1}{2}",
                            Utils.Denull(lead.name_last),
                            Utils.Denull(lead.name_first),
                            Utils.Denull(lead.patronymic))},
                        {"Email", lead.email + ""},
                        {"Phone", lead.phone + ""},
                        {"Promotional time", lead.prom_time + ""},
                        {"Date of completion", lead.appeal_date + ""}
                    };
                }
                else if (this.entityName == "Group") {
                    Groups g = item as Groups;
                    Employees p = Context.ctx.Employees.FirstOrDefault(i => i.id == g.pid);
                    Employees ad = Context.ctx.Employees.FirstOrDefault(i => i.id == g.adid);
                    Employees gs = Context.ctx.Employees.FirstOrDefault(i => i.id == g.gsid);
                    Employees c = Context.ctx.Employees.FirstOrDefault(i => i.id == g.cid);
                    Leads l = Context.ctx.Leads.FirstOrDefault(i => i.id == g.lid);

                    names = new Dictionary<string, string>() {
                        {"Producer", string.Format("{0}{1}{2}",
                            Utils.Denull(p.name_last),
                            Utils.Denull(p.name_first),
                            Utils.Denull(p.patronymic))},
                        {"Artist designer", string.Format("{0}{1}{2}",
                            Utils.Denull(ad.name_last),
                            Utils.Denull(ad.name_first),
                            Utils.Denull(ad.patronymic))},
                        {"Graphics specialist", string.Format("{0}{1}{2}",
                            Utils.Denull(gs.name_last),
                            Utils.Denull(gs.name_first),
                            Utils.Denull(gs.patronymic))},
                        {"Copywriter", string.Format("{0}{1}{2}",
                            Utils.Denull(c.name_last),
                            Utils.Denull(c.name_first),
                            Utils.Denull(c.patronymic))},
                        {"Lead", string.Format("{0}{1}{2}",
                            Utils.Denull(l.name_last),
                            Utils.Denull(l.name_first),
                            Utils.Denull(l.patronymic))},
                        {"Date of completion", g.comp_date + ""}
                    };
                }
                else if (this.entityName == "Order request") {
                    OrdReqs r = item as OrdReqs;
                    Employees p = Context.ctx.Employees.FirstOrDefault(i => i.id == r.pid);
                    Leads l = Context.ctx.Leads.FirstOrDefault(i => i.id == r.lid);

                    names = new Dictionary<string, string>() {
                        {"Product name", r.prod_name + ""},
                        {"Product quantity", r.prod_quantity + ""},
                        {"Period date", r.period_date + ""},
                        {"Producer", string.Format("{0}{1}{2}",
                            Utils.Denull(p.name_last),
                            Utils.Denull(p.name_first),
                            Utils.Denull(p.patronymic))},
                        {"Lead", string.Format("{0}{1}{2}",
                            Utils.Denull(l.name_last),
                            Utils.Denull(l.name_first),
                            Utils.Denull(l.patronymic))},
                    };
                }
                else if (this.entityName == "Production contractor") {
                    ContractorsProduction cp = item as ContractorsProduction;
                    OrdReqs r = Context.ctx.OrdReqs.FirstOrDefault(i => i.id == cp.ordid);
                    Employees p = Context.ctx.Employees.FirstOrDefault(i => i.id == r.pid);
                    Leads l = Context.ctx.Leads.FirstOrDefault(i => i.id == r.lid);

                    names = new Dictionary<string, string>() {
                        {"Production contractor", string.Format("{0}{1}{2}",
                            Utils.Denull(p.name_last),
                            Utils.Denull(p.name_first),
                            Utils.Denull(p.patronymic))},
                        {"Email", cp.email + ""},
                        {"Phone", cp.phone + ""},
                        {"Product name", r.prod_name + ""},
                        {"Product quantity", r.prod_quantity + ""},
                        {"Price", cp.price + ""},
                        {"Period date", r.period_date + ""},
                        {"Producer", string.Format("{0}{1}{2}",
                            Utils.Denull(p.name_last),
                            Utils.Denull(p.name_first),
                            Utils.Denull(p.patronymic))},
                        {"Lead", string.Format("{0}{1}{2}",
                            Utils.Denull(l.name_last),
                            Utils.Denull(l.name_first),
                            Utils.Denull(l.patronymic))}
                    };
                }
                else if (this.entityName == "Media contractor") {
                    ContractorsMedia cm = item as ContractorsMedia;
                    Leads l = Context.ctx.Leads.FirstOrDefault(i => i.id == cm.lid);

                    names = new Dictionary<string, string>() {
                        {"Media contractor", string.Format("{0}{1}{2}",
                            Utils.Denull(cm.name_last),
                            Utils.Denull(cm.name_first),
                            Utils.Denull(cm.patronymic))},
                        {"Email", cm.email + ""},
                        {"Phone", cm.phone + ""},
                        {"Price", cm.price + ""},
                        {"Lead", string.Format("{0}{1}{2}",
                            Utils.Denull(l.name_last),
                            Utils.Denull(l.name_first),
                            Utils.Denull(l.patronymic))}
                    };
                }
                else if (this.entityName == "Good") {
                    Stock s = item as Stock;
                    OrdReqs r = Context.ctx.OrdReqs.FirstOrDefault(i => i.id == s.ordid);
                    Employees p = Context.ctx.Employees.FirstOrDefault(i => i.id == r.pid);
                    Leads l = Context.ctx.Leads.FirstOrDefault(i => i.id == r.lid);

                    names = new Dictionary<string, string>() {
                        {"Product name", r.prod_name + ""},
                        {"Product quantity", r.prod_quantity + ""},
                        {"Period date", r.period_date + ""},
                        {"Producer", string.Format("{0}{1}{2}",
                            Utils.Denull(p.name_last),
                            Utils.Denull(p.name_first),
                            Utils.Denull(p.patronymic))},
                        {"Lead", string.Format("{0}{1}{2}",
                            Utils.Denull(l.name_last),
                            Utils.Denull(l.name_first),
                            Utils.Denull(l.patronymic))},
                        {"Date of receiving", s.rec_date + ""}
                    };
                }

                new winInfoTable(names, this.entityName).Show();
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
            if (this.entityName == EntitiesNames.Employee)
                new winEmployeesRowManipulator(Actions.Modification, DataGridMergeVirtual,
                    (Employees) this.dataGrid.SelectedItem).Show();
            else if (this.entityName == EntitiesNames.Lead)
                new winLeadsRowManipulator(Actions.Modification, DataGridMergeVirtual,
                    (Leads) this.dataGrid.SelectedItem).Show();
            else if (this.entityName == EntitiesNames.Group)
                new winGroupsRowManipulator(Actions.Modification, DataGridMergeVirtual,
                    (Groups) this.dataGrid.SelectedItem).Show();
            else if (this.entityName == EntitiesNames.OrdReq)
                new winOrdReqsRowManipulator(Actions.Modification, DataGridMergeVirtual,
                    (OrdReqs) this.dataGrid.SelectedItem).Show();
            else if (this.entityName == EntitiesNames.ContractorMedia)
                new winContractorsMediaRowManipulator(Actions.Modification, DataGridMergeVirtual,
                    (ContractorsMedia) this.dataGrid.SelectedItem).Show();
            else if (this.entityName == EntitiesNames.ContractorProduction)
                new winContractorsProductionRowManipulator(Actions.Modification, DataGridMergeVirtual,
                    (ContractorsProduction) this.dataGrid.SelectedItem).Show();
            else if (this.entityName == EntitiesNames.Stock)
                new winStockRowManipulator(Actions.Modification, DataGridMergeVirtual,
                    (Stock) this.dataGrid.SelectedItem).Show();
        }

        void RemoveMenuItem_Click(object sender, RoutedEventArgs e) {
            StringBuilder sb = new StringBuilder();
            List<object> itemsSel = new List<object>();

            foreach (object item in this.dataGrid.SelectedItems) {
                sb.Append(Utils.RowToStr(item, delim: ' '));
                itemsSel.Add(item);
            }

            if (MessageBox.Show(string.Format(
                    "Are you sure you want to delete this data?\n\n{0}", sb + ""), "Remove",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes) {
                foreach (object item in itemsSel) {
                    if (this.entityName == EntitiesNames.Employee)
                        Context.ctx.Employees.Remove((Employees) item);
                    else if (this.entityName == EntitiesNames.Lead)
                        Context.ctx.Leads.Remove((Leads) item);
                    else if (this.entityName == EntitiesNames.Group)
                        Context.ctx.Groups.Remove((Groups) item);
                    else if (this.entityName == EntitiesNames.OrdReq)
                        Context.ctx.OrdReqs.Remove((OrdReqs) item);
                    else if (this.entityName == EntitiesNames.ContractorMedia)
                        Context.ctx.ContractorsMedia.Remove((ContractorsMedia) item);
                    else if (this.entityName == EntitiesNames.ContractorProduction)
                        Context.ctx.ContractorsProduction.Remove((ContractorsProduction) item);
                    else if (this.entityName == EntitiesNames.Stock)
                        Context.ctx.Stock.Remove((Stock) item);
                }
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

        /* The context needs to be updated here. 
         * The added and removed virtual records will be lost. 
         * Therefore, need to merge the updated context with 
         * the buffered virtual records. */
        void OnDataGridMergeVirtual() {
            if (this.entityName == EntitiesNames.Employee)
                this.lstEntities = Context.ctx.Employees.ToList<object>();
            else if (this.entityName == EntitiesNames.Lead)
                this.lstEntities = Context.ctx.Leads.ToList<object>();
            else if (this.entityName == EntitiesNames.Group)
                this.lstEntities = Context.ctx.Groups.ToList<object>();
            else if (this.entityName == EntitiesNames.OrdReq)
                this.lstEntities = Context.ctx.OrdReqs.ToList<object>();
            else if (this.entityName == EntitiesNames.ContractorMedia)
                this.lstEntities = Context.ctx.ContractorsMedia.ToList<object>();
            else if (this.entityName == EntitiesNames.ContractorProduction)
                this.lstEntities = Context.ctx.ContractorsProduction.ToList<object>();
            else if (this.entityName == EntitiesNames.Stock)
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

        void GetColumnNames<T>(T entity) {
            foreach (string f in Utils.RowToStr(entity, true, endBreak: false).Split('\t')) {
                DataGridTextColumn col = new DataGridTextColumn {
                    Header = f.IndexOf('_') == -1 ? f : f.Insert(f.IndexOf('_'), "_"),
                    Binding = new Binding(f)
                };

                this.dataGrid.Columns.Add(col);
                this.cmbAttributes.Items.Add(f);
            }
        }

        void OnUnloaded(object sender, RoutedEventArgs e) {
            if (this.entityName == EntitiesNames.Employee)
                Context.ctx.Employees.Local.Clear();
            else if (this.entityName == EntitiesNames.Lead)
                Context.ctx.Leads.Local.Clear();
            else if (this.entityName == EntitiesNames.Group)
                Context.ctx.Groups.Local.Clear();
            else if (this.entityName == EntitiesNames.OrdReq)
                Context.ctx.OrdReqs.Local.Clear();
            else if (this.entityName == EntitiesNames.ContractorMedia)
                Context.ctx.ContractorsMedia.Local.Clear();
            else if (this.entityName == EntitiesNames.ContractorProduction)
                Context.ctx.ContractorsProduction.Local.Clear();
            else if (this.entityName == EntitiesNames.Stock)
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
        }
    }
}
