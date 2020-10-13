using System.Linq;
using System.Windows;

namespace Ais.src
{
    public partial class winMain : Window
    {
        readonly int eid;

        public winMain() {
            InitializeComponent();
        }

        public winMain(int eid) {
            InitializeComponent();

            this.eid = eid;

            this.FrameMain.Navigate(new pageProfile(eid));
        }

        void btnProfile_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageProfile(this.eid));
        }

        void btnEmployees_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.Employees.ToList<object>(),
                "Employee")
            );
        }

        void btnLeads_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.Leads.ToList<object>(),
                "Lead")
            );
        }

        void btnGroups_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.Groups.ToList<object>(),
                "Group")
            );
        }

        void btnOrdReqs_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.OrdReqs.ToList<object>(),
                "Order request")
            );
        }

        void btnContractorsProduction_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
               Context.ctx.ContractorsProduction.ToList<object>(),
               "Production contractor")
           );
        }

        void btnContractorsMedia_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.ContractorsMedia.ToList<object>(),
                "Media contractor")
            );
        }

        void btnStock_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.Stock.ToList<object>(),
                "Good")
            );
        }
    }
}
