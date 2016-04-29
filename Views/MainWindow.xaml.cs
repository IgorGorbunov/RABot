using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                SetIcon();
                RABotProgram.SetTempFolder();
                Timers.InitTimers(LabelMoexTime, LabelLocalTime);
                NetClass.StartPing(ImgNetConn, ImgSlConn);
                SetLastLittleTable();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                
            }
        }

        

        private void CleanUp()
        {
            NetClass.DisposeAsync();
            RABotProgram.DeleteTempFolder();
            _ni.Dispose();
        }

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

        private void SetLastLittleTable()
        {
            DgLittleTable.ItemsSource = RABotProgram.GetLastLittleStops().Value;
        }

        //------------------------------------------------------------------------------------


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
            using (QT trader = new QT("127.0.0.1:5001", "quikBot", "quik_RomaN"))
            {
                trader.LuaConnect();
                trader.RegisterSecurity("SBER");
                MessageBox.Show(trader.GetSecOpenVal("SBER").ToString());
            }
        }

        private void BttnAddLittleStops_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSourceLittleTable"));
            RABotProgram.SetLittleStops(itemCollectionViewSource);
            RABotProgram.SaveLittleStops();
        }

        private void DgLittleTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            TableViewer item = e.Row.Item as TableViewer;
            if (item != null)
            {
                if (item.IsNullValue())
                {
                    e.Row.Background = System.Windows.Media.Brushes.Yellow;
                }
                else
                {
                    e.Row.Background = System.Windows.Media.Brushes.White;
                }
            }
        }


    }
}
