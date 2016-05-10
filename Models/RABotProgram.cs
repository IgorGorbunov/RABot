using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RABot.VM_Little_table;

namespace RABot.Models
{
    public static class RaBotProgram
    {
        public static QT Qt;

        public static string TempFolder;
        
        
        private const string LittleTableFileName = "littleTable.txt";
        private const string LittleCopyTableFileName = "littleTableCopy.txt";

        private static Dictionary<DateTime, List<TableViewer>> _littleDeals;

        public static void SetTempFolder()
        {
            string temp = Path.GetTempPath();
            TempFolder = Path.Combine(temp, Config.TempFolderName);
            DeleteTempFolder();
            Directory.CreateDirectory(TempFolder);
        }

        public static void DeleteTempFolder()
        {
            if (Directory.Exists(TempFolder))
            {
                try
                {
                    Directory.Delete(TempFolder, true);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                    throw;
                }
            
            }
        }

        public static string UnloadImage(Bitmap bitmap, string name)
        {
            string path = Path.Combine(TempFolder, name + ".png");
            if (!File.Exists(path))
            {
                bitmap.Save(path, ImageFormat.Png);
            }
            return path;
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

        public static KeyValuePair <DateTime, List<TableViewer>> GetLastLittleStops()
        {
            if (_littleDeals == null)
            {
                GetLittleStops();
            }
            return _littleDeals.Last();
        }

        public static void SetOpenValuesLittleTable(ref ObservableCollection <TableViewer> table)
        {
            Qt = new QT();
            Qt.LuaConnect();
            try
            {
                foreach (TableViewer tableViewer in table)
                {
                    Qt.RegisterSecurity(TradeInstrument.GetIssuerCode(tableViewer.Instrument));
                }
                for (int i = 0; i < table.Count; i++)
                {
                    decimal? openValue = Qt.GetSecOpenVal
                            (TradeInstrument.GetIssuerCode(table[i].Instrument));
                    if (openValue.HasValue)
                    {
                        table[i].OpenValue = openValue.Value;
                    }
                }
            }
            finally
            {
                Qt.LuaDisconnect();
            }
        }

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
                    (Application.StartupPath, Config.MiscFolderName, LittleCopyTableFileName);
            using (StreamWriter sw = new StreamWriter(newFilePath, false, Encoding.UTF8))
            {
                foreach (KeyValuePair<DateTime, List<TableViewer>> dayLittleTable in _littleDeals)
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
                    (Application.StartupPath, Config.MiscFolderName, LittleTableFileName);
            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }
            File.Move(newFilePath, oldFilePath);

        }

        public static void GetLittleStops()
        {
            _littleDeals = new Dictionary<DateTime, List<TableViewer>>();
            string filePath = Path.Combine(Application.StartupPath, Config.MiscFolderName, LittleTableFileName);
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
                            int isLong = int.Parse(parametrs[1]);
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
                            if (string.IsNullOrEmpty(parametrs[4]))
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

        public static void SetFreeMoney()
        {
            ViewModels.MoneyPurse.SetLocalFreeMoney();
        }


        private static ObservableCollection <TableViewer> FillLittleTable(Dictionary<TradeInstrument.Issuer, double> instrumentList)
        {
            List<TableViewer> deals = new List<TableViewer>();
            foreach (KeyValuePair<TradeInstrument.Issuer, double> dealProps in instrumentList)
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

        private static Dictionary<TradeInstrument.Issuer, double> GetInstruments(string text)
        {
            Dictionary<TradeInstrument.Issuer, double> dictionary = new Dictionary<TradeInstrument.Issuer, double>();
            string[] lineSplit = text.Split('\n', '\r');
            bool isFutures = true;
            foreach (string line in lineSplit)
            {
                if (!string.IsNullOrEmpty(line))
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
    }
}

