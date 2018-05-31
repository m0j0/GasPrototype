using System.Windows.Controls;

namespace Prototype.Core.Design.Pipes
{
    internal partial class TaskPanel : UserControl
    {
        public TaskPanel()
        {
            InitializeComponent();

            ViewModel = new TaskPanelVm();
            DataContext = ViewModel;
        }

        public TaskPanelVm ViewModel { get; }
    }
}
