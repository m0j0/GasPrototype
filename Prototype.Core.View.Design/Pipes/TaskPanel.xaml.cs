using System.Windows.Controls;

namespace Prototype.Core.Design.Pipes
{
    public partial class TaskPanel : UserControl
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
