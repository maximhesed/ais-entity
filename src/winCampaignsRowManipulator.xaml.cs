using System;
using System.Windows;
using System.Windows.Controls;
using Ais.src.model;

namespace Ais.src
{
    public partial class winCampaignsRowManipulator : Window
    {
        readonly Campaigns c;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;
        event DataGridChangedEventHandler DataGridChanged;
        readonly Button btnSave;

        public winCampaignsRowManipulator(RowManipulatorContainer container) {
            InitializeComponent();

            this.c = (Campaigns) container.itemSel;
            DataGridMergeVirtual = container.DataGridMergeVirtual;
            DataGridChanged = container.DataGridChanged;
            this.btnSave = container.btnSave;

            foreach (Groups g in Context.ctx.Groups) {
                string item = string.Format("({0}) {1} {2}",
                    g.id,
                    g.pid,
                    g.Leads.id);

                this.cmbGroups.Items.Add(item);
            }

            this.cmbGroups.SelectedIndex = 0;
            this.Title = "campaign";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (this.c == null)
                throw new NullReferenceException();

            Utils.SetComboBoxItem(this.cmbGroups, this.c.gid);
            this.dtpCompDate.SelectedDate = this.c.comp_date;
            this.txtBudgetStarting.Text = this.c.budget_starting + "";

            this.Title = this.Title.Insert(0, "Modification of the ");
            this.btnDone.Content = "Modify";
            this.btnDone.Width = 65;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.CanMinimize;
            this.UseLayoutRounding = true;
        }

        void btnDone_Click(object sender, RoutedEventArgs e) {
            this.c.gid = byte.Parse(this.cmbGroups.Text[1] + "");
            this.c.comp_date = DateTime.Parse(this.dtpCompDate.Text);
            this.c.budget_starting = decimal.Parse(this.txtBudgetStarting.Text);

            DataGridMergeVirtual();
            DataGridChanged();
            this.btnSave.Visibility = Visibility.Visible;

            Close();
        }
    }
}
