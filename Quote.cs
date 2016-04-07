using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Quote
{
    public DateTime Date
    {
        get;
        private set;
    }
    public double Open
    {
        get;
        private set;
    }
    public double Close
    {
        get;
        private set;
    }
    public double High
    {
        get;
        private set;
    }
    public double Low
    {
        get;
        private set;
    }
    public ulong Volume
    {
        get;
        private set;
    }
    public int Row
    {
        get;
        private set;
    }

    public Quote(DateTime dt, double open, double close, double high, double low, ulong volume, int iRow)
    {
        Date = dt;
        Open = open;
        Close = close;
        High = high;
        Low = low;
        Volume = volume;
        Row = iRow;
    }
}

