using System;
using System.Collections;
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
    public sealed class AttrTableCb : Control
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items", typeof(IList), typeof(AttrTableCb), new PropertyMetadata(default(IList<AttrItemModel>)));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(AttrTableCb), new PropertyMetadata(default(string)));

        static AttrTableCb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AttrTableCb), new FrameworkPropertyMetadata(typeof(AttrTableCb)));
        }

        public AttrTableCb()
        {
            Items = new List<AttrItemModel>();
        }

        public IList Items
        {
            get { return (IList)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        
        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        
    }
}
