using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Blockcypher.Models
{
    public class BlockcypherResponse<T>
    {
        public string Address { get; set; }
        public BigInteger Balance { get; set; }
        public T[] Txrefs { get; set; }
    }
}
