using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RABot.VM_Little_table;
using RABot.ViewModels;

namespace RABot.Models
{
    public static class LittleTable
    {
        public static List<LittleDealsViewer> CurrentDealViewers
        {
            get
            {
                if (_currentDeals == null || _currentDeals.Count < 1)
                {
                    ReadCurrentDeals();
                }
                List <LittleDealsViewer> list = new List <LittleDealsViewer>();
                foreach (Deal deal in _currentDeals)
                {
                    LittleDealsViewer dealsViewer = new LittleDealsViewer();
                    dealsViewer.InstrumentName = TradeInstrument.GetIssuerName(deal.Issuer);
                    dealsViewer.IsLong = deal.IsLong;
                    dealsViewer.OpenDate = deal.OpenDate;
                    dealsViewer.OpenValue = deal.OpenValue.Value;
                    dealsViewer.Volume = deal.Volume;
                    dealsViewer.Value = deal.Volume*deal.OpenValue.Value;
                    dealsViewer.LastStopValue = deal.LastStopValue;
                    dealsViewer.LastStopDate = deal.LastStopDate;
                    list.Add(dealsViewer);
                }
                return list;
            }
        }

        private const string Folder = "littleTable";
        private const string FileName = "littleTable.txt";
        private const string CopyFileName = "littleTableCopy.txt";
        private const string CurrentDealsFileName = "littleTableCurrentDeals.txt";

        private static Dictionary <DateTime, List <LittleTableViewer>> _littleDeals;
        private static List <Deal> _currentDeals; 

        public static decimal GetShortSum()
        {
            if (_littleDeals == null || _littleDeals.Count < 1)
            {
                return 0.0m;
            }
            decimal shortSum = 0.0m;
            foreach (Deal currentDeal in _currentDeals)
            {
                if (!currentDeal.IsLong)
                {
                    shortSum += currentDeal.OpenValue.Value*currentDeal.Volume;
                }
            }
            return shortSum;
        }

        public static void AppendLittleTable(DateTime dateTime, ObservableCollection <LittleTableViewer> table)
        {
            List <LittleTableViewer> littleTable = _littleDeals[dateTime.Date];
            {
                for (int i = 0; i < littleTable.Count; i++)
                {
                    littleTable[i].Instrument = table[i].Instrument;
                    littleTable[i].IsLong = table[i].IsLong;
                    littleTable[i].OpenValue = table[i].OpenValue;
                    littleTable[i].StopValue = table[i].StopValue;
                    littleTable[i].Profit = table[i].Profit;
                }
            }
            string newFilePath = Path.Combine
                    (Application.StartupPath, Config.MiscFolderName, Folder, CopyFileName);
            using (StreamWriter sw = new StreamWriter(newFilePath, false, Encoding.UTF8))
            {
                foreach (KeyValuePair <DateTime, List <LittleTableViewer>> dayLittleTable in _littleDeals)
                {
                    sw.WriteLine(dayLittleTable.Key.ToShortDateString());
                    foreach (LittleTableViewer dealParams in dayLittleTable.Value)
                    {
                        sw.Write(TradeInstrument.GetIssuerName(dealParams.Instrument));
                        sw.Write(';');
                        if (dealParams.IsLong)
                            sw.Write(1);
                        else
                            sw.Write(0);
                        sw.Write(';');
                        sw.Write(dealParams.OpenValue);
                        sw.Write(';');
                        sw.Write(dealParams.StopValue);
                        sw.Write(';');
                        sw.WriteLine(dealParams.Profit);
                    }
                    sw.Flush();
                }
                sw.Close();
            }
            string oldFilePath = Path.Combine
                    (Application.StartupPath, Config.MiscFolderName, Folder, FileName);
            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }
            File.Move(newFilePath, oldFilePath);

        }

