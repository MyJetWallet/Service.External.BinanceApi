﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binance;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using MyJetWallet.Sdk.ExternalMarketsSettings.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;

namespace Service.ExternalBinanceApi.Services
{
    public class MarketAndBalanceCache: IDisposable
    {
        private readonly BinanceApi _client;
        private readonly IBinanceApiUser _user;
        private readonly ILogger<MarketAndBalanceCache> _logger;
        private readonly IMyNoSqlServerDataReader<ExternalMarketSettingsNoSql> _externalMarketSettingsReader;

        private Dictionary<string, ExchangeBalance> _balances = new Dictionary<string, ExchangeBalance>();
        

        private readonly MyTaskTimer _timer;
        private int _countFailUpdate;

        public MarketAndBalanceCache(
            ILogger<MarketAndBalanceCache> logger, 
            IMyNoSqlServerDataReader<ExternalMarketSettingsNoSql> externalMarketSettingsReader)
        {
            _logger = logger;
            _externalMarketSettingsReader = externalMarketSettingsReader;

            _client = new BinanceApi();
            _user = new BinanceApiUser(Program.Settings.BinanceApiKey, Program.Settings.BinanceApiSecret);

            _timer = new MyTaskTimer(nameof(MarketAndBalanceCache), TimeSpan.FromSeconds(10), logger, DoTimer);
        }

        private async Task DoTimer()
        {
            _timer.ChangeInterval(TimeSpan.FromSeconds(Program.Settings.RefreshBalanceIntervalSec));

            using var activity = MyTelemetry.StartActivity("Refresh balance data");
            try
            {
                await RefreshData();
                _countFailUpdate = 0;
            }
            catch (Exception ex)
            {
                _countFailUpdate++;
                if (_countFailUpdate > 10) _logger.LogError(ex, "Error on refresh balance");
                else _logger.LogWarning(ex, "Error on refresh balance");
                ex.FailActivity();
            }
        }

        public async Task RefreshData()
        {
            _logger.LogInformation("Balance and market update");

            var balances = await GetMarginAccountBalances();

            var dict = new Dictionary<string, ExchangeBalance>();

            foreach (var balance in balances)
            {
                using var activityBalance = MyTelemetry.StartActivity($"Update balance {balance.Asset}")?.AddTag("asset", balance.Asset);
                var item = new ExchangeBalance()
                {
                    Symbol = balance.Asset,
                    Balance = balance.Free - balance.Borrowed,
                };
                
                dict[item.Symbol] = item;
                
                try
                {
                    var free = await _client.GetMaxBorrowAsync(_user, balance.Asset);
                    item.Free = (decimal)free;
                }
                catch (Exception ex)
                {
                    ex.FailActivity();
                    _logger.LogWarning(ex, $"Cannot update borrow balance {balance.Asset}: {ex.Message}");
                }
            }

            _balances = dict;
        }

        private async Task<List<MarginAccountBalance>> GetMarginAccountBalances()
        {
            try
            {
                var markets = GetMarkets();
                var baseAssets = markets.Select(e => e.BaseAsset).ToHashSet();
                var quoteAssets = markets.Select(e => e.QuoteAsset).ToHashSet();
                var balances = await _client.GetMarginBalancesAsync(_user);
                
                balances = balances.Where(e => baseAssets.Contains(e.Asset) || quoteAssets.Contains(e.Asset)).ToList();
                return balances;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot get account balance");
                throw;
            }
        }
        
        public void Start()
        {
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }

        public List<ExchangeMarketInfo> GetMarkets()
        {
            try
            {
                var data = _externalMarketSettingsReader.Get().Select(e => e.Settings).ToList();

                return data.Select(e => new ExchangeMarketInfo()
                {
                    Market = e.Market,
                    BaseAsset = e.BaseAsset,
                    QuoteAsset = e.QuoteAsset,
                    MinVolume = e.MinVolume,
                    PriceAccuracy = e.PriceAccuracy,
                    VolumeAccuracy = e.VolumeAccuracy,
                    AssociateInstrument = e.AssociateInstrument,
                    AssociateBaseAsset = e.AssociateBaseAsset,
                    AssociateQuoteAsset = e.AssociateQuoteAsset
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get Binance GetMarketInfo");
                throw;
            }
        }

        public List<ExchangeBalance> GetBalances()
        {
            return _balances.Values.ToList();
        }
    }
}