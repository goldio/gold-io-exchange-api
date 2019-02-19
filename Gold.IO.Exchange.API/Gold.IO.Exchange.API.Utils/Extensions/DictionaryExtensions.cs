using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gold.IO.Exchange.API.Utils.Extensions
{
    public static class DictionaryExtensions
    {
        public static string ToQueryString(this IDictionary<string, string> values)
        {
            return string.Join("&", values.Select(x => $"{x.Key}={x.Value}"));
        }
    }
}
