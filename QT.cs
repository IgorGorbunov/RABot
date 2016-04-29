using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ecng.Common;
using StockSharp.BusinessEntities;
using StockSharp.Localization;
using StockSharp.Logging;
using StockSharp.Quik;


public class QT : IDisposable
{
    public QuikTrader Trader;

    public delegate void NewSecuritiesDelegate(IEnumerable<Security> securities);

    private string _fixServerAddress;
    private string _luaLogin;
    private string _luaPassword;

    private readonly LogManager _logManager = new LogManager();

    private bool _isConnected;
    private bool _findSecurity;
    private IEnumerable<Security> _securities;
    private readonly List<Security> _findedSecurities = new List <Security>(); 

    public QT() : this("127.0.0.1:5001", "quikBot", "quik_RomaN")
    {
        
    }

    public QT(string fixServerAddress, string luaLogin, string luaPassword)
    {
        _fixServerAddress = fixServerAddress;
        _luaLogin = luaLogin;
        _luaPassword = luaPassword;
    }

    public void LuaConnect()
    {
        if (!_isConnected)
		{
			if (_fixServerAddress.IsEmpty())
			{
				MessageBox.Show(LocalizedStrings.Str2977);
				return;
			}

			if (_luaLogin.IsEmpty())
			{
				MessageBox.Show(LocalizedStrings.Str2978);
				return;
			}

			if (_luaPassword.IsEmpty())
			{
				MessageBox.Show(LocalizedStrings.Str2979);
				return;
			}



			if (Trader == null)
			{
				// создаем подключение
				Trader = new QuikTrader
				    {
				            LuaFixServerAddress = _fixServerAddress.To <EndPoint>(),
				            LuaLogin = _luaLogin,
				            LuaPassword = _luaPassword.To <SecureString>()
				    };


				Trader.LogLevel = LogLevels.Debug;

				_logManager.Sources.Add(Trader);

				// отключение автоматического запроса всех инструментов.
				//Trader.RequestAllSecurities = AllSecurities.IsChecked == true;
                Trader.RequestAllSecurities = true;

				// возводим флаг, что соединение установлено
				_isConnected = true;

				// переподключение будет работать только во время работы биржи РТС
				// (чтобы отключить переподключение когда торгов нет штатно, например, ночью)
				Trader.ReConnectionSettings.WorkingTime = ExchangeBoard.Forts.WorkingTime;

				// подписываемся на событие об успешном восстановлении соединения
				Trader.Restored += () => MessageBox.Show(LocalizedStrings.Str2958);

				// подписываемся на событие разрыва соединения
				Trader.ConnectionError += error => MessageBox.Show(error.ToString());

				// подписываемся на ошибку обработки данных (транзакций и маркет)
				//Trader.Error += error =>
				//	this.GuiAsync(() => MessageBox.Show(this, error.ToString(), "Ошибка обработки данных"));

				// подписываемся на ошибку подписки маркет-данных
				Trader.MarketDataSubscriptionFailed += (security, type, error) => MessageBox.Show(error.ToString(), LocalizedStrings.Str2956Params.Put(type, security));

				Trader.NewSecurities += TraderOnNewSecurities;

                Trader.LookupSecuritiesResult += TraderOnLookupSecuritiesResult;

                //Trader.NewMyTrades += trades => _myTradesWindow.TradeGrid.Trades.AddRange(trades);
                //Trader.NewTrades += trades => _tradesWindow.TradeGrid.Trades.AddRange(trades);
                //Trader.NewOrders += orders => _ordersWindow.OrderGrid.Orders.AddRange(orders);
                //Trader.NewStopOrders += orders => _stopOrderWindow.OrderGrid.Orders.AddRange(orders);
                //Trader.OrdersRegisterFailed += fails => fails.ForEach(fail => this.GuiAsync(() => MessageBox.Show(this, fail.Error.Message, LocalizedStrings.Str2960)));
                //Trader.OrdersCancelFailed += fails => fails.ForEach(fail => this.GuiAsync(() => MessageBox.Show(this, fail.Error.Message, LocalizedStrings.Str2981)));
                //Trader.StopOrdersRegisterFailed += fails => fails.ForEach(fail => this.GuiAsync(() => MessageBox.Show(this, fail.Error.Message, LocalizedStrings.Str2960)));
                //Trader.StopOrdersCancelFailed += fails => fails.ForEach(fail => this.GuiAsync(() => MessageBox.Show(this, fail.Error.Message, LocalizedStrings.Str2981)));
                //Trader.NewPortfolios += portfolios => _portfoliosWindow.PortfolioGrid.Portfolios.AddRange(portfolios);
                //Trader.NewPositions += positions => _portfoliosWindow.PortfolioGrid.Positions.AddRange(positions);

				// устанавливаем поставщик маркет-данных
				//_securitiesWindow.SecurityPicker.MarketDataProvider = Trader;

                //ShowSecurities.IsEnabled = ShowTrades.IsEnabled =
                //    ShowMyTrades.IsEnabled = ShowOrders.IsEnabled =
                //        ShowPortfolios.IsEnabled = ShowStopOrders.IsEnabled = true;
			}

			Trader.Connect();
			_isConnected = true;
            Debug.WriteLine(Trader.ConnectionState.ToString());
		}
		else
		{
			Trader.Disconnect();
			_isConnected = false;
		}
    }

