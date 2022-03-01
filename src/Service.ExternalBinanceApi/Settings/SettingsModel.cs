using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.ExternalBinanceApi.Settings
{
    public class SettingsModel
    {
        [YamlProperty("ExternalBinanceApi.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("ExternalBinanceApi.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("ExternalBinanceApi.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("ExternalBinanceApi.BinanceApiKey")]
        public string BinanceApiKey { get; set; }
        
        [YamlProperty("ExternalBinanceApi.BinanceApiSecret")]
        public string BinanceApiSecret { get; set; }
        
        [YamlProperty("ExternalBinanceApi.RefreshBalanceIntervalSec")]
        public int RefreshBalanceIntervalSec { get; set; }
        
        [YamlProperty("ExternalBinanceApi.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }
    }
}
