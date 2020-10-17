using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Ais.model;

namespace Ais.src
{
    public partial class winGroupsRowManipulator : Window
    {
        readonly string action;
        readonly Groups g;
        event DataGridMergeVirtualEventHandler DataGridMergeVirtual;

        public winGroupsRowManipulator(string action, DataGridMergeVirtualEventHandler DataGridMergeVirtual,
                Groups g = null) {
            InitializeComponent();

            foreach (Positions p in Context.ctx.Positions.Where(i =>
                    new List<string> {
                        "Producer", "Artist designer", "Graphics specialist", "Copywriter"
                    }.Contains(i.position))) {
                Employees empl = Context.ctx.Employees.First(i => i.id == p.uid);
                string item = string.Format("[{0}] {1} {2} {3}",
                    empl.id,
                    empl.name_last,
                    empl.name_first,
                    Utils.Denull(empl.patronymic));

                if (p.position == "Producer")
                    this.cmbProducers.Items.Add(item);
                else if (p.position == "Artist designer")
                    this.cmbArtists.Items.Add(item);
                else if (p.position == "Graphics specialist")
                    this.cmbGraphics.Items.Add(item);
                else if (p.position == "Copywriter")
                    this.cmbCopywriters.Items.Add(item);
            }

            foreach (Leads lead in Context.ctx.Leads)
                this.cmbLeads.Items.Add(string.Format("[{0}] {1}{2}{3}",
                    lead.id,
                    Utils.Denull(lead.name_last),
                    Utils.Denull(lead.name_first),
                    Utils.Denull(lead.patronymic)));

            this.cmbProducers.SelectedIndex = 0;
            this.cmbArtists.SelectedIndex = 0;
            this.cmbGraphics.SelectedIndex = 0;
            this.cmbCopywriters.SelectedIndex = 0;
            this.cmbLeads.SelectedIndex = 0;
            this.Title = "group";
            this.btnDone.Margin = new Thickness(0, 7, 0, 7);

            if (action == Actions.Addition) {
                this.Title = this.Title.Insert(0, "Adding a new ");
                this.btnDone.Content = "Add";
                this.btnDone.Width = 50;
            }
            else if (action == Actions.Modification) {
                if (g == null)
                    throw new NullReferenceException();

                Utils.SetComboBoxItem(this.cmbProducers, g.pid);
                Utils.SetComboBoxItem(this.cmbArtists, g.pid);
                Utils.SetComboBoxItem(this.cmbGraphics, g.pid);
                Utils.SetComboBoxItem(this.cmbCopywriters, g.pid);
                this.dtpCompDate.Text = g.comp_date + "";
                Utils.SetComboBoxItem(this.cmbLeads, g.pid);

                this.g = g;
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
                byte id = 0;

                if (Context.ctx.Groups.Local.Count > 0)
                    id = Context.ctx.Groups.Local.Last().id;
                else if (Context.ctx.Groups.ToList().Count > 0)
                    id = Context.ctx.Groups.ToList().Last().id;

                Context.ctx.Groups.Add(new Groups {
                    id = (byte) (id + 1),
                    pid = byte.Parse(this.cmbProducers.Text[1] + ""),
                    adid = byte.Parse(this.cmbArtists.Text[1] + ""),
                    gsid = byte.Parse(this.cmbGraphics.Text[1] + ""),
                    cid = byte.Parse(this.cmbCopywriters.Text[1] + ""),
                    comp_date = DateTime.Parse(this.dtpCompDate.Text),
                    lid = byte.Parse(this.cmbLeads.Text[1] + ""),
                });
            }
            else if (this.action == Actions.Modification) {
                this.g.pid = byte.Parse(this.cmbProducers.Text[1] + "");
                this.g.adid = byte.Parse(this.cmbArtists.Text[1] + "");
                this.g.gsid = byte.Parse(this.cmbGraphics.Text[1] + "");
                this.g.cid = byte.Parse(this.cmbCopywriters.Text[1] + "");
                this.g.comp_date = DateTime.Parse(this.dtpCompDate.Text);
                this.g.lid = byte.Parse(this.cmbLeads.Text[1] + "");

                DataGridMergeVirtual?.Invoke();
            }

            Close();
        }
    }
}
