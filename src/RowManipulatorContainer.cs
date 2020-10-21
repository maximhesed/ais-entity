using System.Windows.Controls;

namespace Ais.src
{
    public struct RowManipulatorContainer
    {
        internal string action;
        internal DataGridMergeVirtualEventHandler DataGridMergeVirtual;
        internal DataGridChangedEventHandler DataGridChanged;
        internal Button btnSave;
        internal object itemSel;
    };
}
