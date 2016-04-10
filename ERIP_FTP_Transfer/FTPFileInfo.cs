using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ERIP_FTP_Transfer
{
    public class FTPFileInfo
    {
        public string FileName { get; set; }

        public string DateTime { get; set; }

        public float FileSize { get; set; }
    }
}
