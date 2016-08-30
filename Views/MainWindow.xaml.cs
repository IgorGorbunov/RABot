using System;
using System.Collections.ObjectModel;
using System.Windows;
using RABot.Models;
using RABot.VM_Little_table;
using Forms = System.Windows.Forms;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;

namespace RABot
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Forms.NotifyIcon _ni;
        private ObservableCollection <TableViewer> _currentLittleTable; 

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                SetIcon();
                RaBotProgram.SetTempFolder();
                Timers.InitTimers(LabelMoexTime, LabelLocalTime);
                NetClass.StartPing(ImgNetConn, ImgSlConn);
                LittleTable.GetLittleStops();
                RaBotProgram.SetFreeMoney();
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
            RaBotProgram.Qt.Dispose();
            NetClass.DisposeAsync();
            RaBotProgram.DeleteTempFolder();
            _ni.Dispose();
        }

        private void SetIcon()
        {
            _ni = new Forms.NotifyIcon
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
            _currentLittleTable = new ObservableCollection <TableViewer>
                    (LittleTable.GetLastLittleStops().Value);
            DgLittleTable.ItemsSource = _currentLittleTable;
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
            QT qt = new QT();
            qt.LuaConnect();
            try
            {

            }
            finally
            {
                qt.LuaDisconnect();
            }
        }

        private void BttnAddLittleStops_Click(object sender, RoutedEventArgs e)
        {
            _currentLittleTable = new ObservableCollection<TableViewer>
                    (LittleTable.SetLittleStops());
            DgLittleTable.ItemsSource = _currentLittleTable;
            LittleTable.AppendLittleTable(DateTime.Today, _currentLittleTable);
        }

        private void BttnSaveCurrentLittleTable_Click(object sender, RoutedEventArgs e)
        {
            LittleTable.AppendLittleTable(DateTime.Today, _currentLittleTable);
        }

        private void BttnAddOpenToLittle_Click(object sender, RoutedEventArgs e)
        {
            LittleTable.SetOpenValuesLittleTable(ref _currentLittleTable);
        }

        private void BttnSaveXls_Click(object sender, RoutedEventArgs e)
        {
            XlsDocs.SaveRomanIssuers(_currentLittleTable);
        }

        private void BttnSaveXlsQuote_Click(object sender, RoutedEventArgs e)
        {
            XlsDocs.SaveQuotes(_currentLittleTable);
        }

        private void Label_MouseDoubleClick_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://smart-lab.ru/friends/IndiaGlem/");
        }

        private void BttnTryToCreateDeals_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
