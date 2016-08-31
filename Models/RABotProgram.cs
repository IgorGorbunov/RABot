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

        public static void SetFreeMoney()
        {
            ViewModels.MoneyPurse.SetLocalFreeMoney();
        }
    }
}

