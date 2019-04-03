using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Response
{
    public class DepthResponse : ResponseModel
    {
        public List<double[]> Asks { get; set; }
        public List<double[]> Bids { get; set; }
    }
}
