using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

/// <summary>
/// Класс для работы с данными об эмитенте
/// </summary>
public class TradeInstrument
{
    /// <summary>
    /// Эмитент
    /// </summary>
    public enum Issuer
    {
        /// <summary>
        /// Отсутствующий эмитент
        /// </summary>
        None,
        /// <summary>
        /// ФСК
        /// </summary>
        Fsk,
        /// <summary>
        /// Газпром
        /// </summary>
        Gazprom,
        /// <summary>
        /// ГМКНорНикель
        /// </summary>
        GmkNorNikel,
        /// <summary>
        /// Лукойл
        /// </summary>
        Lukoil,
        /// <summary>
        /// МТС
        /// </summary>
        Mts,
        /// <summary>
        /// Роснефть
        /// </summary>
        Rosneft,
        /// <summary>
        /// Ростелеком
        /// </summary>
        Rostelekom,
        /// <summary>
        /// Сбербанк ао
        /// </summary>
        SberbankAo,
        /// <summary>
        /// Сбербанк ап
        /// </summary>
        SberbankAp,
        /// <summary>
        /// Северсталь
        /// </summary>
        Severstal,
        /// <summary>
        /// СургутНефтеГаз ао
        /// </summary>
        SurgutNfgAo,
        /// <summary>
        /// СургутНефетГаз ап
        /// </summary>
        SurgutNfgAp,
        /// <summary>
        /// ВТБ
        /// </summary>
        Vtb
    }

    /// <summary>
    /// Метод возвращает код эмитента
    /// </summary>
    /// <param name="issuer">Эмитент</param>
    /// <returns></returns>
    public static string GetIssuerCode(Issuer issuer)
    {
        switch (issuer)
        {
            case Issuer.Fsk:
                return "FEES";
            case Issuer.Gazprom:
                return "GAZP";
            case Issuer.GmkNorNikel:
                return "GMKN";
            case Issuer.Lukoil:
                return "LKOH";
            case Issuer.Mts:
                return "MTSS";
            case Issuer.Rosneft:
                return "ROSN";
            case Issuer.Rostelekom:
                return "RTKM";
            case Issuer.SberbankAo:
                return "SBER";
            case Issuer.SberbankAp:
                return "SBERP";
            case Issuer.Severstal:
                return "CHMF";
            case Issuer.SurgutNfgAo:
                return "SNGS";
            case Issuer.SurgutNfgAp:
                return "SNGSP";
            case Issuer.Vtb:
                return "VTBR";
        }
        return null;
    }
    /// <summary>
    /// Метод возвращает название эмитента
    /// </summary>
    /// <param name="issuer">Эмитент</param>
    /// <returns></returns>
    public static string GetIssuerName(Issuer issuer)
    {
        switch (issuer)
        {
            case Issuer.Fsk:
                return "ФСК";
            case Issuer.Gazprom:
                return "Газпром";
            case Issuer.GmkNorNikel:
                return "ГМКНорНикель";
            case Issuer.Lukoil:
                return "Лукойл";
            case Issuer.Mts:
                return "МТС";
            case Issuer.Rosneft:
                return "Роснефть";
            case Issuer.Rostelekom:
                return "Ростелеком";
            case Issuer.SberbankAo:
                return "Сбербанк ао";
            case Issuer.SberbankAp:
                return "Сбербанк ап";
            case Issuer.Severstal:
                return "Северсталь";
            case Issuer.SurgutNfgAo:
                return "СургутНефтеГаз ао";
            case Issuer.SurgutNfgAp:
                return "СургутНефтеГаз ап";
            case Issuer.Vtb:
                return "ВТБ";
        }
        return null;
    }
    /// <summary>
    /// Метод эмитент по его коду
    /// </summary>
    /// <param name="issuerCode">Код эмитента</param>
    /// <returns></returns>
    public static Issuer GetIssuer(string issuerCode)
    {
        switch (issuerCode)
        {
            case "FEES":
                return Issuer.Fsk;
            case "GAZP":
                return Issuer.Gazprom;
            case "GMKN":
                return Issuer.GmkNorNikel;
            case "LKOH":
                return Issuer.Lukoil;
            case "MTSS":
                return Issuer.Mts;
            case "ROSN":
                return Issuer.Rosneft;
            case "RTKM":
                return Issuer.Rostelekom;
            case "SBER":
                return Issuer.SberbankAo;
            case "SBERP":
                return Issuer.SberbankAp;
            case "CHMF":
                return Issuer.Severstal;
            case "SNGS":
                return Issuer.SurgutNfgAo;
            case "SNGSP":
                return Issuer.SurgutNfgAp;
            case "VTBR":
                return Issuer.Vtb;
        }
        return Issuer.None;
    }
    
