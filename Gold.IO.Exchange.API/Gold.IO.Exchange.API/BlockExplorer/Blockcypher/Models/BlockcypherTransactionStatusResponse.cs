using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Blockcypher.Models
{
    public class BlockcypherTransactionStatusResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public BlockcypherTransactionStatus Result { get; set; }
    }
}
