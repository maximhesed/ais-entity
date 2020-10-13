using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Ais.model;

namespace Ais.src
{
    public partial class pageTable : Page
    {
        readonly List<object> tableList;
        readonly string customEntityName;

        public pageTable(List<object> tableList, string customEntityName) {
            InitializeComponent();

            if (tableList.Count > 0) {
                foreach (string f in Utils.RowToStr(tableList[0], true,
                        endBreak: false).Split('\t')) {
                    DataGridTextColumn col = new DataGridTextColumn {
                        Header = f.IndexOf('_') == -1 ? f : f.Insert(f.IndexOf('_'), "_"),
                        Binding = new Binding(f)
                    };

                    this.dataGrid.Columns.Add(col);
                    this.cmbAttributes.Items.Add(f);
                }
            }

            this.tableList = tableList;
            this.customEntityName = customEntityName;

            this.dataGrid.ItemsSource = tableList;
            this.dataGrid.IsReadOnly = true;
            this.cmbAttributes.SelectedIndex = 0;
        }

        void txtSearch_TextChanged(object sender, TextChangedEventArgs e) {
            List<object> lstResults = new List<object>();

            foreach (object item in this.tableList) {
                if (Utils.DecomposedSearch(item, this.txtSearch.Text, 
                        this.cmbAttributes.SelectedIndex))
                    lstResults.Add(item);
            }

            if (string.IsNullOrWhiteSpace(this.txtSearch.Text))
                this.dataGrid.ItemsSource = this.tableList;
            else
                this.dataGrid.ItemsSource = lstResults;
        }

        void InfoMenuItem_Click(object sender, RoutedEventArgs e) {
            foreach (object item in this.dataGrid.SelectedItems) {
                Dictionary<string, string> names = new Dictionary<string, string>();

                if (this.customEntityName == "Employee") {
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
                else if (this.customEntityName == "Lead") {
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
                else if (this.customEntityName == "Group") {
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
                else if (this.customEntityName == "Order request") {
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
                else if (this.customEntityName == "Production contractor") {
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
                else if (this.customEntityName == "Media contractor") {
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
                else if (this.customEntityName == "Good") {
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

                new winInfoTable(names, this.customEntityName).Show();
            }
        }

        void RemoveMenuItem_Click(object sender, RoutedEventArgs e) {
            StringBuilder sb = new StringBuilder();

            foreach (object item in this.dataGrid.SelectedItems)
                sb.Append(Utils.RowToStr(item, delim: ' '));

            if (MessageBox.Show(string.Format(
                    "Are you sure you want to delete this data?\n\n{0}", sb + ""), "Remove",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes) {

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
    }
}
