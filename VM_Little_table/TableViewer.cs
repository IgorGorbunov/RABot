﻿public class TableViewer
{
    public string Instrument { get; set; }
    public bool IsLong { get; set; }
    public double OpenValue { get; set; }
    public double StopValue { get; set; }
    public double? Profit { get; set; }

    public bool IsNullValue()
    {
        double value = OpenValue;
        if (value <= 0)
        {
            return true;
        }
        return false;
    }
}

