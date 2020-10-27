using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ais.src.model;

namespace Ais.src
{
    public partial class winOrdReqsRowManipulator : Window
    {
        readonly string action;
        readonly OrdReqs r;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;
        event DataGridChangedEventHandler DataGridChanged;
        readonly Button btnSave;

        public winOrdReqsRowManipulator(RowManipulatorContainer container) {
            InitializeComponent();

            this.action = container.action;
            this.r = (OrdReqs) container.itemSel;
            DataGridMergeVirtual = container.DataGridMergeVirtual;
            DataGridChanged = container.DataGridChanged;
            this.btnSave = container.btnSave;

            foreach (Employees e in Context.ctx.Employees.Where(i => i.Positions.position == "Producer"))
                this.cmbProducers.Items.Add(string.Format("({0}) {1} {2} {3}",
                    e.id,
                    e.name_last,
                    e.name_first,
                    Utils.Denull(e.patronymic)));

            foreach (Leads l in Context.ctx.Leads)
                this.cmbLeads.Items.Add(string.Format("({0}) {1}{2} {3}",
                    l.id,
                    Utils.Denull(l.name_last),
                    l.name_first,
                    Utils.Denull(l.patronymic)));

            this.cmbProducers.SelectedIndex = 0;
            this.cmbLeads.SelectedIndex = 0;
            this.Title = "order request";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (this.action == Actions.Addition) {
                this.dtpPeriodDate.SelectedDate = DateTime.Now;

                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (this.action == Actions.Modification) {
                if (this.r == null)
                    throw new NullReferenceException();

                this.txtProdName.Text = this.r.prod_name;
                this.txtProdQuantity.Text = this.r.prod_quantity + "";
                this.dtpPeriodDate.SelectedDate = this.r.period_date;
                this.cmbProducers.Text = this.r.pid + "";
                this.cmbLeads.Text = this.r.lid + "";

                this.Title = this.Title.Insert(0, "Modification of the ");
                this.btnDone.Content = "Modify";
                this.btnDone.Width = 65;
            }

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.CanMinimize;
        }

        void btnDone_Click(object sender, RoutedEventArgs e) {
            if (!Utils.CheckNumeric(this.txtProdQuantity, "product quantity", 1, isReq: true))
                return;

            if (this.action == Actions.Addition) {
                try {
                    Context.ctx.OrdReqs.Add(new OrdReqs {
                        prod_name = this.txtProdName.Text,
                        prod_quantity = int.Parse(this.txtProdQuantity.Text),
                        period_date = DateTime.Parse(this.dtpPeriodDate.Text),
                        pid = byte.Parse(this.cmbProducers.Text[1] + ""),
                        lid = byte.Parse(this.cmbLeads.Text[1] + "")
                    });
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Save the changes", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }
            }
            else if (this.action == Actions.Modification) {
                this.r.prod_name = this.txtProdName.Text;
                this.r.prod_quantity = int.Parse(this.txtProdQuantity.Text);
                this.r.period_date = DateTime.Parse(this.dtpPeriodDate.Text);
                this.r.pid = byte.Parse(this.cmbProducers.Text[1] + "");
                this.r.lid = byte.Parse(this.cmbLeads.Text[1] + "");

                DataGridMergeVirtual();
            }

            DataGridChanged();
            this.btnSave.Visibility = Visibility.Visible;

            Close();
        }
    }
}
