using Gold.IO.Exchange.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class FileViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }

        public FileViewModel() { }

        public FileViewModel(File file)
        {
            ID = file.ID;
            Name = file.Name;
            URL = file.URL;
        }

        public static explicit operator FileViewModel(File file) => new FileViewModel(file);
    }
}
