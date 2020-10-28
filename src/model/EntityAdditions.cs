using System.Data.SqlTypes;

namespace Ais.src.model
{
    interface IEntityIntersector
    {
        string email {
            get; set;
        }

        string phone {
            get; set;
        }
    }

    public partial class Employees : IEntityIntersector
    {
        public string _reg_date {
            get => this.reg_date.Date.ToShortDateString();
            private set {
            }
        }

        public string department {
            get => this.Positions.department;
            private set {
            }
        }

        public string position {
            get => this.Positions.position;
            private set {
            }
        }
    }

    public partial class Leads : IEntityIntersector
    {
        public string _appeal_date {
            get => this.appeal_date.Date.ToShortDateString();
            private set {
            }
        }
    }

    public partial class Campaigns
    {
        public string _comp_date {
            get => this.comp_date.Date.ToShortDateString();
            private set {
            }
        }

        public string _status {
            get => this.status ? "finished" : "in progress";
            private set {
            }
        }
    }

    public partial class OrdReqs
    {
        public string _period_date {
            get => this.period_date.Date.ToShortDateString();
            private set {
            }
        }
    }

    public partial class ContractorsMedia : IEntityIntersector
    {
        public string _price {
            get => "$" + SqlMoney.Parse(this.price + "");
            private set {
            }
        }
    }

    public partial class ContractorsProduction : IEntityIntersector
    {
        public string _price {
            get => "$" + SqlMoney.Parse(this.price + "");
            private set {
            }
        }
    }

    public partial class Stock
    {
        public string prod_name {
            get => this.OrdReqs.prod_name;
            private set {
            }
        }

        public string prod_quantity {
            get => this.OrdReqs.prod_quantity + "";
            private set {
            }
        }

        public string _rec_date {
            get => this.rec_date.Date.ToShortDateString();
            private set {
            }
        }
    }
}
