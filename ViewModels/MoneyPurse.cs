using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RABot.ViewModels
{
    public class MoneyPurse
    {
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
        }

        public static event EventHandler FreeMoneyChanged;

        public static void RaiseFreeMoneyChanged()
        {
            EventHandler handler = FreeMoneyChanged;
            if (handler != null)
                handler(null, EventArgs.Empty);
        }

    }
}
