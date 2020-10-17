using System;
using System.Linq;
using System.Windows;
using Ais.model;

namespace Ais.src
{
    public partial class winOrdReqsRowManipulator : Window
    {
        readonly string action;
        readonly OrdReqs r;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;

        public winOrdReqsRowManipulator(string action, DataGridMergeVirtualEventHandler DataGridMergeVirtual,
                OrdReqs r = null) {
            InitializeComponent();

            foreach (Positions p in Context.ctx.Positions.Where(i => i.position == "Producer")) {
                Employees empl = Context.ctx.Employees.First(i => i.id == p.uid);

                this.cmbProducers.Items.Add(string.Format("[{0}] {1}{2}{3}",
                    empl.id,
                    empl.name_last,
                    empl.name_first,
                    Utils.Denull(empl.patronymic)));
            }

            foreach (Leads l in Context.ctx.Leads)
                this.cmbLeads.Items.Add(string.Format("[{0}] {1}{2}{3}",
                    l.id,
                    Utils.Denull(l.name_last),
                    Utils.Denull(l.name_first),
                    Utils.Denull(l.patronymic)));

            this.cmbProducers.SelectedIndex = 0;
            this.cmbLeads.SelectedIndex = 0;
            this.Title = "order request";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (action == Actions.Addition) {
                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (action == Actions.Modification) {
                if (r == null)
                    throw new NullReferenceException();

                this.txtProdName.Text = r.prod_name;
                this.txtProdQuantity.Text = r.prod_quantity + "";
                this.dtpPeriodDate.Text = r.period_date + "";
                this.cmbProducers.Text = r.pid + "";
                this.cmbLeads.Text = r.lid + "";

                this.r = r;
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
                long id = 0;

                if (Context.ctx.OrdReqs.Local.Count > 0)
                    id = Context.ctx.OrdReqs.Local.Last().id;
                else if (Context.ctx.OrdReqs.ToList().Count > 0)
                    id = Context.ctx.OrdReqs.ToList().Last().id;

                Context.ctx.OrdReqs.Add(new OrdReqs {
                    id = id + 1,
                    prod_name = this.txtProdName.Text,
                    prod_quantity = int.Parse(this.txtProdQuantity.Text),
                    period_date = DateTime.Parse(this.dtpPeriodDate.Text),
                    pid = byte.Parse(this.cmbProducers.Text[1] + ""),
                    lid = byte.Parse(this.cmbLeads.Text[1] + "")
                });
            }
            else if (this.action == Actions.Modification) {
                this.r.prod_name = this.txtProdName.Text;
                this.r.prod_quantity = int.Parse(this.txtProdQuantity.Text);
                this.r.period_date = DateTime.Parse(this.dtpPeriodDate.Text);
                this.r.pid = byte.Parse(this.cmbProducers.Text[1] + "");
                this.r.lid = byte.Parse(this.cmbLeads.Text[1] + "");

                DataGridMergeVirtual?.Invoke();
            }

            Close();
        }
    }
}
