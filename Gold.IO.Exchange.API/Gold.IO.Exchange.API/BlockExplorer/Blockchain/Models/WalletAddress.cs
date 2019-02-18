using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Blockchain.Models
{
    public class WalletAddress
    {
        [JsonProperty("total_received")]
        private long _totalReceived;

        [JsonProperty("total_sent")]
        private long _totalSent;

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("final_balance")]
        public double FinalBalance { get; set; }

        [JsonProperty("hash160")]
        public string Hash160 { get; set; }

        [JsonProperty("n_tx")]
        public long TxsCount { get; set; }

        public double TotalReceived
        {
            get => _totalReceived / 100000000;
        }

        public double TotalSent
        {
            get => _totalSent / 100000000;
        }

        [JsonProperty("txs")]
        public List<Transaction> Txs { get; set; }
    }
}
