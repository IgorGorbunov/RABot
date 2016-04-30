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
    public static class XlsDocs
    {
        private const string LittleTemplateFileName = "littleTemplate.xls";
        private const string FolderName = "xls";

        public static void SaveRomanIssuers(ObservableCollection <TableViewer> list)
        {
            foreach (TableViewer tableViewer in list)
            {
                if (tableViewer.OpenValue > 0)
                {
                    SetRomanIssuer(tableViewer);
                }
            }
        }

        public static void SaveQuotes(ObservableCollection<TableViewer> list)
        {
            RaBotProgram.qt.LuaConnect();
            try
            {
                foreach (TableViewer tableViewer in list)
                {
                    if (tableViewer.OpenValue > 0)
                    {
                        string code = TradeInstrument.GetIssuerCode(tableViewer.Instrument);
                        Quote quote = RaBotProgram.qt.GetQuote(code);
                        SetQuote(quote, code);
                    }
                }
            }
            finally
            {
                RaBotProgram.qt.LuaDisconnect();
            }
        }

        private static void SetRomanIssuer(TableViewer props)
        {
            string shortFileName = TradeInstrument.GetIssuerCode(props.Instrument);
            string fullPath = SetFile(shortFileName);

            using (ExcelClass xls = new ExcelClass())
            {
                xls.OpenDocument(fullPath, false);
                int firstFreeRow = int.Parse(xls.GetCellStringValue(1, 1));

                xls.SetCellValue(9, firstFreeRow, Deal.GetDirection(props.IsLong));
                xls.SetCellValue(10, firstFreeRow, props.OpenValue.ToString());
                xls.SetCellValue(11, firstFreeRow, props.StopValue.ToString());
                xls.SetCellValue(12, firstFreeRow, props.Profit.ToString());
                xls.CloseDocumentSave();
            }

        }

        private static void SetQuote(Quote quote, string code)
        {
            string fullPath = SetFile(code);

            using (ExcelClass xls = new ExcelClass())
            {
                xls.OpenDocument(fullPath, false);
                int firstFreeRow = int.Parse(xls.GetCellStringValue(1, 1));
                xls.SetCellValue(1, firstFreeRow, quote.Date.ToShortDateString());
                xls.SetCellValue(2, firstFreeRow, quote.Open.ToString());
                xls.SetCellValue(3, firstFreeRow, quote.Close.ToString());
                xls.SetCellValue(4, firstFreeRow, quote.Low.ToString());
                xls.SetCellValue(5, firstFreeRow, quote.High.ToString());
                xls.SetCellValue(6, firstFreeRow, quote.Volume.ToString());
                xls.SetCellValue(7, firstFreeRow, quote.Lot.ToString());
                
                xls.SetCellValue(1, 1, (++firstFreeRow).ToString());
                xls.CloseDocumentSave();
            }

        }

        private static string SetFile(string shortFileName)
        {
            string miscFolder = Path.Combine(Application.StartupPath, RaBotProgram.MiscFolderName);
            if (!Directory.Exists(miscFolder))
            {
                Directory.CreateDirectory(miscFolder);
            }
            string xlsFolder = Path.Combine(miscFolder, FolderName);
            if (!Directory.Exists(xlsFolder))
            {
                Directory.CreateDirectory(xlsFolder);
            }
            string xlsFile = Path.Combine
                    (xlsFolder, shortFileName + ".xls");
            if (!File.Exists(xlsFile))
            {
                File.Copy(Path.Combine(miscFolder, LittleTemplateFileName), xlsFile);
            }
            return xlsFile;
        }

    }
}
