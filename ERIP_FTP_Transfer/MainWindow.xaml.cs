using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Globalization;


using System.IO;
using System.Net;
using System.Diagnostics;
using System.Data;

namespace ERIP_FTP_Transfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string port = Properties.Settings.Default.Port;
        string server_adr = Properties.Settings.Default.server;
        string username = Properties.Settings.Default.username;
        string password = Properties.Settings.Default.password;
        string archivefolder = Properties.Settings.Default.archivepath;
        string workingpath = Properties.Settings.Default.workingpath;


        bool bIsDownloading = false;
        string CurrentDownloadingFile = "";
        int fileindex = 0;
        int filescount = 0;

        void downloadfiles(string[] files)
        {
            bIsDownloading = true;
            //files = GetFileList();
            string str = "";

            fileindex = 0;
            filescount = files.Count();

            foreach (string file in files)
            {
                string extension = System.IO.Path.GetExtension(file);

                if (extension == "")
                {
                    filescount--;
                }
                else
                {

                    CurrentDownloadingFile = file;

                    str += file + "\n";
                    DownloadFile(file);

                    fileindex ++;

                    if (Properties.Settings.Default.bDeleteFiles)
                    {
                        if (extension == ".210")
                        {
                            DeleteFile(file);
                        }
                    }
                }



            }

            //MessageBox.Show(str);
            bIsDownloading = false;
            UpdateDataGridList();
            CurrentDownloadingFile = "";
            fileindex = 0;
            filescount = 0;
        }


        void downloadResultfiles(string[] files)
        {

            foreach (string file in files)
            {
                string extension = System.IO.Path.GetExtension(file);

                if (extension == "")
                {
                }
                else
                {
                    if (extension == ".204")
                    {
                        DownloadResultFile(file);
                    }
                }
            }
        }

        public string[] GetFileList()
        {
            string[] downloadFiles;
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
                reqFTP.UsePassive = false;
                //response = reqFTP.GetResponse();
                response = (FtpWebResponse)reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();


                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                // to remove the trailing '\n'
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    MessageBox.Show(ex.Message);
                    reader.Close();
                }
                if (response != null)
                {
                    MessageBox.Show(ex.Message);
                    response.Close();
                }
                downloadFiles = null;
                return downloadFiles;
            }
        }


        private int intCount = 0;
        int filestatus = 0;

        long DownloadFile(string file)
        {


            try
            {
                string uri = "ftp://" + server_adr + "/" + "out" + "/" + file;
                Uri serverUri = new Uri(uri);
                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return 0;
                }
                FtpWebRequest reqFTP;

                long filesize = GetFileSize(file);

                DateTime filetime = GetFileTimestamp(file);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + server_adr + ":" + port + "/" + "out" + "/" + file));
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Proxy = null;
                reqFTP.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream responseStream = response.GetResponseStream();
                FileStream writeStream = new FileStream(workingpath + archivefolder + Properties.Settings.Default.outPath + @"\" + file, FileMode.Create);
                int Length = 4096;
                intCount = 0;
                int bytesRead = 0;
                Byte[] buffer = new Byte[Length];
                bytesRead = responseStream.Read(buffer, 0, Length);

                while (bytesRead > 0)
                {

                    intCount += bytesRead;
                    writeStream.Write(buffer, 0, bytesRead);
                    string test = "Скопировано с ftp-сервера - " + ((long)(intCount * 100 / filesize)).ToString() + "%";
                    int status = ((int)(intCount * 100 / filesize));

                    filestatus = status;

                    bytesRead = responseStream.Read(buffer, 0, Length);

                }
                writeStream.Close();
                response.Close();

                filestatus = 0;

                File.SetCreationTime(workingpath+ archivefolder + Properties.Settings.Default.outPath +@"\" + file, filetime);
                File.SetLastAccessTime(workingpath + archivefolder + Properties.Settings.Default.outPath + @"\" + file, filetime);
                File.SetLastWriteTime(workingpath + archivefolder + Properties.Settings.Default.outPath + @"\" + file, filetime);

                FileInfo newfile = new FileInfo(workingpath + archivefolder + Properties.Settings.Default.outPath + @"\" + file);

                if (newfile.Extension == ".210")
                {
                    if (!File.Exists(workingpath + Properties.Settings.Default.outPath + @"\" + file))
                    {
                        File.Copy(workingpath + archivefolder + Properties.Settings.Default.outPath + @"\" + file, workingpath + Properties.Settings.Default.outPath + @"\" + file);
                    }
                }

                return filesize;

            }
            catch (WebException wEx)
            {
                MessageBox.Show(wEx.Message, "Download Error");
                return 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Download Error");
                return 0;

            }



        }


        void DownloadResultFile(string file)
        {


            try
            {
                string uri = "ftp://" + server_adr + "/" + "out" + "/" + file;
                Uri serverUri = new Uri(uri);
                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return;
                }
                FtpWebRequest reqFTP;

                long filesize = GetFileSize(file);

                DateTime filetime = GetFileTimestamp(file);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + server_adr + ":" + port + "/" + "out" + "/" + file));
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Proxy = null;
                reqFTP.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream responseStream = response.GetResponseStream();
                FileStream writeStream = new FileStream(workingpath + archivefolder + Properties.Settings.Default.outPath + @"\" + Properties.Settings.Default.eripresultpath + @"\" + file, FileMode.Create);
                int Length = 4096;
                intCount = 0;
                int bytesRead = 0;
                Byte[] buffer = new Byte[Length];
                bytesRead = responseStream.Read(buffer, 0, Length);

                while (bytesRead > 0)
                {

                    intCount += bytesRead;
                    writeStream.Write(buffer, 0, bytesRead);
                    string test = "Скопировано с ftp-сервера - " + ((long)(intCount * 100 / filesize)).ToString() + "%";
                    int status = ((int)(intCount * 100 / filesize));

                    bytesRead = responseStream.Read(buffer, 0, Length);

                }
                writeStream.Close();
                response.Close();

                File.SetCreationTime(workingpath + archivefolder + Properties.Settings.Default.outPath + @"\" + Properties.Settings.Default.eripresultpath + @"\" + file, filetime);
                File.SetLastAccessTime(workingpath + archivefolder + Properties.Settings.Default.outPath + @"\" + Properties.Settings.Default.eripresultpath + @"\" + file, filetime);
                File.SetLastWriteTime(workingpath + archivefolder + Properties.Settings.Default.outPath + @"\" + Properties.Settings.Default.eripresultpath + @"\" + file, filetime);

            }
            catch (WebException wEx)
            {
                MessageBox.Show(wEx.Message, "Download Error");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Download Error");

            }



        }



        void DeleteFile(string file)
        {
            try
            {
                string uri = "ftp://" + server_adr + "/" + "out" + "/" + file;
                Uri serverUri = new Uri(uri);
                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return;
                }
                FtpWebRequest reqFTP;

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + server_adr + ":" + port + "/" + "out" + "/" + file));
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.UseBinary = true;
                reqFTP.Proxy = null;
                reqFTP.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                response.Close();

            }
            catch (WebException wEx)
            {
                MessageBox.Show(wEx.Message, "Delete Error");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Delete Error");

            }



        }


        int processProgress(int invalue)
        {
           // Trace.WriteLine(invalue);

            return invalue;
        }


        // Use FTP to get a remote file's timestamp.
        private DateTime GetFileTimestamp(string file)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + server_adr + ":" + port + "/" + "out" + "/" + file);
            request.Method = WebRequestMethods.Ftp.GetDateTimestamp;

            // Get network credentials.
            request.Credentials =
                new NetworkCredential(username, password);

            try
            {
                using (FtpWebResponse response =
                    (FtpWebResponse)request.GetResponse())
                {
                    // Return the size.
                    return response.LastModified;
                }
            }
            catch (Exception ex)
            {
                // If the file doesn't exist, return Jan 1, 3000.
                // Otherwise rethrow the error.
                if (ex.Message.Contains("File unavailable"))
                    return new DateTime(3000, 1, 1);
                throw;
            }
        }


        long GetFileSize(string file)
        {
            FtpWebRequest request;
            request = (FtpWebRequest)FtpWebRequest.Create("ftp://" + server_adr + ":" + port + "/" + "out" + "/" + file);
            request.Credentials = new NetworkCredential(username, password);
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.UseBinary = true;
            request.Proxy = null;
            request.UsePassive = true;

            try
            {
                using (FtpWebResponse response =
                    (FtpWebResponse)request.GetResponse())
                {
                    // Return the size.
                    return response.ContentLength;
                }
            }
            catch (Exception ex)
            {
                // If the file doesn't exist, return Jan 1, 3000.
                // Otherwise rethrow the error.
                if (ex.Message.Contains("File unavailable"))
                    return 0;
                throw;
            }
        }








        private void getfilelist_Click(object sender, RoutedEventArgs e)
        {
            Thread getfiles_thread = new Thread(() => UpdateDataGridList());
            getfiles_thread.Start();

        }



        List<string> resultfiles = new List<string>();

        void UpdateDataGridList()
        {
            Dispatcher.BeginInvoke(new Action(() => getfilelist.IsEnabled = false));
            Dispatcher.BeginInvoke(new Action(() => dataGrid.IsEnabled = false));
            Dispatcher.BeginInvoke(new Action(() => getfiles_button.IsEnabled = false));

            List<string> resultfilesdir = (Directory.GetFiles(workingpath + archivefolder + Properties.Settings.Default.outPath + @"\" + Properties.Settings.Default.eripresultpath + @"\")).ToList<string>();

            List<string> tempresultfiles = new List<string>();

            List<string> files = GetFileList().ToList<string>();

            if (files != null)
            {
                ObservableCollection<FTPFileInfo> items = new ObservableCollection<FTPFileInfo>();

                int i = 0;
                int j = 0;

                foreach (string file in files)
                {
                    string extension = System.IO.Path.GetExtension(file);
                    if (extension == ".210")
                    {

                        Dispatcher.BeginInvoke(new Action(() => сurrentf_label.Content = "Чтение каталога: root/out/ | Статус: Чтение файла - " + file));
                        items.Add(new FTPFileInfo() { FileName = file, DateTime = (GetFileTimestamp(file)).ToString("dd.MM.yyyy H:mm:ss"), FileSize = GetFileSize(file) / 1000.0f });
                        i++;
                    }

                    if (extension == ".204")
                    {
                        tempresultfiles.Add(file);
                        j++;
                    }
                }


                foreach (string item in tempresultfiles)
                {
                    if(!resultfilesdir.Contains(item))
                    {
                        resultfiles.Add(item);
                    }
                }

                Dispatcher.BeginInvoke(new Action(() => сurrentf_label.Content = "Чтение каталога: root/out/ | Статус: Операция завершена"));


                Dispatcher.BeginInvoke(new Action(() => dataGrid.ItemsSource = items));

                Dispatcher.BeginInvoke(new Action(() => getfiles_button.IsEnabled = true));
                Dispatcher.BeginInvoke(new Action(() => dataGrid.IsEnabled = true));
                Dispatcher.BeginInvoke(new Action(() => getfilelist.IsEnabled = true));

                MessageBox.Show("Операция завершена", "Системное сообщение");
            }

            else
            {
                MessageBox.Show("Нет файлов для загрузки","Системное сообщение");
            }
        }


        private void getfiles_button_Click(object sender, RoutedEventArgs e)
        {


            List<FTPFileInfo> items = dataGrid.SelectedItems.Cast<FTPFileInfo>().ToList();



            if (resultfiles.Count > 0)
            {
                Thread resultthreads = new Thread(() => downloadResultfiles(resultfiles.ToArray()));

                resultthreads.Start();
            }

            if (items.Count > 0)
            { 
                getfiles_button.IsEnabled = false;
                getfilelist.IsEnabled = false;

                string[] files = new string[items.Count];

                int i = 0;
                foreach (FTPFileInfo item in items)
                {
                    files[i]= item.FileName;
                    i++;
                }

                Thread threads1 = new Thread(() => downloadfiles(files));

                threads1.Start();

                Thread th2 = new Thread(
                    new ThreadStart(() =>
                    {
                        while (bIsDownloading)
                        {
                            Thread.Sleep(50);

                            Dispatcher.BeginInvoke(new Action(() => progressfilestatus.Value = filestatus));
                        }

                    }
                 ));
                th2.Start();


                Thread th3 = new Thread(
                    new ThreadStart(() =>
                    {
                        while (bIsDownloading)
                        {
                            Thread.Sleep(50);

                            Dispatcher.BeginInvoke(new Action(() => statusfile.Content = (fileindex.ToString()) + "/" + (filescount).ToString() + "  Скачивается файл: " + CurrentDownloadingFile));
                        }

                        Thread.Sleep(50);
                        Dispatcher.BeginInvoke(new Action(() => statusfile.Content = (fileindex) + "/" + (filescount).ToString() + "  Все файлы загружены"));

                    }
                 ));
                th3.Start();


                Thread th4 = new Thread(
                    new ThreadStart(() =>
                    {
                        while (bIsDownloading)
                        {
                            Thread.Sleep(50);
                            Dispatcher.BeginInvoke(new Action(() => getfilelist.IsEnabled = false));

                            Thread.Sleep(50);
                            Dispatcher.BeginInvoke(new Action(() => getfiles_button.IsEnabled = false));

                            Thread.Sleep(50);
                            Dispatcher.BeginInvoke(new Action(() => statusoperation.Content = ""));

                        }

                        Thread.Sleep(50);
                        Dispatcher.BeginInvoke(new Action(() => getfilelist.IsEnabled = true));

                        Thread.Sleep(50);
                        Dispatcher.BeginInvoke(new Action(() => getfiles_button.IsEnabled = true));

                        Thread.Sleep(50);
                        Dispatcher.BeginInvoke(new Action(() => statusoperation.Content = "Операция завершена"));

                    }
                 ));
                th4.Start();

            }

            else
            {
                MessageBox.Show("Не выбраны файлы для загрузки", "Системное сообщение");
            }

        }

        private void menubut_exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menubut_settings_Click(object sender, RoutedEventArgs e)
        {
            SettingForm settings_wnd = new SettingForm();

            settings_wnd.Owner = this;
            settings_wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings_wnd.ShowDialog();
        }

        private void generatefiles_but_Click(object sender, RoutedEventArgs e)
        {
            Random numbers = new Random();

            FileStream fileopen = new FileStream(workingpath + @"\test.txt",FileMode.Open);

            byte[] bytes = new byte[fileopen.Length];

            fileopen.Read(bytes,0,(int)fileopen.Length);

            for (int i = 0; i < 30; i++)
            {
                string name = "";

                for (int j = 0; j < 8; j++)
                {
                    name += numbers.Next(0, 9);
                }

                FileStream file = new FileStream(workingpath + @"\temp\" + name + ".210",FileMode.Create);

                file.Write(bytes,0,bytes.Length);
                file.Flush();
                file.Close();
            }

        }
    }
}
