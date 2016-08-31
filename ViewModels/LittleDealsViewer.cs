using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RABot.Annotations;

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

        public decimal StopValue
        {
            get;
            set;
        }
    }
}
