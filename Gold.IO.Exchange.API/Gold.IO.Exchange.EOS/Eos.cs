using EosSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace EosSharp.Core
{
    /// <summary>
    /// EOSIO client wrapper using general purpose HttpHandler
    /// </summary>
    public class Eos : EosBase
    {
        /// <summary>
        /// EOSIO Client wrapper constructor.
        /// </summary>
        /// <param name="config">Configures client parameters</param>
        public Eos(EosConfigurator config) :
            base(config, new HttpHandler())
        {
        }
    }
}
