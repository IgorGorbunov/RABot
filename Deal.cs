using System;
using System.Collections.Generic;

/// <summary>
/// Класс с данными о сделках
/// </summary>
public class Deal
{
    /// <summary>
    /// Перечисление направления сделки
    /// </summary>
    public enum Direction
    {
        Short = 0,
        Long = 1
    }

    /// <summary>
    /// Возвращает TRUE, если сделка в LONG
    /// </summary>
    public bool IsLong
    {
        get;
        private set;
    }
    /// <summary>
    /// Направление сделки
    /// </summary>
    public Direction DirectionEnum
    {
        get;
        private set;
    }
    /// <summary>
    /// Направление сделки
    /// </summary>
    public string DirectionStr
    {
        get
        {
            switch (DirectionEnum)
            {
                case Direction.Long:
                    return "ЛОНГ";
                case Direction.Short:
                    return "шорт";
            }
            return "";
        }
    }
    /// <summary>
    /// Дата открытия сделки
    /// </summary>
    public DateTime OpenDate
    {
        get;
        private set;
    }
    /// <summary>
    /// Дата закрытия сделки
    /// </summary>
    public DateTime CloseDate
    {
        get;
        private set;
    }
    /// <summary>
    /// Цена входа
    /// </summary>
    public double? OpenValue
    {
        get;
        private set;
    }
    /// <summary>
    /// Цена выхода
    /// </summary>
    public double? CloseValue
    {
        get;
        private set;
    }
    /// <summary>
    /// Прибыль в процентах
    /// </summary>
    public double ProfitProcent
    {
        get
        {
            int directCoef = -1;
            if (IsLong)
            {
                directCoef = 1;
            }
            double? profitProcentNul = (CloseValue - OpenValue) * directCoef * 100 / OpenValue;
            return Math.Round((double)profitProcentNul, 2);
        }
    }
    /// <summary>
    /// Прибыль в процентах c комиссией
    /// </summary>
    public double ProfitProcentWithCosts
    {
        get
        {
            int directCoef = -1;
            if (IsLong)
            {
                directCoef = 1;
            }
            double profitProcentNul = ((((double)CloseValue - (double)OpenValue) * directCoef) - Costs) * 100 / (double)OpenValue;
            return Math.Round(profitProcentNul, 2);
        }
    }
    /// <summary>
    /// Прибыль в процентах
    /// </summary>
    public string ProfitProcentStr
    {
        get
        {
            double? profitProcentNul = ProfitProcent;
            char c = '+';
            if (profitProcentNul < 0)
            {
                c = '-';
            }
            double profitProcent = Math.Abs((double)profitProcentNul);
            return profitProcent.ToString() + c;
        }
    }
    /// <summary>
    /// Перевороты, которые ставились во время жизни сделки
    /// </summary>
    public Dictionary <DateTime, double?> Stops
    {
        get { return _stops; }
    }
    /// <summary>
    /// Брокерская комиссия (за сделку + комиссия за шорт)
    /// </summary>
    public double Costs
    {
        get
        {
            double openCost = Math.Round((double) OpenValue * BrokerCosts.DealProcentCost / 100, 2);
            //if (openCost < BrokerCosts.MinDealCost)
            //{
            //    openCost = BrokerCosts.MinDealCost;
            //}
            double closeCost = Math.Round((double) CloseValue * BrokerCosts.DealProcentCost / 100, 2);
            //if (closeCost < BrokerCosts.MinDealCost)
            //{
            //    closeCost = BrokerCosts.MinDealCost;
            //}
            double shortCost = 0;
            if (!IsLong)
            {
                int nDays = 365;
                if (DateTime.IsLeapYear(DateTime.Now.Year))
                {
                    nDays = 366;
                }
                double procentCostInDay = BrokerCosts.ShortYearProcentCost/nDays;
                double costInDay = Math.Round((double) OpenValue * procentCostInDay / 100, 2);
                shortCost = costInDay * (_stops.Count - 1);
            }
            return openCost + closeCost + shortCost;
        }
    }

    private readonly Dictionary <DateTime, double?> _stops;

    private const string LongTitle = "ЛOHГ";
    private const string ShortTitle = "ШOPT";

    /// <summary>
    /// Конструктор для создания сделки
    /// </summary>
    /// <param name="isLong">TRUE, если сделка ЛОНГ</param>
    /// <param name="openDate">Дата открытия сделки</param>
    /// <param name="openValue">Цена открытия</param>
    public Deal(bool isLong, DateTime openDate, double? openValue) 
        : this (openDate, openValue)
    {
        IsLong = isLong;
        if (isLong)
        {
            DirectionEnum = Direction.Long;
        }
        else
        {
            DirectionEnum = Direction.Short;
        }
    }
    /// <summary>
    /// Конструктор для создания сделки
    /// </summary>
    /// <param name="direction">Направление сделки</param>
    /// <param name="openDate">Дата открытия сделки</param>
    /// <param name="openValue">Цена открытия</param>
    public Deal(string direction, DateTime openDate, double? openValue)
        : this(openDate, openValue)
    {
        SetDirection(direction);
    }

    private Deal(DateTime openDate, double? openValue)
    {
        OpenDate = openDate;
        OpenValue = openValue;
        _stops = new Dictionary<DateTime, double?>();
    }
    /// <summary>
    /// Метод устанавливает новый стоп (переворот)
    /// </summary>
    /// <param name="date"></param>
    /// <param name="stopValue"></param>
    public void SetStopReverse(DateTime date, double? stopValue)
    {
        if (!_stops.ContainsKey(date))
        {
            _stops.Add(date, stopValue);
        }
    }
    /// <summary>
    /// Метод возвращает TRUE, если направление текущей сделки совпадает с передаваемым
    /// </summary>
    /// <param name="directionTitle">Направление сделки</param>
    /// <returns></returns>
    public bool IsSameDirection(string directionTitle)
    {
        string title = StringFunctions.GetClearlyDefinedString(directionTitle.Trim());
        if (IsLong && title == LongTitle)
        {
            return true;
        }
        if (!IsLong && title == ShortTitle)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Метод закрывает сделку
    /// </summary>
    /// <param name="closeDate">Дата закрытия</param>
    /// <param name="closeValue">Цена закрытия</param>
    public void Close(DateTime closeDate, double? closeValue)
    {
        CloseDate = closeDate;
        CloseValue = closeValue;
    }
    /// <summary>
    /// Метод проводит переворот сделки - закрывает текущую, открывает новую в обратном направлении и отдает её на выходе
    /// </summary>
    /// <param name="closeDate">Дата закрытия</param>
    /// <param name="closeValue">Цена закрытия</param>
    /// <returns></returns>
    public Deal Reverse(DateTime closeDate, double? closeValue)
    {
        Close(closeDate, closeValue);
        return new Deal(!IsLong, closeDate, closeValue);
    }
    /// <summary>
    /// Возвращает значение стопа (переворота) на определённую дату
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public double? GetStop(DateTime date)
    {
        if (_stops.ContainsKey(date))
        {
            return _stops[date];
        }
        return null;
    }

    private void SetDirection(string dir)
    {
        string title = StringFunctions.GetClearlyDefinedString(dir.Trim());
        if (title == LongTitle)
        {
            IsLong = true;
            DirectionEnum = Direction.Long;
        }
        if (title == ShortTitle)
        {
            IsLong = false;
            DirectionEnum = Direction.Short;
        }
    }

}

