using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using RABot.Models;

public static class NetClass
{
    private static BackgroundWorker _netConnectionWorker;
    private static System.Windows.Controls.Image _img1, _img2;

    private const string Site1 = "mail.ru";
    private const string Site2 = "smart-lab.ru";

    public static void StartPing(System.Windows.Controls.Image img1, System.Windows.Controls.Image img2)
    {
        _img1 = img1;
        _img2 = img2;
        _netConnectionWorker = new BackgroundWorker
        {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
        };
        _netConnectionWorker.DoWork += NetConnectionWorker_DoWork;
        _netConnectionWorker.RunWorkerCompleted += NetConnectionWorker_RunWorkerCompleted;
        _netConnectionWorker.ProgressChanged += NetConnectionWorker_ProgressChanged;

        if (_netConnectionWorker.IsBusy != true)
        {
            _netConnectionWorker.RunWorkerAsync();
        }
    }

    public static void DisposeAsync()
    {
        StopAndWaitAsync();
        _netConnectionWorker.ProgressChanged -= NetConnectionWorker_ProgressChanged;
        _netConnectionWorker.RunWorkerCompleted -= NetConnectionWorker_RunWorkerCompleted;
        _netConnectionWorker.DoWork -= NetConnectionWorker_DoWork;
        _netConnectionWorker.Dispose();
    }


    private static void NetConnectionWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        BackgroundWorker worker = sender as BackgroundWorker;

        while (true)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                break;
            }
            // Perform a time consuming operation and report progress.
            System.Threading.Thread.Sleep(500);
            Ping pingSender = new Ping();
            bool netAccess = true, slAccess = true;
            try
            {
                PingReply reply = pingSender.Send(Site1);
                if (reply.Status != IPStatus.Success)
                {
                    netAccess = false;
                }
            }
            catch (PingException ex)
            {
                netAccess = false;
                Debug.WriteLine(ex.ToString());
            }
            try
            {
                PingReply reply = pingSender.Send(Site2);
                if (reply.Status != IPStatus.Success)
                {
                    slAccess = false;
                }
            }
            catch (PingException ex)
            {
                slAccess = false;
                Debug.WriteLine(ex.ToString());
            }
            if (!netAccess && !slAccess)
            {
                worker.ReportProgress(0);
            }
            if (!netAccess && slAccess)
            {
                worker.ReportProgress(1);
            }
            if (netAccess && !slAccess)
            {
                worker.ReportProgress(2);
            }
            if (netAccess && slAccess)
            {
                worker.ReportProgress(3);
            }
            System.Threading.Thread.Sleep(2000);
        }
    }

    // This event handler updates the progress.
    private static void NetConnectionWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        switch (e.ProgressPercentage)
        {
            case 0:
                _img1.Source = SetImgNetConn(false);
                _img2.Source = SetImgNetConn(false);
                break;
            case 1:
                _img1.Source = SetImgNetConn(false);
                _img2.Source = SetImgNetConn(true);
                break;
            case 2:
                _img1.Source = SetImgNetConn(true);
                _img2.Source = SetImgNetConn(false);
                break;
            case 3:
                _img1.Source = SetImgNetConn(true);
                _img2.Source = SetImgNetConn(true);
                break;
        }
    }

    private static void NetConnectionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Cancelled)
        {
            Debug.WriteLine("Canceled!");
        }
        else if (e.Error != null)
        {
            Debug.WriteLine("Error: " + e.Error.Message);
        }
        else
        {
            Debug.WriteLine("Done!");
        }
    }

    private static void StopAndWaitAsync()
    {
        if (_netConnectionWorker.WorkerSupportsCancellation)
        {
            _netConnectionWorker.CancelAsync();
            while (_netConnectionWorker.IsBusy)
            {
                System.Threading.Thread.Sleep(500);
                Application.DoEvents();
            }
        }
    }

    private static BitmapImage SetImgNetConn(bool notBusy)
    {
        string imgPath;
        if (notBusy)
        {
            using (Bitmap bit = RABot.Properties.Resources.notBusy)
            {
                //метод просто выгружает указанную картинку в папку temp и возвращает полный путь к ней
                imgPath = RaBotProgram.UnloadImage(bit, "notBusy");
            }
        }
        else
        {
            using (Bitmap bit = RABot.Properties.Resources.busy)
            {
                imgPath = RaBotProgram.UnloadImage(bit, "busy");
            }
        }

        var stream = File.OpenRead(imgPath);
        BitmapImage b = new BitmapImage();
        b.BeginInit();
        b.CacheOption = BitmapCacheOption.OnLoad;
        b.StreamSource = stream;
        b.EndInit();
        stream.Close();
        return b;
    }


}

