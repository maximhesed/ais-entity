using System;
using System.Windows;
using Ais.model;

namespace Ais.src
{
    public partial class winStockRowManipulator : Window
    {
        readonly string action;
        readonly Stock s;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;

        public winStockRowManipulator(string action, DataGridMergeVirtualEventHandler DataGridMergeVirtual,
                Stock s = null) {
            InitializeComponent();

            foreach (OrdReqs r in Context.ctx.OrdReqs)
                this.cmbOrdReqs.Items.Add(string.Format("[{0}] {1} {2} {3}",
                    r.id, r.prod_name, r.prod_quantity, r.period_date));

            this.cmbOrdReqs.SelectedIndex = 0;
            this.Title = "good";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (action == Actions.Addition) {
                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (action == Actions.Modification) {
                if (s == null)
                    throw new NullReferenceException();

                this.dtpRecDate.Text = s.rec_date + "";

                this.s = s;
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
                Context.ctx.Stock.Add(new Stock {
                    ordid = long.Parse(this.cmbOrdReqs.Text[1] + ""),
                    rec_date = DateTime.Parse(this.dtpRecDate.Text)
                });
            }
            else if (this.action == Actions.Modification) {
                this.s.ordid = long.Parse(this.cmbOrdReqs.Text[1] + "");
                this.s.rec_date = DateTime.Parse(this.dtpRecDate.Text);

                DataGridMergeVirtual?.Invoke();
            }

            Close();
        }
    }
}
