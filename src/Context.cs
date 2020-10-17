using System.Linq;
using System.Windows;
using Ais.model;

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
                MessageBox.Show("An insufficient data for the addition.", "Addition");

                return false;
            }

            return true;
        }

        internal static bool CheckOrdReqsDependencies() {
            if (ctx.Positions.Where(i => i.position == "Producer") == null
                    || ctx.Leads.ToList().Count == 0) {
                MessageBox.Show("An insufficient data for the addition.", "Addition");

                return false;
            }

            return true;
        }

        internal static bool CheckContractorsMediaDependencies() {
            if (ctx.Leads.ToList().Count == 0) {
                MessageBox.Show("An insufficient data for the addition.", "Addition");

                return false;
            }

            return true;
        }

        internal static bool CheckContractorsProductionDependencies() {
            if (ctx.OrdReqs.ToList().Count == 0) {
                MessageBox.Show("An insufficient data for the addition.", "Addition");

                return false;
            }

            return true;
        }

        internal static bool CheckStockDependencies() {
            if (ctx.OrdReqs.ToList().Count == 0) {
                MessageBox.Show("An insufficient data for the addition.", "Addition");

                return false;
            }

            return true;
        }
    }
}
