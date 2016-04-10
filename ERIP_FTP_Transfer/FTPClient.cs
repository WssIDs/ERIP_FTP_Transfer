using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Windows;

namespace ERIP_FTP_Transfer
{
    public class FTPClient
    {

        string port = Properties.Settings.Default.Port;
        string server_adr = Properties.Settings.Default.server;
        string username = Properties.Settings.Default.username;
        string password = Properties.Settings.Default.password;
        string archivefolder = Properties.Settings.Default.archivepath;

        public bool ConnectToFTP()
        {
            StringBuilder result = new StringBuilder();
            FtpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + server_adr + ":" + port + "/" + "out" + "/"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = true;
                response = (FtpWebResponse)reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();


                if(line != null)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    MessageBox.Show(ex.Message);
                    reader.Close();

                    return false;
                }
                if (response != null)
                {
                    MessageBox.Show(ex.Message);
                    response.Close();

                    return false;
                }
            }
            return false;
        }
    }
}
