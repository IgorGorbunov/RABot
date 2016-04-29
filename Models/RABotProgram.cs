using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;


public static class RABotProgram
{
    public static string TempFolder;

    private const string TempFolderName = "RAbot";
    private const string LittleTableFileName = "littleTable.txt";

    private const double Epsilon = 0.001;

    private static Dictionary<DateTime, Dictionary<TradeInstrument.Issuer, double>> _littleStops;
    private static Dictionary<DateTime, List<TableViewer>> _littleDeals;

    public static void SetTempFolder()
    {
        string temp = Path.GetTempPath();
        TempFolder = Path.Combine(temp, TempFolderName);
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

    public static void SetLittleStops(CollectionViewSource itemCollectionViewSource)
    {
        if (Clipboard.ContainsText())
        {
            Dictionary <TradeInstrument.Issuer, double> list = GetInstruments(Clipboard.GetText());
            FillLittleTable(list, itemCollectionViewSource);
            _littleStops.Add(DateTime.Today, list);
        }
    }

    public static KeyValuePair <DateTime, List<TableViewer>> GetLastLittleStops()
    {
        if (_littleDeals == null)
        {
            GetLittleStops();
        }
        return _littleDeals.Last();
    }

    public static void SaveLittleStops()
    {
        string filePath = Path.Combine(Application.StartupPath, LittleTableFileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        RecordLittleTable(filePath, DateTime.Today, _littleStops[DateTime.Today]);
    }

    public static void SetOpenValuesLittleTable(ref ObservableCollection <TableViewer> table)
    {
        using (QT qt = new QT())
        {
            qt.LuaConnect();

            foreach (TableViewer tableViewer in table)
            {
                qt.RegisterSecurity(TradeInstrument.GetIssuerCode(tableViewer.Instrument));
            }
            for (int i = 0; i < table.Count; i++)
            {
                decimal? openValue = qt.GetSecOpenVal
                        (TradeInstrument.GetIssuerCode(table[i].Instrument));
                if (openValue.HasValue)
                {
                    table[i].OpenValue = openValue.Value;
                }
            }
        }
    }






    private static void GetLittleStops()
    {
        _littleDeals = new Dictionary<DateTime, List<TableViewer>>();
        string filePath = Path.Combine(Application.StartupPath, LittleTableFileName);
        if (File.Exists(filePath))
        {
            TableViewer tbV = new TableViewer();
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
                        tbV = new TableViewer();
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
                        double profit = StringFunctions.ParseDouble(parametrs[4]);
                        if (Math.Abs(profit - 0) < Epsilon)
                        {
                            tbV.Profit = null;
                        }
                        else
                        {
                            tbV.Profit = profit;
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

    private static void RecordLittleTable(string filePath, DateTime date, Dictionary <TradeInstrument.Issuer, double> values)
    {
        using (StreamWriter streamWriter = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            streamWriter.WriteLine(date.ToShortDateString());
            foreach (KeyValuePair <TradeInstrument.Issuer, double> issuer in values)
            {
                string line = string.Format
                        ("{0};{1};{2};{3};{4}", TradeInstrument.GetIssuerName(issuer.Key), 0, 0.0, issuer.Value, 0);
                streamWriter.WriteLine(line);
            }
            streamWriter.Close();
        }
    }

    private static void FillLittleTable(Dictionary<TradeInstrument.Issuer, double> instrumentList, CollectionViewSource itemCollectionViewSource)
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
        itemCollectionViewSource.Source = deals;
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

