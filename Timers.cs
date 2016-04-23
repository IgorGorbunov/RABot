using System;
using System.Windows.Controls;
using System.Windows.Media;


public static class Timers
{
    private static System.Windows.Forms.Timer _timerMoex, _timerLocal;
    private static Label _labelMoex, _labelLocal;

    private static readonly DateTime StartOpen = new DateTime(1,1,1,9,55,0);
    private static readonly DateTime EndOpen = new DateTime(1, 1, 1, 10, 10, 0);
    private static readonly DateTime StartClose = new DateTime(1, 1, 1, 18, 30, 0);
    private static readonly DateTime EndClose = new DateTime(1, 1, 1, 18, 45, 0);

    public static void InitTimers(Label labelMoex, Label labelLocal)
    {
        _labelMoex = labelMoex;
        _timerMoex = new System.Windows.Forms.Timer
            {
                    Interval = 1000
            };
        _timerMoex.Start();
        _timerMoex.Tick += TimerMoexTick;
        _labelLocal = labelLocal;
        _timerLocal = new System.Windows.Forms.Timer
            {
                    Interval = 1000
            };
        _timerLocal.Start();
        _timerLocal.Tick += TimerLocalTick;
    }

    private static void TimerMoexTick(Object myObject, EventArgs myEventArgs)
    {
        DateTime moexTime = DateTime.Now.AddHours(-1);
        _labelMoex.Content = moexTime.ToLongTimeString();
        SetStyle();
    }

    private static void TimerLocalTick(Object myObject, EventArgs myEventArgs)
    {
        _labelLocal.Content = DateTime.Now.ToLongTimeString();
    }

    private static void SetStyle()
    {
        DateTime now = DateTime.Now.AddHours(-1);
        DateTime nowTime = new DateTime(1,1,1, now.Hour, now.Minute, now.Second);
        if (nowTime > StartOpen &&
            nowTime < EndOpen)
        {
            _labelMoex.Foreground = Brushes.Blue;
            _labelLocal.Foreground = Brushes.Blue;
        }
        else if (nowTime > EndOpen &&
                nowTime < StartClose)
        {
            _labelMoex.Foreground = Brushes.Green;
            _labelLocal.Foreground = Brushes.Green;
        }
        else if (nowTime > StartClose &&
                nowTime < EndClose)
        {
            _labelMoex.Foreground = Brushes.Red;
            _labelLocal.Foreground = Brushes.Red;
        }
        else if (nowTime > EndClose ||
                nowTime < StartOpen)
        {
            _labelMoex.Foreground = Brushes.Pink;
            _labelLocal.Foreground = Brushes.Pink;
        }
    }
}

