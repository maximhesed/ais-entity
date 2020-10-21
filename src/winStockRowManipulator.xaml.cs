using System;
using System.Windows;
using System.Windows.Controls;
using Ais.src.model;

namespace Ais.src
{
    public partial class winStockRowManipulator : Window
    {
        readonly string action;
        readonly Stock s;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;
        event DataGridChangedEventHandler DataGridChanged;
        readonly Button btnSave;

        public winStockRowManipulator(RowManipulatorContainer container) {
            InitializeComponent();

            this.action = container.action;
            this.s = (Stock) container.itemSel;
            DataGridMergeVirtual = container.DataGridMergeVirtual;
            DataGridChanged = container.DataGridChanged;
            this.btnSave = container.btnSave;

            foreach (OrdReqs r in Context.ctx.OrdReqs)
                this.cmbOrdReqs.Items.Add(string.Format("[{0}] {1} {2} {3}",
                    r.id, r.prod_name, r.prod_quantity, r.period_date));

            this.cmbOrdReqs.SelectedIndex = 0;
            this.Title = "good";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (this.action == Actions.Addition) {
                this.dtpRecDate.SelectedDate = DateTime.Now;

                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (this.action == Actions.Modification) {
                if (this.s == null)
                    throw new NullReferenceException();

                this.dtpRecDate.SelectedDate = this.s.rec_date;

                this.Title = this.Title.Insert(0, "Modification of the ");
                this.btnDone.Content = "Modify";
                this.btnDone.Width = 65;
            }

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.CanMinimize;
        }

        void btnDone_Click(object sender, RoutedEventArgs e) {
            if (this.action == Actions.Addition) {
                try {
                    Context.ctx.Stock.Add(new Stock {
                        ordid = long.Parse(this.cmbOrdReqs.Text[1] + ""),
                        rec_date = DateTime.Parse(this.dtpRecDate.Text)
                    });
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Save the changes", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }
            }
            else if (this.action == Actions.Modification) {
                this.s.ordid = long.Parse(this.cmbOrdReqs.Text[1] + "");
                this.s.rec_date = DateTime.Parse(this.dtpRecDate.Text);

                DataGridMergeVirtual();
            }

            DataGridChanged();
            this.btnSave.Visibility = Visibility.Visible;

            Close();
        }
    }
}
