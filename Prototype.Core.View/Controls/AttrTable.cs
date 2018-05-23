using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MugenMvvmToolkit.Binding;
using MugenMvvmToolkit.Interfaces.Collections;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models.AttrList;

namespace Prototype.Core.Controls
{
    public sealed class AttrTable : Control
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(AttrTableVm), typeof(AttrTable), new PropertyMetadata(default(AttrTableVm), PropertyChangedCallback));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items", typeof(INotifiableCollection<AttrItemModel>), typeof(AttrTable), new PropertyMetadata(default(INotifiableCollection<AttrItemModel>)));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(AttrTable), new PropertyMetadata(default(string)));

        static AttrTable()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AttrTable), new FrameworkPropertyMetadata(typeof(AttrTable)));
        }



        public AttrTableVm ViewModel
        {
            get { return (AttrTableVm) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public INotifiableCollection<AttrItemModel> Items
        {
            get { return (INotifiableCollection<AttrItemModel>) GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        
        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var valve = (AttrTable)d;
            var model = e.NewValue as AttrTableVm;

            if (model == null)
            {
                return;
            }
            valve.Bind(() => v => v.Items).To(model, () => (m, ctx) => m.Attrs).Build();
            valve.Bind(() => v => v.Title).To(model, () => (m, ctx) => m.Title).Build();
        }
    }
}
