using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RABot.VM_Little_table;

namespace RABot.Models
{
    public static class LittleTable
    {
        private const string Folder = "littleTable";
        private const string FileName = "littleTable.txt";
        private const string CopyFileName = "littleTableCopy.txt";
        private const string CurrentDeals = "littleTableCurrentDeals.txt";

        private static Dictionary <DateTime, List <TableViewer>> _littleDeals;
        private static List <Deal> _currentDeals; 

        public static void AppendLittleTable(DateTime dateTime, ObservableCollection <TableViewer> table)
        {
            List <TableViewer> littleTable = _littleDeals[dateTime.Date];
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
                foreach (KeyValuePair <DateTime, List <TableViewer>> dayLittleTable in _littleDeals)
                {
                    sw.WriteLine(dayLittleTable.Key.ToShortDateString());
                    foreach (TableViewer dealParams in dayLittleTable.Value)
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

        public static void SetOpenValuesLittleTable(ref ObservableCollection<TableViewer> table)
        {
            RaBotProgram.Qt = new QT();
            RaBotProgram.Qt.LuaConnect();
            try
            {
                foreach (TableViewer tableViewer in table)
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
            _littleDeals = new Dictionary <DateTime, List <TableViewer>>();
            string filePath = Path.Combine(Application.StartupPath, Config.MiscFolderName, Folder, FileName);
            if (File.Exists(filePath))
            {
                List <TableViewer> deals = new List <TableViewer>();
                DateTime date = DateTime.Today;
                using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        string[] parametrs = line.Split(';');
                        if (parametrs.Length > 1)
                        {
                            TableViewer tbV = new TableViewer();
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

        public static void GetCurrentDeals()
        {
            _currentDeals = new List <Deal>();
            StreamReader streamReader = new StreamReader
                    (Path.Combine(Application.StartupPath, Config.MiscFolderName, Folder, CurrentDeals),
                     Encoding.UTF8);
            try
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        break;
                    string[] split = line.Split(';');
                    DateTime date = StringFunctions.GetDate(split[0], "dd.MM.yyyy");
                    TradeInstrument.Issuer issuer = TradeInstrument.GetIssuer(split[1]);
                    decimal openVal = StringFunctions.ParseDecimal(split[2]);
                    string dir = split[3];
                    int vol = int.Parse(split[4]);

                    Deal deal = new Deal(issuer, dir, date, openVal, vol);
                    _currentDeals.Add(deal);
                }

            }
            finally
            {
                streamReader.Close();
            }
        }

        public static KeyValuePair <DateTime, List <TableViewer>> GetLastLittleStops()
        {
            if (_littleDeals == null)
            {
                GetLittleStops();
            }
            return _littleDeals.Last();
        }

        public static ObservableCollection <TableViewer> SetLittleStops()
        {
            if (Clipboard.ContainsText())
            {
                Dictionary <TradeInstrument.Issuer, double> list = GetInstruments(Clipboard.GetText());
                ObservableCollection <TableViewer> littleTable = FillLittleTable(list);
                _littleDeals.Add(DateTime.Today, littleTable.ToList());
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

        private static ObservableCollection <TableViewer> FillLittleTable(Dictionary <TradeInstrument.Issuer, double> instrumentList)
        {
            List <TableViewer> deals = new List <TableViewer>();
            foreach (KeyValuePair <TradeInstrument.Issuer, double> dealProps in instrumentList)
            {
                TableViewer dealParams = new TableViewer();
                dealParams.Instrument = dealProps.Key;
                dealParams.IsLong = false;
                dealParams.OpenValue = 0.0m;
                dealParams.StopValue = (decimal)dealProps.Value;
                dealParams.Profit = null;
                deals.Add(dealParams);
            }
            return new ObservableCollection <TableViewer>(deals);
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
                             (Application.StartupPath, Config.MiscFolderName, Folder, CurrentDeals), false, Encoding.UTF8);
            try
            {
                foreach (Deal deal in _currentDeals)
                {
                    streamWriter.WriteLine
                            ("{0};{1};{2};{3};{4}", deal.OpenDate.ToShortDateString(), TradeInstrument.GetIssuerCode(deal.Issuer),
                             deal.OpenValue, deal.DirectionStr, deal.Volume);
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
