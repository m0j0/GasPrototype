using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Prototype.Core.Controls;

namespace Prototype.Core.Design
{
    public class VisualStudioMetadata : IProvideAttributeTable
    {
        public VisualStudioMetadata()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            //builder.AddCustomAttributes(
            //    typeof(Pipe),
            //    new FeatureAttribute(typeof(CustomContextMenuProvider)));
            builder.AddCustomAttributes(
                typeof(Pipe),
                new FeatureAttribute(typeof(OpacitySliderAdornerProvider)));

            AttributeTable = builder.CreateTable();
        }

        public AttributeTable AttributeTable { get; }
    }

    class CustomContextMenuProvider : PrimarySelectionContextMenuProvider
    {
        private MenuAction setBackgroundToBlueMenuAction;
        private MenuAction clearBackgroundMenuAction;

        // The provider's constructor sets up the MenuAction objects 
        // and the the MenuGroup which holds them.
        public CustomContextMenuProvider()
        {
            // Set up the MenuAction which sets the control's 
            // background to Blue.
            setBackgroundToBlueMenuAction = new MenuAction("Blue");
            setBackgroundToBlueMenuAction.Checkable = true;
            setBackgroundToBlueMenuAction.Execute +=
                new EventHandler<MenuActionEventArgs>(SetBackgroundToBlue_Execute);

            // Set up the MenuAction which sets the control's 
            // background to its default value.
            clearBackgroundMenuAction = new MenuAction("Cleared");
            clearBackgroundMenuAction.Checkable = true;
            clearBackgroundMenuAction.Execute +=
                new EventHandler<MenuActionEventArgs>(ClearBackground_Execute);

            // Set up the MenuGroup which holds the MenuAction items.
            MenuGroup backgroundFlyoutGroup =
                new MenuGroup("SetBackgroundsGroup", "Set Background");

            // If HasDropDown is false, the group appears inline, 
            // instead of as a flyout. Set to true.
            backgroundFlyoutGroup.HasDropDown = true;
            backgroundFlyoutGroup.Items.Add(setBackgroundToBlueMenuAction);
            backgroundFlyoutGroup.Items.Add(clearBackgroundMenuAction);
            this.Items.Add(backgroundFlyoutGroup);

            // The UpdateItemStatus event is raised immediately before 
            // this provider shows its tabs, which provides the opportunity 
            // to set states.
            UpdateItemStatus +=
                new EventHandler<MenuActionEventArgs>(
                    CustomContextMenuProvider_UpdateItemStatus);
        }

        // The following method handles the UpdateItemStatus event.
        // It sets the MenuAction states according to the state
        // of the control's Background property. This method is
        // called before the context menu is shown.
        void CustomContextMenuProvider_UpdateItemStatus(
            object sender,
            MenuActionEventArgs e)
        {
            // Turn everything on, and then based on the value 
            // of the BackgroundProperty, selectively turn some off.
            clearBackgroundMenuAction.Checked = false;
            clearBackgroundMenuAction.Enabled = true;
            setBackgroundToBlueMenuAction.Checked = false;
            setBackgroundToBlueMenuAction.Enabled = true;

            // Get a ModelItem which represents the selected control. 
            ModelItem selectedControl = e.Selection.PrimarySelection;

            // Get the value of the Background property from the ModelItem.
            ModelProperty backgroundProperty =
                selectedControl.Properties["Background"];

            // Set the MenuAction items appropriately.
            if (!backgroundProperty.IsSet)
            {
                clearBackgroundMenuAction.Checked = true;
                clearBackgroundMenuAction.Enabled = false;
            }
            else if (backgroundProperty.ComputedValue == Brushes.Blue)
            {
                setBackgroundToBlueMenuAction.Checked = true;
                setBackgroundToBlueMenuAction.Enabled = false;
            }
        }

        // The following method handles the Execute event. 
        // It sets the Background property to its default value.
        void ClearBackground_Execute(
            object sender,
            MenuActionEventArgs e)
        {
            ModelItem selectedControl = e.Selection.PrimarySelection;
            selectedControl.Properties["Background"].ClearValue();
        }

        // The following method handles the Execute event. 
        // It sets the Background property to Brushes.Blue.
        void SetBackgroundToBlue_Execute(
            object sender,
            MenuActionEventArgs e)
        {
            ModelItem selectedControl = e.Selection.PrimarySelection;
            selectedControl.Properties["Background"].SetValue(Brushes.Blue);
        }
    }

    class OpacitySliderAdornerProvider : PrimarySelectionAdornerProvider
    {
        private ModelItem adornedControlModel;
        private ModelEditingScope batchedChange;
        private Slider opacitySlider;
        private AdornerPanel opacitySliderAdornerPanel;

        public OpacitySliderAdornerProvider()
        {
            opacitySlider = new Slider();
        }

        // The following method is called when the adorner is activated.
        // It creates the adorner control, sets up the adorner panel,
        // and attaches a ModelItem to the adorned control.
        protected override void Activate(ModelItem item)
        {
            // Save the ModelItem and hook into when it changes.
            // This enables updating the slider position when 
            // a new Background value is set.
            adornedControlModel = item;
            adornedControlModel.PropertyChanged +=
                new System.ComponentModel.PropertyChangedEventHandler(
                    AdornedControlModel_PropertyChanged);

            // Setup the slider's min and max values.
            opacitySlider.Minimum = 0;
            opacitySlider.Maximum = 1;

            // Setup the adorner panel.
            // All adorners are placed in an AdornerPanel
            // for sizing and layout support.
            AdornerPanel myPanel = this.Panel;

            // The slider extends the full width of the control it adorns.
            AdornerPanel.SetAdornerHorizontalAlignment(
                opacitySlider,
                AdornerHorizontalAlignment.Stretch);

            // Position the adorner above the control it adorns.
            AdornerPanel.SetAdornerVerticalAlignment(
                opacitySlider,
                AdornerVerticalAlignment.OutsideTop);

            // Position the adorner 5 pixels above the control. 
            AdornerPanel.SetAdornerMargin(
                opacitySlider,
                new Thickness(0, 0, 0, 5));

            // Initialize the slider when it is loaded.
            opacitySlider.Loaded += new RoutedEventHandler(slider_Loaded);

            // Handle the value changes of the slider control.
            opacitySlider.ValueChanged +=
                new RoutedPropertyChangedEventHandler<double>(
                    slider_ValueChanged);

            opacitySlider.PreviewMouseLeftButtonUp +=
                new System.Windows.Input.MouseButtonEventHandler(
                    slider_MouseLeftButtonUp);

            opacitySlider.PreviewMouseLeftButtonDown +=
                new System.Windows.Input.MouseButtonEventHandler(
                    slider_MouseLeftButtonDown);

            base.Activate(item);
        }

        // The Panel utility property demand-creates the 
        // adorner panel and adds it to the provider's 
        // Adorners collection.
        public AdornerPanel Panel
        {
            get
            {
                if (this.opacitySliderAdornerPanel == null)
                {
                    opacitySliderAdornerPanel = new AdornerPanel();

                    opacitySliderAdornerPanel.Children.Add(opacitySlider);

                    // Add the panel to the Adorners collection.
                    Adorners.Add(opacitySliderAdornerPanel);
                }

                return this.opacitySliderAdornerPanel;
            }
        }


        // The following method deactivates the adorner.
        protected override void Deactivate()
        {
            adornedControlModel.PropertyChanged -=
                new System.ComponentModel.PropertyChangedEventHandler(
                    AdornedControlModel_PropertyChanged);
            base.Deactivate();
        }

        // The following method handles the PropertyChanged event.
        // It updates the slider control's value if the adorned control's 
        // Background property changed,
        void AdornedControlModel_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Background")
            {
                opacitySlider.Value = GetCurrentOpacity();
            }
        }

        // The following method handles the Loaded event.
        // It assigns the slider control's initial value.
        void slider_Loaded(object sender, RoutedEventArgs e)
        {
            opacitySlider.Value = GetCurrentOpacity();
        }

        // The following method handles the MouseLeftButtonDown event.
        // It calls the BeginEdit method on the ModelItem which represents 
        // the adorned control.
        void slider_MouseLeftButtonDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            batchedChange = adornedControlModel.BeginEdit();
        }

        // The following method handles the MouseLeftButtonUp event.
        // It commits any changes made to the ModelItem which represents the
        // the adorned control.
        void slider_MouseLeftButtonUp(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (batchedChange != null)
            {
                batchedChange.Complete();
                batchedChange.Dispose();
                batchedChange = null;
            }
        }

        // The following method handles the slider control's 
        // ValueChanged event. It sets the value of the 
        // Background opacity by using the ModelProperty type.
        void slider_ValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            double newOpacityValue = e.NewValue;

            // During setup, don't make a value local and set the opacity.
            if (newOpacityValue == GetCurrentOpacity())
            {
                return;
            }

            // Access the adorned control's Background property
            // by using the ModelProperty type.
            ModelProperty backgroundProperty =
                adornedControlModel.Properties["Background"];
            if (!backgroundProperty.IsSet)
            {
                // If the value isn't local, make it local 
                // before setting a sub-property value.
                backgroundProperty.SetValue(backgroundProperty.ComputedValue);
            }

            // Set the Opacity property on the Background Brush.
            backgroundProperty.Value.Properties["Opacity"].SetValue(newOpacityValue);
        }

        // This utility method gets the adorned control's
        // Background brush by using the ModelItem.
        private double GetCurrentOpacity()
        {
            return 0.44;
            Brush backgroundBrushComputedValue =
                (Brush)adornedControlModel.Properties["Background"].ComputedValue;

            return backgroundBrushComputedValue.Opacity;
        }
    }
}

