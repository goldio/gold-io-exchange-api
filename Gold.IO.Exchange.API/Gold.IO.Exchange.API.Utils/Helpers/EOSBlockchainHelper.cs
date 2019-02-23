using EosSharp.Core;
using EosSharp.Core.Api.v1;
using EosSharp.Core.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.Utils.Helpers
{
    public static class EOSBlockchainHelper
    {
        private static Eos eos = new Eos(new EosConfigurator()
        {
            HttpEndpoint = "https://mainnet.eoscannon.io",
            ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906",
            ExpireSeconds = 60,
            SignProvider = new DefaultSignProvider("dasdas")
        });

        public static async Task<List<GlobalAction>> GetActions()
        {
            var request = await eos.GetActions("goldioioioio", 1, 10);
            return request.actions;
        }
    }
}
