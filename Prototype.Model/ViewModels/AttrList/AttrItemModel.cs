using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MugenMvvmToolkit.Models;

namespace Prototype.ViewModels.AttrList
{
   public class AttrItemModel : NotifyPropertyChangedBase
    {
        private int _value;
        private readonly Random _random = new Random();

        public AttrItemModel(string title, string tiePath, bool isEditable = false)
        {
            Update();
        }

        public string Title { get; set; }

        public int Value
        {
            get => _value;
            private set
            {
                if (Equals(_value, value))
                {
                    return;
                }
                _value = value;
                OnPropertyChanged();
            }
        }

        private async void Update()
        {
            while (true)
            {
                Value = _random.Next();
                await Task.Delay(500);
            }
        }
    }
}
