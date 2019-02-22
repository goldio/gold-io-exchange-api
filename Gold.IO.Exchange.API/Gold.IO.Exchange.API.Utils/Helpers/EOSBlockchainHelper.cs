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
        private static readonly Eos eos = new Eos();

        public static async Task<List<GlobalAction>> GetActions()
        {
            var response = await eos.GetActions("goldioioioio", 1, 0);
            return response.actions;
        }
    }
}
