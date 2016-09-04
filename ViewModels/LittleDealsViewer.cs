using System;

namespace RABot.ViewModels
{
    public class LittleDealsViewer
    {
        public string InstrumentName
        {
            get;
            set;
        }

        public DateTime OpenDate
        {
            get;
            set;
        }

        public bool IsLong
        {
            get;
            set;
        }

        public int Volume
        {
            get;
            set;
        }

        public decimal OpenValue
        {
            get;
            set;
        }

        public decimal Value
        {
            get;
            set;
        }

        public decimal LastStopValue
        {
            get;
            set;
        }

        public DateTime LastStopDate
        {
            get;
            set;
        }
    }
}
