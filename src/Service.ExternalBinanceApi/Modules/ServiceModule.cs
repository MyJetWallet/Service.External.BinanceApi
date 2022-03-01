using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Binance;
using MyJetWallet.Sdk.ExternalMarketsSettings.NoSql;
using MyJetWallet.Sdk.NoSql;
using Service.ExternalBinanceApi.Services;

namespace Service.ExternalBinanceApi.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));
            
            builder.RegisterMyNoSqlReader<ExternalMarketSettingsNoSql>(noSqlClient, ExternalMarketSettingsNoSql.TableName);

            builder.RegisterType<MarketAndBalanceCache>().AsSelf().AutoActivate().SingleInstance();
            
            var api = new BinanceApi();
            var user = new BinanceApiUser(Program.Settings.BinanceApiKey, Program.Settings.BinanceApiSecret);
            
            builder.RegisterInstance(api).AsSelf().SingleInstance();
            builder.RegisterInstance(user).AsSelf().SingleInstance();
        }
    }
}