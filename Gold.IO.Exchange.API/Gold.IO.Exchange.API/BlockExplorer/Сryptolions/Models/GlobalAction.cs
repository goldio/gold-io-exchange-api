using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Сryptolions.Models
{
    public class GlobalAction
    {
        [JsonProperty("_id")]
        public string ID { get; set; }

        [JsonProperty("act")]
        public Action Action { get; set; }

        [JsonProperty("block_num")]
        public long BlockNum { get; set; }

        [JsonProperty("block_time")]
        public DateTime BlockTime { get; set; }

        [JsonProperty("console")]
        public string Console { get; set; }

        [JsonProperty("context_free")]
        public bool ContextFree { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("elapsed")]
        public long Elapsed { get; set; }

        [JsonProperty("producer_block_id")]
        public string ProducerBlockID { get; set; }

        [JsonProperty("trx_id")]
        public string TransactionID { get; set; }

        [JsonProperty("trx_status")]
        public string TransactionStatus { get; set; }
    }
}
