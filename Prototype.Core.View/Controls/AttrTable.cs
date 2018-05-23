using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Prototype.Core.Models.AttrList;

namespace Prototype.Core.Controls
{
    public sealed class AttrTable : Control
    {
        static AttrTable()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AttrTable), new FrameworkPropertyMetadata(typeof(AttrTable)));
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(AttrTableVm), typeof(AttrTable), new PropertyMetadata(default(AttrTableVm)));

        public AttrTableVm ViewModel
        {
            get { return (AttrTableVm) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
