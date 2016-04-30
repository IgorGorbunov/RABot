using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RABot.VM_Little_table
{
    public class TableViewer : INotifyPropertyChanged
    {
        public TradeInstrument.Issuer Instrument
        {
            get;
            set;
        }

        public bool IsLong
        {
            get { return _isLong; }
            set
            {
                if (_isLong != value)
                {
                    _isLong = value;
                    OnPropertyChanged("IsLong");
                }
            }
        }

        public decimal OpenValue
        {
            get { return _openValue; }
            set
            {
                if (_openValue != value)
                {
                    _openValue = value;
                    OnPropertyChanged("OpenValue");
                }
            }
        }

        public decimal StopValue
        {
            get { return _stopValue; }
            set
            {
                if (_stopValue != value)
                {
                    _stopValue = value;
                    OnPropertyChanged("StopValue");
                }
            }
        }

        public double? Profit
        {
            get;
            set;
        }

        private bool _isLong;
        private decimal _openValue;
        private decimal _stopValue;

        public bool IsNullValue()
        {
            if (OpenValue <= 0)
            {
                return true;
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

