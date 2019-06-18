using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain.Enum
{
    public enum ApiKeyRole
    {
        NoActions = 0,
        OnlyAccount = 1,
        OnlyOrders = 2,
        AllActions = 3
    }
}
