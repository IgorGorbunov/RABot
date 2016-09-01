using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using RABot.Models;

namespace RABot.ViewModels
{
    public class MoneyPurse
    {
        private static decimal _freeMoneyWithShort;
        private static decimal _freeMoney;

        public static decimal FreeMoney
        {
            get { return _freeMoney; }
            private set
            {
                _freeMoney = value;
                RaiseFreeMoneyChanged();
            }
        }

        public static decimal FreeMoneyWithShort
        {
            get { return _freeMoneyWithShort; }
            private set
            {
                _freeMoneyWithShort = value;
                RaiseFreeMoneyWithShortChanged();
            }
        }

        public static void SetLocalFreeMoney()
        {
            string miscFolder = Path.Combine(Application.StartupPath, Config.MiscFolderName);
            if (!Directory.Exists(miscFolder))
            {
                Directory.CreateDirectory(miscFolder);
            }
            string file = Path.Combine
                    (miscFolder, Config.FreeMoneyFileName);
            if (File.Exists(file))
            {
                using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
                {
                    string line = reader.ReadLine();
                    decimal val = StringFunctions.ParseDecimal(line);
                    FreeMoney = val;
                }
            }
            FreeMoneyWithShort = FreeMoney - LittleTable.GetShortSum();
        }

        public static event EventHandler FreeMoneyChanged;

        public static event EventHandler FreeMoneyWithShortChanged;

        private static void RaiseFreeMoneyChanged()
        {
            EventHandler handler = FreeMoneyChanged;
            if (handler != null)
                handler(null, EventArgs.Empty);
        }

        private static void RaiseFreeMoneyWithShortChanged()
        {
            EventHandler handler = FreeMoneyWithShortChanged;
            if (handler != null)
                handler(null, EventArgs.Empty);
        }

    }
}
