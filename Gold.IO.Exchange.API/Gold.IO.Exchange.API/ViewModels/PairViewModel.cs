using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class PairViewModel
    {
        public string Symbol { get; set; } // e.g. BTC/USDT
        public CoinViewModel BaseAsset { get; set; }
        public CoinViewModel QuoteAsset { get; set; }

        public PairViewModel() { }

        public PairViewModel(CoinViewModel baseAsset, CoinViewModel quoteAsset)
        {
            BaseAsset = baseAsset;
            QuoteAsset = quoteAsset;
            Symbol = $"{BaseAsset.ShortName}/{QuoteAsset.ShortName}";
        }
    }
}
