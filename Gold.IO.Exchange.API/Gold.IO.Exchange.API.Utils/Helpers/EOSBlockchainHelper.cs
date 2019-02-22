using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EosSharp;
using EosSharp.Core;
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
            SignProvider = new DefaultSignProvider("5JCvkEf3bUKfEeRVeZgVYcgoZSjF3xTF6aXbRHfFSxpEbZ5EycC")
        });
    }
}