        public static void SetOpenValuesLittleTable(ref ObservableCollection<LittleTableViewer> table)
        {
            RaBotProgram.Qt = new QT();
            RaBotProgram.Qt.LuaConnect();
            try
            {
                foreach (LittleTableViewer tableViewer in table)
                {
                    RaBotProgram.Qt.RegisterSecurity(TradeInstrument.GetIssuerCode(tableViewer.Instrument));
                }
                for (int i = 0; i < table.Count; i++)
                {
                    decimal? openValue = RaBotProgram.Qt.GetSecOpenVal
                            (TradeInstrument.GetIssuerCode(table[i].Instrument));
                    if (openValue.HasValue)
                    {
                        table[i].OpenValue = openValue.Value;
                    }
                }
            }
            finally
            {
                RaBotProgram.Qt.LuaDisconnect();
            }
        }

        public static void GetLittleStops()
        {
            _littleDeals = new Dictionary <DateTime, List <LittleTableViewer>>();
            string filePath = Path.Combine(Application.StartupPath, Config.MiscFolderName, Folder, FileName);
            if (File.Exists(filePath))
            {
                List <LittleTableViewer> deals = new List <LittleTableViewer>();
                DateTime date = DateTime.Today;
                using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        string[] parametrs = line.Split(';');
                        if (parametrs.Length > 1)
                        {
                            LittleTableViewer tbV = new LittleTableViewer();
                            tbV.Instrument = TradeInstrument.GetIssuer2Name(parametrs[0]);
                            int isLong = Int32.Parse(parametrs[1]);
                            switch (isLong)
                            {
                                case 0:
                                    tbV.IsLong = false;
                                    break;
                                case 1:
                                    tbV.IsLong = true;
                                    break;
                            }
                            tbV.OpenValue = StringFunctions.ParseDecimal(parametrs[2]);
                            tbV.StopValue = StringFunctions.ParseDecimal(parametrs[3]);
                            if (String.IsNullOrEmpty(parametrs[4]))
                            {
                                tbV.Profit = null;
                            }
                            else
                            {
                                tbV.Profit = StringFunctions.ParseDouble(parametrs[4]);
                            }
                            deals.Add(tbV);
                        }
                        else
                        {
                            if (deals.Count > 0)
                            {
                                _littleDeals.Add(date, deals);
                                deals.Clear();
                            }
                            date = StringFunctions.GetDate(line, "dd.MM.yyyy");
                        }
                    }
                    _littleDeals.Add(date, deals);
                }
            }
        }

        public static void ReadCurrentDeals()
        {
            _currentDeals = new List <Deal>();
            StreamReader streamReader = new StreamReader
                    (Path.Combine(Application.StartupPath, Config.MiscFolderName, Folder, CurrentDealsFileName),
                     Encoding.UTF8);
            try
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        break;
                    string[] split = line.Split(';');
                    DateTime date = DateTime.ParseExact(split[0], "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    TradeInstrument.Issuer issuer = TradeInstrument.GetIssuer(split[1]);
                    decimal openVal = StringFunctions.ParseDecimal(split[2]);
                    string dir = split[3];
                    int vol = int.Parse(split[4]);
                    decimal stopVal = StringFunctions.ParseDecimal(split[5]);
                    DateTime stopDate = DateTime.ParseExact(split[6], "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    Deal deal = new Deal(issuer, dir, date, openVal, vol, stopVal, stopDate);
                    _currentDeals.Add(deal);
                }

            }
            finally
            {
                streamReader.Close();
            }
        }

        public static KeyValuePair <DateTime, List <LittleTableViewer>> GetLastLittleStops()
        {
            if (_littleDeals == null)
            {
                GetLittleStops();
            }
            return _littleDeals.Last();
        }

        public static ObservableCollection <LittleTableViewer> SetLittleStops()
        {
            if (Clipboard.ContainsText())
            {
                ObservableCollection <LittleTableViewer> littleTable;
                if (_littleDeals.ContainsKey(DateTime.Today))
                {
                    MessageBox.Show("Таблица на сегодня уже записана!");
                    littleTable = new ObservableCollection <LittleTableViewer>
                            (_littleDeals[DateTime.Today]);
                }
                else
                {
                    Dictionary<TradeInstrument.Issuer, double> list = GetInstruments(Clipboard.GetText());
                    littleTable = FillLittleTable(list);
                    _littleDeals.Add(DateTime.Today, littleTable.ToList());
                }
                return littleTable;
            }
            return null;
        }

        public static void SetNewDeal(Deal newDeal)
        {
            int iDel = 0;
            bool found = false;
            for (int i = 0; i < _currentDeals.Count; i++)
            {
                if (_currentDeals[i].Issuer == newDeal.Issuer)
                {
                    iDel = i;
                    found = true;
                    break;
                }
            }
            if (found)
            {
                _currentDeals.RemoveAt(iDel);
            }
            _currentDeals.Add(newDeal);
            SaveCurrentDeals();
        }

        private static decimal GetTodeyStopValue(TradeInstrument.Issuer issuer)
        {
            if (!_littleDeals.ContainsKey(DateTime.Today))
                return -1m;

            List <LittleTableViewer> list = _littleDeals[DateTime.Today];
            foreach (LittleTableViewer littleTableViewer in list)
            {
                if (littleTableViewer.Instrument != issuer)
                    continue;
                return littleTableViewer.StopValue;
            }
            return -1m;
        }

        private static ObservableCollection <LittleTableViewer> FillLittleTable(Dictionary <TradeInstrument.Issuer, double> instrumentList)
        {
            List <LittleTableViewer> deals = new List <LittleTableViewer>();
            foreach (KeyValuePair <TradeInstrument.Issuer, double> dealProps in instrumentList)
            {
                LittleTableViewer dealParams = new LittleTableViewer();
                dealParams.Instrument = dealProps.Key;
                dealParams.IsLong = false;
                dealParams.OpenValue = 0.0m;
                dealParams.StopValue = (decimal)dealProps.Value;
                dealParams.Profit = null;
                deals.Add(dealParams);
            }
            return new ObservableCollection <LittleTableViewer>(deals);
        }

        private static Dictionary <TradeInstrument.Issuer, double> GetInstruments(string text)
        {
            Dictionary <TradeInstrument.Issuer, double> dictionary = new Dictionary <TradeInstrument.Issuer, double>();
            string[] lineSplit = text.Split('\n', '\r');
            bool isFutures = true;
            foreach (string line in lineSplit)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    string lineWoCommas = line.Trim(':', ';');
                    if (lineWoCommas == SLSettings.FuturesName)
                    {
                        isFutures = true;
                        continue;
                    }
                    if (lineWoCommas == SLSettings.StocksName)
                    {
                        isFutures = false;
                        continue;
                    }
                    string[] issuerAndStop = lineWoCommas.Split(' ');
                    TradeInstrument.Issuer issuer = TradeInstrument.GetIssuerRa(issuerAndStop[0], isFutures);
                    dictionary.Add(issuer, StringFunctions.ParseDouble(issuerAndStop[1]));
                }

            }
            return dictionary;
        }

        private static void SaveCurrentDeals()
        {
            StreamWriter streamWriter = new StreamWriter
                    (Path.Combine
                             (Application.StartupPath, Config.MiscFolderName, Folder, CurrentDealsFileName), false, Encoding.UTF8);
            try
            {
                foreach (Deal deal in _currentDeals)
                {
                    streamWriter.WriteLine
                            ("{0};{1};{2};{3};{4};{5};{6}", deal.OpenDate.ToString("dd.MM.yyyy HH:mm:ss"), TradeInstrument.GetIssuerCode(deal.Issuer),
                             deal.OpenValue, deal.DirectionStr, deal.Volume, deal.LastStopValue, deal.LastStopDate.ToString("dd.MM.yyyy HH:mm:ss"));
                }
                streamWriter.Flush();
            }
            finally
            {
                streamWriter.Close();
            }
        }
    }
}
