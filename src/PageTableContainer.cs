using System.Collections.Generic;

namespace Ais.src
{
    public struct PageTableContainer
    {
        internal List<object> lstEntities;
        internal string entityInstanceName;
        internal DataGridChangedEventHandler DataGridChanged;
        internal DbChangedEventHandler DbChanged;
    };
}
