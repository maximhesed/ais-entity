using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using Ais.src.model;

namespace Ais.src
{
    static class Context
    {
        internal static AgencyEntities ctx = new AgencyEntities();

        internal static bool CheckGroupsDependencies() {
            if (ctx.Positions.Where(i => i.position == "Producer") == null
                    || ctx.Positions.Where(i => i.position == "Artist designer") == null
                    || ctx.Positions.Where(i => i.position == "Graphics specialist") == null
                    || ctx.Positions.Where(i => i.position == "Copywriter") == null
                    || ctx.Leads.ToList().Count == 0) {
                ShowInsufficientDataMsg();

                return false;
            }

            return true;
        }

        internal static bool CheckOrdReqsDependencies() {
            if (ctx.Positions.Where(i => i.position == "Producer") == null
                    || ctx.Leads.ToList().Count == 0) {
                ShowInsufficientDataMsg();

                return false;
            }

            return true;
        }

        internal static bool CheckContractorsMediaDependencies() {
            if (ctx.Leads.ToList().Count == 0) {
                ShowInsufficientDataMsg();

                return false;
            }

            return true;
        }

        internal static bool CheckContractorsProductionDependencies() {
            if (ctx.OrdReqs.ToList().Count == 0) {
                ShowInsufficientDataMsg();

                return false;
            }

            return true;
        }

        internal static bool CheckStockDependencies() {
            if (ctx.OrdReqs.ToList().Count == 0) {
                ShowInsufficientDataMsg();

                return false;
            }

            return true;
        }

        internal static bool TrySaveChanges() {
            try {
                ctx.SaveChanges();
            } catch (DbEntityValidationException ex) {
                string entityInstanceName = "";
                string entityErrStr = "";
                string dbErrStr = "";

                foreach (DbEntityValidationResult r in ex.EntityValidationErrors) {
                    entityInstanceName = r.Entry.Entity.GetType().Name;

                    foreach (DbValidationError e in r.ValidationErrors)
                        entityErrStr += string.Format("   - {0}: \"{1}\"\n", e.PropertyName, e.ErrorMessage);

                    dbErrStr += string.Format("- {0}:\n{1}\n", entityInstanceName, entityErrStr);
                }

                MessageBox.Show(string.Format("Tne next entities has the invalid properties:\n\n{0}",
                    dbErrStr), "Save the changes", MessageBoxButton.OK, MessageBoxImage.Information);

                return false;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Save the changes", MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        static void ShowInsufficientDataMsg() {
            MessageBox.Show("An insufficient data for the addition.", "Addition");
        }
    }
}
