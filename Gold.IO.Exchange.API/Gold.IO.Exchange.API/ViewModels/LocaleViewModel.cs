using Gold.IO.Exchange.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class LocaleViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string LangCode { get; set; }

        public LocaleViewModel() { }

        public LocaleViewModel(Locale locale)
        {
            ID = locale.ID;
            Name = locale.Name;
            LangCode = locale.LangCode;
        }

        public static explicit operator LocaleViewModel(Locale locale) => new LocaleViewModel(locale);
    }
}
