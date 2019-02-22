using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EosSharp;
using EosSharp.Core;
using EosSharp.Core.Api.v1;
using EosSharp.Core.Providers;

namespace Gold.IO.Exchange.API.Utils.Helpers
{
    public static class EOSBlockchainHelper
    {
        private static readonly Eos eos = new Eos(new EosConfigurator()
        {
            HttpEndpoint = "https://mainnet.eoscannon.io",
            ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906",
            ExpireSeconds = 60,
            SignProvider = new DefaultSignProvider("fdsfsfswdewt34")
        });

        public static async Task<List<GlobalAction>> GetActions()
        {
            var response = await eos.GetActions("goldioioioio", 1, 0);
            return response.actions;
        }
    }
}
