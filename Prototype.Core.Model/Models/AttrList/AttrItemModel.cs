using System;
using System.Threading.Tasks;
using MugenMvvmToolkit.Models;

namespace Prototype.Core.Models.AttrList
{
    public class AttrItemModel : NotifyPropertyChangedBase
    {
        private static readonly Random _random = new Random();

        private int _value;

        public AttrItemModel()
        {
            Update();
        }

        public AttrItemModel(string title, string tiePath, bool isEditable = false)
        {
            Title = title;
            TiePath = tiePath;
            IsEditable = isEditable;

            if (!isEditable)
            {
                Update();
            }
        }

        public string Title { get; set; }

        public string TiePath { get; set; }

        public bool IsEditable { get; set; }

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
                Value = _random.Next() % 1000;
                await Task.Delay(1000);
            }
        }
    }
}