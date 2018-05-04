using System.Windows;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;

namespace Prototype.Core.Design.Pipes
{
    public class PipeTaskPanelAdornerProvider : PrimarySelectionAdornerProvider
    {
        private readonly TaskPanelVm _taskPanelVm;

        public PipeTaskPanelAdornerProvider()
        {
            var taskPanel = new TaskPanel();
            _taskPanelVm = taskPanel.ViewModel;

            var adornerPanel = new AdornerPanel
            {
                // TODO Order = AdornerOrder.Content,
                Children =
                {
                    taskPanel
                }
            };

            AdornerPanel.SetAdornerHorizontalAlignment(taskPanel, AdornerHorizontalAlignment.OutsideRight);
            AdornerPanel.SetAdornerVerticalAlignment(taskPanel, AdornerVerticalAlignment.OutsideBottom);
            AdornerPanel.SetAdornerMargin(taskPanel, new Thickness(0, 0, 5, 0));

            Adorners.Add(adornerPanel);
        }

        protected override void Activate(ModelItem item)
        {
            base.Activate(item);

            _taskPanelVm.Activate(item);
        }

        protected override void Deactivate()
        {
            base.Deactivate();

            _taskPanelVm.Deactivate();
        }
    }
}
