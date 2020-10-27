using System.Linq;

namespace Ais.src
{
    public struct PageTableContainer<T>
    {
        internal IQueryable<T> entity;
        internal string entityInstanceName;
        internal DataGridChangedEventHandler DataGridChanged;
        internal DbChangedEventHandler DbChanged;
        internal ConstraintContainer constraint;
    };
}
