using System.Linq;
using System.Windows;
using Ais.model;

namespace Ais.src
{
    public partial class winMain : Window
    {
        readonly int eid;
        readonly string title;

        //*
        public winMain() {
            Employees e = Context.ctx.Employees.First(i => i.id == 1);

            InitializeComponent();

            this.eid = 1;
            this.title = this.Title + string.Format(" ({0} {1} {2}{3} {4}) ",
                e.name_last,
                e.name_first,
                Utils.Denull(e.patronymic),
                e.Positions.department,
                e.Positions.position);
            this.Title = this.title;

            ConstraintAccess();
        }
        //*/

        public winMain(int eid) {
            Employees e = Context.ctx.Employees.First(i => i.id == eid);

            InitializeComponent();

            this.eid = eid;
            this.title = this.Title + string.Format(" ({0} {1} {2}{3} {4}) ",
                e.name_last,
                e.name_first,
                Utils.Denull(e.patronymic),
                e.Positions.department,
                e.Positions.position);
            this.Title = this.title;

            ConstraintAccess();
        }

        void btnEmployees_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.Employees.ToList<object>(),
                EntitiesNames.Employee)
            );

            this.Title = this.title + " -> " + this.btnEmployees.Content;
        }

        void btnLeads_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.Leads.ToList<object>(),
                EntitiesNames.Lead)
            );

            this.Title = this.title + " -> " + this.btnLeads.Content;
        }

        void btnGroups_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.Groups.ToList<object>(),
                EntitiesNames.Group)
            );

            this.Title = this.title + " -> " + this.btnGroups.Content;
        }

        void btnOrdReqs_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.OrdReqs.ToList<object>(),
                EntitiesNames.OrdReq)
            );

            this.Title = this.title + " -> " + this.btnOrdReqs.Content;
        }

        void btnContractorsMedia_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.ContractorsMedia.ToList<object>(),
                EntitiesNames.ContractorMedia)
            );

            this.Title = this.title + " -> " + this.btnContractorsMedia.Content;
        }

        void btnContractorsProduction_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
               Context.ctx.ContractorsProduction.ToList<object>(),
               EntitiesNames.ContractorProduction)
            );

            this.Title = this.title + " -> " + this.btnContractorsProduction.Content;
        }

        void btnStock_Click(object sender, RoutedEventArgs e) {
            this.FrameMain.Navigate(new pageTable(
                Context.ctx.Stock.ToList<object>(),
                EntitiesNames.Stock)
            );

            this.Title = this.title + " -> " + this.btnStock.Content;
        }

        void ConstraintAccess() {
            Positions p = Context.ctx.Positions.FirstOrDefault(i => i.uid == this.eid);

            switch (p.department) {
                case "Administrative":
                    switch (p.position) {
                        case "Director":
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnLeads.Visibility = Visibility.Visible;
                            this.btnGroups.Visibility = Visibility.Visible;
                            this.btnOrdReqs.Visibility = Visibility.Visible;
                            this.btnContractorsMedia.Visibility = Visibility.Visible;
                            this.btnContractorsProduction.Visibility = Visibility.Visible;
                            this.btnStock.Visibility = Visibility.Visible;

                            break;
                    }

                    break;

                case "Leads service":
                    switch (p.position) {
                        case "Director":
                            /* Can {info} on Employees its employees. 
                             * Can {info} on Leads. */
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnLeads.Visibility = Visibility.Visible;

                            break;

                        case "Senior manager":
                            /* Can {add, info, remove} on Leads. */
                            this.btnLeads.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Creative":
                    switch (p.position) {
                        case "Director":
                            /* Can {info} on Employees its employees. 
                             * Can {info} on Groups. 
                             * Can {info} on OrdReqs. */
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnGroups.Visibility = Visibility.Visible;
                            this.btnOrdReqs.Visibility = Visibility.Visible;

                            break;

                        case "General producer":
                            /* Can {add, info, remove} on Groups.
                             * Can {add, info, remove} on OrdReqs. */
                            this.btnGroups.Visibility = Visibility.Visible;
                            this.btnOrdReqs.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Media":
                    switch (p.position) {
                        case "Director":
                            /* Can {info} on Employees its employees.
                             * Can {info} on ContractorsMedia. */
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnContractorsMedia.Visibility = Visibility.Visible;

                            break;

                        case "Media buyer specialist":
                            /* Can {add, info, remove} on ContractorsMedia. */
                            this.btnContractorsMedia.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Production":
                    switch (p.position) {
                        case "Director":
                            /* Can {info} on Employees its employees.
                             * Can {info} on ContractorsProduction. */
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnContractorsProduction.Visibility = Visibility.Visible;

                            break;

                        case "Senior manager":
                            /* Can {add, info, remove} on ContractorsProduction. */
                            this.btnContractorsProduction.Visibility = Visibility.Visible;

                            break;

                        case "Manager":
                            /* Can {info} on OrdReqs. */
                            this.btnOrdReqs.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;

                case "Courier":
                    switch (p.position) {
                        case "Director":
                            /* Can {info} on Employees its employees.
                             * Can {info} on Stock. */
                            this.btnEmployees.Visibility = Visibility.Visible;
                            this.btnStock.Visibility = Visibility.Visible;

                            break;

                        case "Senior manager":
                            /* Can {add, info, remove} on Stock. */
                            this.btnStock.Visibility = Visibility.Visible;

                            break;

                        case "Manager":
                            /* Can {info} on Stock. */
                            this.btnStock.Visibility = Visibility.Visible;

                            break;

                        default:
                            break;
                    }

                    break;
            }
        }
    }
}