    /// <summary>
    /// Код эмитента
    /// </summary>
    public string Code
    {
        get;
        private set;
    }
    /// <summary>
    /// Наименование эмитента
    /// </summary>
    public string Name
    {
        get;
        private set;
    }

    private readonly Issuer _issuer;

    private readonly string _quotesFileName;
    private Dictionary <DateTime, Quote> _quotes;
    private Dictionary<DateTime, Deal> _deals;
    private Dictionary<DateTime, Deal> _simpleDeals;

    private const string DateCol = "C";
    private const string OpenCol = "E";
    private const string CloseCol = "H";
    private const string HighCol = "F";
    private const string LowCol = "G";
    private const string VolumeCol = "I";

    private const string DirectionDealCol = "K";
    private const string OpenDealCol = "L";
    private const string ReverseDealCol = "M";

    private const string NewDirectionDealCol = "O";
    private const string NewOpenDealCol = "P";
    private const string NewReverseDealCol = "Q";
    private const string NewProfitLostCol = "R";

    private const string SimpleDirectionDealCol = "T";
    private const string SimpleOpenDealCol = "U";
    private const string SimpleReverseDealCol = "V";
    private const string SimpleProfitLostCol = "W";

    private const int FirstRow = 2;

    private const string DateFormat = "ddMMyy";

    public TradeInstrument(string quotesFileName)
    {
        string shortFileName = Path.GetFileNameWithoutExtension(quotesFileName);
        string[] split = shortFileName.Split('_');
        Code = split[0].Trim();
        _issuer = GetIssuer(Code);
        Name = GetIssuerName(_issuer);
        _quotesFileName = quotesFileName;
    }

    public void ReadAllQuotes()
    {
        DateTime fDateTime = new DateTime(1, 1, 1);
        DateTime tDateTime = DateTime.Today;
        ReadQuotes(fDateTime, tDateTime);
    }

    public void ReadQuotes(DateTime fromDate, DateTime toDate)
    {
        using (ExcelClass xls = new ExcelClass())
        {
            try
            {
                _quotes = new Dictionary <DateTime, Quote>();
                xls.OpenDocument(_quotesFileName, false);
                string sDate = xls.GetCellStringValue(DateCol, FirstRow);
                int i = FirstRow;
                while (!string.IsNullOrEmpty(sDate))
                {
                    DateTime date = StringFunctions.GetDate(sDate, DateFormat);
                    if (date > fromDate && date < toDate)
                    {
                        double open = double.Parse(xls.GetCellStringValue(OpenCol, i));
                        double close = double.Parse(xls.GetCellStringValue(CloseCol, i));
                        double high = double.Parse(xls.GetCellStringValue(HighCol, i));
                        double low = double.Parse(xls.GetCellStringValue(LowCol, i));
                        ulong volume = ulong.Parse(xls.GetCellStringValue(VolumeCol, i));
                        Quote quote = new Quote(date, open, close, high, low, volume, i);
                        _quotes.Add(date, quote);
                    }
                    i++;
                    sDate = xls.GetCellStringValue(DateCol, i);
                }
            }
            finally 
            {
                xls.CloseDocument(false);
            }
        }
    }

