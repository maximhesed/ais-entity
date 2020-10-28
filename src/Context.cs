using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ais.src.model;

namespace Ais.src
{
    static class Context
    {
        internal static AgencyEntities ctx = new AgencyEntities();

        internal static bool CheckGroupsDependencies() {
            if (ctx.Positions.Where(i => i.position == "Producer").Count() == 0
                    || ctx.Positions.Where(i => i.position == "Artist designer").Count() == 0
                    || ctx.Positions.Where(i => i.position == "Graphics specialist").Count() == 0
                    || ctx.Positions.Where(i => i.position == "Copywriter").Count() == 0
                    || ctx.Leads.Count() == 0) {
                ShowInsufficientDataMsg();

                return false;
            }

            return true;
        }

        internal static bool CheckOrdReqsDependencies() {
            if (ctx.Positions.Where(i => i.position == "Producer").Count() == 0
                    || ctx.Leads.Count() == 0) {
                ShowInsufficientDataMsg();

                return false;
            }

            return true;
        }

        internal static bool CheckContractorsMediaDependencies() {
            if (ctx.Leads.Count() == 0) {
                ShowInsufficientDataMsg();

                return false;
            }

            return true;
        }

        internal static bool CheckContractorsProductionDependencies() {
            if (ctx.OrdReqs.Count() == 0) {
                ShowInsufficientDataMsg();

                return false;
            }

            return true;
        }

        internal static bool CheckStockDependencies() {
            if (ctx.OrdReqs.Count() == 0) {
                ShowInsufficientDataMsg();

                return false;
            }

            return true;
        }

        internal static bool TrySaveChanges() {
            try {
                ctx.SaveChanges();
            } catch (DbEntityValidationException ex) {
                string entityErrStr = "";
                string dbErrStr = "";

                foreach (DbEntityValidationResult r in ex.EntityValidationErrors) {
                    foreach (DbValidationError e in r.ValidationErrors)
                        entityErrStr += string.Format("   - {0}: \"{1}\"\n", e.PropertyName, e.ErrorMessage);

                    dbErrStr += string.Format("- {0}:\n{1}\n", r.Entry.Entity.GetType().Name, entityErrStr);
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

        internal static bool CheckDublicateEmail<T>(T _, TextBox txtEmail, string fieldName)
                where T : class, IEntityIntersector {
            if (ctx.Set<T>().Where(l => l.email == txtEmail.Text).Count() > 0) {
                MessageBox.Show($"{fieldName} with such an email already exists.");

                txtEmail.Focus();

                return false;
            }

            return true;
        }

        internal static bool CheckDublicatePhone<T>(T _, TextBox txtPhone, string fieldName)
                where T : class, IEntityIntersector {
            if (ctx.Set<T>().Where(l => l.phone == txtPhone.Text).Count() > 0) {
                MessageBox.Show($"{fieldName} with such a phone already exists.");

                txtPhone.Focus();

                return false;
            }

            return true;
        }

        internal static bool CheckDublicateEmailOrPhone<T>(T _, TextBox txtEmail,
                TextBox txtPhone, string fieldName) where T : class, IEntityIntersector {
            if (!CheckDublicateEmail(_, txtEmail, fieldName))
                return false;

            if (!CheckDublicatePhone(_, txtPhone, fieldName))
                return false;

            return true;
        }
    }
}