    public void GetAllSecurities()
    {
        Security security = new Security();
        Trader.LookupSecurities(security);
    }

    public Security GetSecurity(string code)
    {
        Security security = GetLocalSaveSecurity(code);
        if (security != null)
        {
            return security;
        }
        security = new Security();
        security.Code = code;
        _findSecurity = true;
        Trader.LookupSecurities(security);
        while (_findSecurity)
        {
            Thread.Sleep(100);
        }
        foreach (Security findSecurity in _securities)
        {
            if (findSecurity.Class == "TQBR" && findSecurity.Code == code)
            {
                _findedSecurities.Add(findSecurity);
                return findSecurity;
            }
        }
        return null;
    }

    public decimal? GetSecOpenVal(string code)
    {
        Security security = RegisterSecurity(code);
        decimal? value = security.OpenPrice;
        while (value == null)
        {
            Thread.Sleep(100);
            value = security.OpenPrice;
        }
        Trader.UnRegisterSecurity(security);
        //Trader.UnRegisterTrades(security);
        return value;
    }

    public Security RegisterSecurity(string code)
    {
        Security security = GetSecurity(code);
        if (!Trader.RegisteredSecurities.Contains(security))
        {
            Trader.RegisterSecurity(security);
            //Trader.RegisterTrades(security);
        }
        return security;
    }

    private Security GetLocalSaveSecurity(string code)
    {
        foreach (Security findedSecurity in _findedSecurities)
        {
            if (findedSecurity.Code == code)
            {
                return findedSecurity;
            }
        }
        return null;
    }

    private void TraderOnLookupSecuritiesResult(IEnumerable <Security> securities)
    {
        //System.Windows.Forms.MessageBox.Show(securities.Count().ToString());
        if (securities.Count() < 1 )
            //|| securities.Count() > 2000)
        {
            return;
        }
        if (_findSecurity)
        {
            _securities = securities;
        }
        _findSecurity = false;
        //if (securities.Count() > 1)
        //{
        //    System.Windows.Forms.MessageBox.Show("Lookup " + securities.Count().ToString());
        //    foreach (Security security in securities)
        //    {
        //        string line = "";
        //        line += security.Board + Environment.NewLine;
        //        line += security.Class + Environment.NewLine;
        //        line += security.Code + Environment.NewLine;
        //        line += security.Currency + Environment.NewLine;
        //        line += security.Id + Environment.NewLine;
        //        line += security.Name + Environment.NewLine;
        //        line += security.State + Environment.NewLine;
        //        line += security.Type + Environment.NewLine;
        //        line += security.OpenPrice + Environment.NewLine;
        //        System.Windows.Forms.MessageBox.Show(line);
        //    }
        //}
    }

    

    private void TraderOnNewSecurities(IEnumerable <Security> securities)
    {
        if (securities.Count() > 1)
        {
            System.Windows.Forms.MessageBox.Show("New " + securities.Count().ToString());
        }
    }

    #region Implementation of IDisposable

    bool _disposed;

    /// <summary>
    /// Выполняет определяемые приложением задачи, связанные с высвобождением или сбросом неуправляемых ресурсов.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            //handle.Dispose();
            // Free any other managed objects here.
            //
        }

        // Free any unmanaged objects here.
        _findedSecurities.Clear();
        Trader.Disconnect();
        Trader.Dispose();
        _disposed = true;
    }

    #endregion
}