    public void ReadAllRomanDeals()
    {
        DateTime fDateTime = new DateTime(1, 1, 1);
        DateTime tDateTime = DateTime.Today;
        ReadRomanDeals(fDateTime, tDateTime);
    }

    public void ReadAllSimpleDeals()
    {
        DateTime fDateTime = new DateTime(1, 1, 1);
        DateTime tDateTime = DateTime.Today;
        ReadSimpleDeals(fDateTime, tDateTime);
    }

    public Dictionary<DateTime, Deal> GetAllRomanDeals()
    {
        return _deals;
    }

    public Dictionary<DateTime, Deal> GetAllSimpleDeals()
    {
        return _simpleDeals;
    }

    public void WriteAllDeals()
    {
        using (ExcelClass xls = new ExcelClass())
        {
            try
            {
                xls.OpenDocument(_quotesFileName, false);
                int i = FirstRow;
                foreach (KeyValuePair<DateTime, Deal> pair in _deals)
                {
                    string sDate = xls.GetCellStringValue(DateCol, i);
                    while (!string.IsNullOrEmpty(sDate))
                    {
                        DateTime date = StringFunctions.GetDate(sDate, DateFormat);
                        if (date.AddDays(-1) == pair.Key)
                        {
                            SetDealStops
                                    (xls, ref i, pair.Value, NewDirectionDealCol, NewOpenDealCol,
                                     NewReverseDealCol, NewProfitLostCol);
                            break;
                        }
                        i++;
                        sDate = xls.GetCellStringValue(DateCol, i);
                    }
                }
            }
            finally
            {
                xls.CloseDocumentSave();
            }
        }
    }

    public void WriteSimpleDeals()
    {
        using (ExcelClass xls = new ExcelClass())
        {
            try
            {
                _simpleDeals = new Dictionary<DateTime, Deal>();
                xls.OpenDocument(_quotesFileName, false);
                string sDate = xls.GetCellStringValue(DateCol, FirstRow);
                int i = FirstRow;
                int iOpen = i;
                Deal deal = null;
                Deal romanDeal = null;
                bool isCurrentDeal = false;
                bool isFirstDeal = true;
                double? currentReverse = 0.0;
                double? stop;
                double? lastStop = 0;
                DateTime prevDay = new DateTime(1, 1, 1);

                foreach (KeyValuePair<DateTime, Quote> quote in _quotes)
                {
                    DateTime date = quote.Key;
                    if (!isCurrentDeal)
                    {
                        if (!_deals.ContainsKey(prevDay))
                        {
                            prevDay = date;
                            continue;
                        }


                        romanDeal = _deals[prevDay];
                        isCurrentDeal = true;
                        deal = new Deal(romanDeal.IsLong, romanDeal.OpenDate, romanDeal.OpenValue);
                    }

                    if (!isFirstDeal)
                    {
                        stop = romanDeal.GetStop(date);
                        if (stop == null)
                        {
                            deal.Close(prevDay, lastStop);
                            Quote quoteRow = _quotes[romanDeal.OpenDate];
                            int row = quoteRow.Row;
                            SetDealStops
                                    (xls, ref row, deal, SimpleDirectionDealCol, SimpleOpenDealCol,
                                     SimpleReverseDealCol, SimpleProfitLostCol);
                            _simpleDeals.Add(date, deal);
                            isCurrentDeal = false;
                        }
                        else
                        {
                            deal.SetStopReverse(date, stop);
                            if (deal.IsLong && quote.Value.Close < stop ||
                               !deal.IsLong && quote.Value.Close > stop)
                            {
                                Deal newDeal = deal.Reverse(date, quote.Value.Close);
                                Quote quoteRow = _quotes[romanDeal.OpenDate];
                                int row = quoteRow.Row;
                                SetDealStops
                                        (xls, ref row, deal, SimpleDirectionDealCol, SimpleOpenDealCol,
                                         SimpleReverseDealCol, SimpleProfitLostCol);
                                _simpleDeals.Add(date, deal);
                                deal = newDeal;
                                isCurrentDeal = false;
                            }
                        }
                        lastStop = stop;
                    }

                    prevDay = date;
                    isFirstDeal = false;
                }


            }
            finally
            {
                xls.CloseDocumentSave();
            }
        }
    }

