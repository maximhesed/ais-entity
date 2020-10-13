using System.Linq;
using System.Windows.Controls;
using Ais.model;

namespace Ais.src
{
    public partial class pageProfile : Page
    {
        public pageProfile(int eid) {
            Employees empl = Context.ctx.Employees.FirstOrDefault(i => i.id == eid);
            Positions p = Context.ctx.Positions.FirstOrDefault(i => i.uid == eid);

            InitializeComponent();

            this.lblFullName.Content = string.Format("{0} {1} {2}",
                Utils.Denull(empl.name_last),
                Utils.Denull(empl.name_first),
                Utils.Denull(empl.patronymic));
            this.lblDepartment.Content = p.department;
            this.lblPosition.Content = p.position;
        }
    }
}
