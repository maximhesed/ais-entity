using System.Windows.Controls;

namespace Ais.src
{
    public struct ConstraintContainer
    {
        /* An each flag corresponds to a menu item.
         *
         * The items distribution:
         *
         * flags[0]: item info;
         * flags[1]: item copy;
         * flags[2]: item modify;
         * flags[3]: item remove. 
         */
        internal bool[] flags;

        internal MenuItem[] items;
        internal bool canAdd;

        public ConstraintContainer(bool[] flags, bool canAdd = false) {
            this.flags = new bool[4];
            for (int i = 0; i < 4; i++)
                this.flags[i] = flags[i];

            this.items = new MenuItem[4] {
                new MenuItem { Header = "Show info" },
                new MenuItem { Header = "Copy" },
                new MenuItem { Header = "Modify" },
                new MenuItem { Header = "Remove" }
            };
            this.canAdd = canAdd;
        }
    }
}