    private void ReadRomanDeals(DateTime fromDate, DateTime toDate)
    {
        ReadDeals(ref _deals, fromDate, toDate, DirectionDealCol, OpenDealCol, ReverseDealCol);
    }

    private void ReadSimpleDeals(DateTime fromDate, DateTime toDate)
    {
        ReadDeals(ref _simpleDeals, fromDate, toDate, SimpleDirectionDealCol, SimpleOpenDealCol, SimpleReverseDealCol);
    }

    private void ReadDeals(ref Dictionary<DateTime, Deal> someDeals, DateTime fromDate, DateTime toDate, string directionDealCol, string openDealCol, string reverseDealCol)
    {
        using (ExcelClass xls = new ExcelClass())
        {
            try
            {
                someDeals = new Dictionary<DateTime, Deal>();
                xls.OpenDocument(_quotesFileName, false);
                string sDate = xls.GetCellStringValue(DateCol, FirstRow);
                int i = FirstRow;
                Deal deal = null;
                bool isFirstDeal = true;
                while (!string.IsNullOrEmpty(sDate))
                {
                    DateTime date = StringFunctions.GetDate(sDate, DateFormat);
                    if (date > fromDate && date < toDate)
                    {
                        string sDir = xls.GetCellStringValue(directionDealCol, i);
                        string sOpen = xls.GetCellStringValue(openDealCol, i);
                        string sReverse = xls.GetCellStringValue(reverseDealCol, i);
                        if (!string.IsNullOrEmpty(sDir) &&
                            !string.IsNullOrEmpty(sOpen) &&
                            !string.IsNullOrEmpty(sReverse))
                        {
                            double? reverse = StringFunctions.TryParse(sReverse);
                            if (isFirstDeal)
                            {
                                double? open = StringFunctions.TryParse(sOpen);
                                deal = new Deal(sDir, date.AddDays(-1), open);
                                deal.SetStopReverse(date, reverse);
                                isFirstDeal = false;
                            }
                            else
                            {
                                if (deal.IsSameDirection(sDir))
                                {
                                    deal.SetStopReverse(date, reverse);
                                }
                                else
                                {
                                    double? open = StringFunctions.TryParse(sOpen);
                                    someDeals.Add(deal.OpenDate, deal);
                                    deal = deal.Reverse(date.AddDays(-1), open);
                                    deal.SetStopReverse(date, reverse);
                                }
                            }
                        }
                    }
                    i++;
                    sDate = xls.GetCellStringValue(DateCol, i);
                }
            }
            finally
            {
                xls.CloseDocument(false);
            }
        }
    }

    private void SetDealStops(ExcelClass xls, ref int iRow, Deal deal, string directionCol, string openCol, string reverseCol, string profitLossCol)
    {
        foreach (KeyValuePair<DateTime, double?> stop in deal.Stops)
        {
            string sDate = xls.GetCellStringValue(DateCol, iRow);
            DateTime date = StringFunctions.GetDate(sDate, DateFormat);
            while (stop.Key != date)
            {
                iRow++;
                sDate = xls.GetCellStringValue(DateCol, iRow);
                date = StringFunctions.GetDate(sDate, DateFormat);
            }
            xls.SetCellValue(directionCol, iRow, deal.DirectionStr);
            xls.SetCellValue(openCol, iRow, deal.OpenValue.ToString());
            xls.SetCellValue(reverseCol, iRow, stop.Value.ToString());
            iRow++;
        }
        xls.SetCellValue(profitLossCol, iRow, deal.ProfitProcentStr);
    }

}

