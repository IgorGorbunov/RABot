using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;
using MessageBox = System.Windows.Forms.MessageBox;

namespace RABot
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private NotifyIcon _ni;
        private BackgroundWorker _netConnectionWorker;

        private void SetIcon()
        {
            _ni = new NotifyIcon
            {
                Icon = Properties.Resources.chart,
                Visible = true
            };
            _ni.DoubleClick += delegate
            {
                Show();
                WindowState = WindowState.Normal;
            };

            MenuItem[] menuItems = new MenuItem[1];
            MenuItem menuItem = new MenuItem();
            menuItem.Index = 0;
            menuItem.Text = "Выход";
            menuItem.Click += menuExit_Click;
            menuItems[0] = menuItem;

            _ni.ContextMenu = new ContextMenu(menuItems);
        }
        private void StartPing()
        {
            _netConnectionWorker = new BackgroundWorker();
            _netConnectionWorker.WorkerReportsProgress = true;
            _netConnectionWorker.DoWork += NetConnectionWorker_DoWork;
            _netConnectionWorker.RunWorkerCompleted += NetConnectionWorker_RunWorkerCompleted;
            _netConnectionWorker.ProgressChanged += NetConnectionWorker_ProgressChanged;

            if (_netConnectionWorker.IsBusy != true)
            {
                _netConnectionWorker.RunWorkerAsync();
            }
        }


        private void NetConnectionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (true)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(500);
                    Ping pingSender = new Ping();
                    bool netAccess = true, slAccess = true;
                    try
                    {
                        PingReply reply = pingSender.Send("ya.ru");

                        if (reply.Status == IPStatus.Success)
                        {
                            Debug.WriteLine("Address: {0}", reply.Address);
                            Debug.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                            Debug.WriteLine("Time to live: {0}", reply.Options.Ttl);
                            Debug.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                            Debug.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                        }
                        else
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
                        PingReply reply = pingSender.Send("smart-lab.ru");

                        if (reply.Status == IPStatus.Success)
                        {
                            Debug.WriteLine("Address: {0}", reply.Address);
                            Debug.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                            Debug.WriteLine("Time to live: {0}", reply.Options.Ttl);
                            Debug.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                            Debug.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                        }
                        else
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
                }
                System.Threading.Thread.Sleep(5000);
            }
        }

        // This event handler updates the progress.
        private void NetConnectionWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 0:
                    ImgNetConn.Source = SetImgNetConn(false);
                    ImgSlConn.Source = SetImgNetConn(false);
                    break;
                case 1:
                    ImgNetConn.Source = SetImgNetConn(false);
                    ImgSlConn.Source = SetImgNetConn(true);
                    break;
                case 2:
                    ImgNetConn.Source = SetImgNetConn(true);
                    ImgSlConn.Source = SetImgNetConn(false);
                    break;
                case 3:
                    ImgNetConn.Source = SetImgNetConn(true);
                    ImgSlConn.Source = SetImgNetConn(true);
                    break;
            }
        }

        private void NetConnectionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        private BitmapImage SetImgNetConn(bool notBusy)
        {
            string imgPath;
            if (notBusy)
            {
                imgPath = @"D:\Work\Coding\Github\repos\RABot\Resources\notBusy.png";
            }
            else
            {
                imgPath = @"D:\Work\Coding\Github\repos\RABot\Resources\busy.png";
            }
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(imgPath);
            b.EndInit();
            return b;
        }

        private void CleanUp()
        {
            if (_netConnectionWorker.WorkerSupportsCancellation == true)
            {
                _netConnectionWorker.CancelAsync();
            }
            _ni.Dispose();
        }

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                SetIcon();
                StartPing();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                
            }
            
        }

        private void Window_StateChanged_1(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            CleanUp();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BttnTest_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
