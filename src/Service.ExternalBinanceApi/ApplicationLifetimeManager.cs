﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using Service.ExternalBinanceApi.Services;

namespace Service.ExternalBinanceApi
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly MyNoSqlClientLifeTime _myNoSqlClientLifeTime;
        private readonly MarketAndBalanceCache _marketAndBalanceCache;
        private readonly ILogger<ApplicationLifetimeManager> _logger;

        public ApplicationLifetimeManager(
            IHostApplicationLifetime appLifetime, 
            MyNoSqlClientLifeTime myNoSqlClientLifeTime,
            MarketAndBalanceCache marketAndBalanceCache,
            ILogger<ApplicationLifetimeManager> logger)
            : base(appLifetime)
        {
            _myNoSqlClientLifeTime = myNoSqlClientLifeTime;
            _marketAndBalanceCache = marketAndBalanceCache;
            _logger = logger;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _myNoSqlClientLifeTime.Start();
            _marketAndBalanceCache.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _myNoSqlClientLifeTime.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}