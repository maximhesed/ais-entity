using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ais.src.model;

namespace Ais.src
{
    public partial class winGroupsRowManipulator : Window
    {
        readonly string action;
        readonly Groups g;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;
        event DataGridChangedEventHandler DataGridChanged;
        readonly Button btnSave;

        public winGroupsRowManipulator(RowManipulatorContainer container) {
            InitializeComponent();

            this.action = container.action;
            this.g = (Groups) container.itemSel;
            DataGridMergeVirtual = container.DataGridMergeVirtual;
            DataGridChanged = container.DataGridChanged;
            this.btnSave = container.btnSave;

            foreach (Positions p in Context.ctx.Positions.Where(i =>
                    new List<string> {
                        "Producer", "Artist designer", "Graphics specialist", "Copywriter"
                    }.Contains(i.position))) {
                Employees e = Context.ctx.Employees.First(i => i.id == p.uid);
                string item = string.Format("[{0}] {1} {2} {3}",
                    e.id,
                    e.name_last,
                    e.name_first,
                    Utils.Denull(e.patronymic));

                if (p.position == "Producer")
                    this.cmbProducers.Items.Add(item);
                else if (p.position == "Artist designer")
                    this.cmbArtists.Items.Add(item);
                else if (p.position == "Graphics specialist")
                    this.cmbGraphics.Items.Add(item);
                else if (p.position == "Copywriter")
                    this.cmbCopywriters.Items.Add(item);
            }

            foreach (Leads l in Context.ctx.Leads)
                this.cmbLeads.Items.Add(string.Format("[{0}] {1}{2} {3}",
                    l.id,
                    Utils.Denull(l.name_last),
                    l.name_first,
                    Utils.Denull(l.patronymic)));

            this.cmbProducers.SelectedIndex = 0;
            this.cmbArtists.SelectedIndex = 0;
            this.cmbGraphics.SelectedIndex = 0;
            this.cmbCopywriters.SelectedIndex = 0;
            this.cmbLeads.SelectedIndex = 0;
            this.Title = "group";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (this.action == Actions.Addition) {
                this.dtpCompDate.SelectedDate = DateTime.Now;

                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (this.action == Actions.Modification) {
                if (this.g == null)
                    throw new NullReferenceException();

                Utils.SetComboBoxItem(this.cmbProducers, this.g.pid);
                Utils.SetComboBoxItem(this.cmbArtists, this.g.pid);
                Utils.SetComboBoxItem(this.cmbGraphics, this.g.pid);
                Utils.SetComboBoxItem(this.cmbCopywriters, this.g.pid);
                this.dtpCompDate.SelectedDate = this.g.comp_date;
                Utils.SetComboBoxItem(this.cmbLeads, this.g.pid);

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
                    Context.ctx.Groups.Add(new Groups {
                        pid = byte.Parse(this.cmbProducers.Text[1] + ""),
                        adid = byte.Parse(this.cmbArtists.Text[1] + ""),
                        gsid = byte.Parse(this.cmbGraphics.Text[1] + ""),
                        cid = byte.Parse(this.cmbCopywriters.Text[1] + ""),
                        comp_date = DateTime.Parse(this.dtpCompDate.Text),
                        lid = byte.Parse(this.cmbLeads.Text[1] + ""),
                    });
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Save the changes", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }
            }
            else if (this.action == Actions.Modification) {
                this.g.pid = byte.Parse(this.cmbProducers.Text[1] + "");
                this.g.adid = byte.Parse(this.cmbArtists.Text[1] + "");
                this.g.gsid = byte.Parse(this.cmbGraphics.Text[1] + "");
                this.g.cid = byte.Parse(this.cmbCopywriters.Text[1] + "");
                this.g.comp_date = DateTime.Parse(this.dtpCompDate.Text);
                this.g.lid = byte.Parse(this.cmbLeads.Text[1] + "");

                DataGridMergeVirtual();
            }

            DataGridChanged();
            this.btnSave.Visibility = Visibility.Visible;

            Close();
        }
    }
}
