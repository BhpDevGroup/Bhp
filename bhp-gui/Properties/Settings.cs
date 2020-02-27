using Microsoft.Extensions.Configuration;
using Bhp.Network.P2P;
using System.Linq;

namespace Bhp.Properties
{
    internal sealed partial class Settings
    {
        public AppConfigs Configs { get; }
        public PathsSettings Paths { get; }
        public P2PSettings P2P { get; }
        public BrowserSettings Urls { get; }
        public ContractSettings Contracts { get; }
        public UnlockWalletSettings UnlockWallet { get; }
        public CertificateSettings Certificate { get; }

        public Settings()
        {
            if (NeedUpgrade)
            {
                Upgrade();
                NeedUpgrade = false;
                Save();
            }
            IConfigurationSection section = new ConfigurationBuilder().AddJsonFile("config.json").Build().GetSection("ApplicationConfiguration");
            this.Configs = new AppConfigs(section.GetSection("AppConfigs"));
            this.Paths = new PathsSettings(section.GetSection("Paths"));
            this.P2P = new P2PSettings(section.GetSection("P2P"));
            this.Urls = new BrowserSettings(section.GetSection("Urls"));
            this.Contracts = new ContractSettings(section.GetSection("Contracts"));
            this.UnlockWallet = new UnlockWalletSettings(section.GetSection("UnlockWallet"));
            this.Certificate = new CertificateSettings(section.GetSection("Certificate"));
        }
    }

    internal class AppConfigs
    {
        public string Development { get; }
        public int LastestTxDay { get; }
        public AppConfigs(IConfigurationSection section)
        {
            Development = section.GetSection("Development").Value;
            LastestTxDay = int.Parse(section.GetSection("LastestTxDay").Value);
        }
    }

    internal class PathsSettings
    {
        public string Chain { get; }
        public string Index { get; }
        public string CertCache { get; }

        public PathsSettings(IConfigurationSection section)
        {
            this.Chain = string.Format(section.GetSection("Chain").Value, Message.Magic.ToString("X8"));
            this.Index = string.Format(section.GetSection("Index").Value, Message.Magic.ToString("X8"));
            this.CertCache = section.GetSection("CertCache").Value;
        }
    }

    internal class P2PSettings
    {
        public ushort Port { get; }
        public ushort WsPort { get; }

        public P2PSettings(IConfigurationSection section)
        {
            this.Port = ushort.Parse(section.GetSection("Port").Value);
            this.WsPort = ushort.Parse(section.GetSection("WsPort").Value);
        }
    }

    internal class BrowserSettings
    {
        public string AddressUrl { get; }
        public string AssetUrl { get; }
        public string TransactionUrl { get; }

        public BrowserSettings(IConfigurationSection section)
        {
            this.AddressUrl = section.GetSection("AddressUrl").Value;
            this.AssetUrl = section.GetSection("AssetUrl").Value;
            this.TransactionUrl = section.GetSection("TransactionUrl").Value;
        }
    }

    internal class ContractSettings
    {
        public UInt160[] BRC20 { get; }

        public ContractSettings(IConfigurationSection section)
        {
            this.BRC20 = section.GetSection("BRC20").GetChildren().Select(p => UInt160.Parse(p.Value)).ToArray();
        }
    }

    internal class UnlockWalletSettings
    {
        public bool IsBhpFee { get; }

        public UnlockWalletSettings(IConfigurationSection section)
        {
            if (section.Exists())
            {
                this.IsBhpFee = bool.Parse(section.GetSection("IsBhpFee").Value);
            }
        }
    }

    internal class CertificateSettings
    {
        public string Name { get; }

        public CertificateSettings(IConfigurationSection section)
        {
            if (section.Exists())
            {
                this.Name = section.GetSection("Name").Value;
            }
        }
    }
}
