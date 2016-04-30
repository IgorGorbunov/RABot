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
    public decimal Open
    {
        get;
        private set;
    }
    public decimal Close
    {
        get;
        private set;
    }
    public decimal High
    {
        get;
        private set;
    }
    public decimal Low
    {
        get;
        private set;
    }
    public ulong Volume
    {
        get;
        private set;
    }
    public int Lot
    {
        get;
        set;
    }
    public int Row
    {
        get;
        private set;
    }

    public Quote(DateTime dt, decimal open, decimal close, decimal high, decimal low, ulong volume, int iRow)
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

