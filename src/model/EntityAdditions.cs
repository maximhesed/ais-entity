namespace Ais.src.model
{
    public partial class Employees
    {
        public string _reg_date {
            get => reg_date.Date.ToShortDateString();
            private set {
            }
        }
    }

    public partial class Leads
    {
        public string _appeal_date {
            get => appeal_date.Date.ToShortDateString();
            private set {
            }
        }
    }

    public partial class Groups
    {
        public string _comp_date {
            get => comp_date.Date.ToShortDateString();
            private set {
            }
        }
    }

    public partial class OrdReqs
    {
        public string _period_date {
            get => period_date.Date.ToShortDateString();
            private set {
            }
        }
    }

    public partial class Stock
    {
        public string _rec_date {
            get => rec_date.Date.ToShortDateString();
            private set {
            }
        }
    }
}
