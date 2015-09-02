using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Framework
{
    public class FTPClient : IDisposable
    {
        private const int TIMEOUT = 300000;

        private string host = null;
        private string user = null;
        private string pass = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;

        public string Host { get { return host; } }
        public string User { get { return user; } }
        public string Pass { get { return pass; } }

        public FTPClient(string hostIP, string userName, string password) { host = hostIP; user = userName; pass = password; }

        public void DownloadFile(string remoteFile, string localFile)
        {
            // Создаем FTP Request
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(String.Format("{0}/{1}", host, remoteFile));
            // Инициализируем сетевые учетные данные
            ftpRequest.Credentials = new NetworkCredential(user, pass);

            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;

            // Задаем команду, которая будет отправлена на FTP-сервер
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpRequest.Timeout = TIMEOUT;
            // Ответ FTP-сервера
            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            // Возвращаем поток данных
            ftpStream = ftpResponse.GetResponseStream();
            // Создаем локальный файл
            FileStream localFileStream = new FileStream(localFile, FileMode.Create);
            // Пишем в файл
            ftpStream.CopyTo(localFileStream);

            localFileStream.Close();
            ftpStream.Close();
            ftpResponse.Close();
            ftpRequest = null;
        }

        public string[] ListDirectory(string directory)
        {

            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(String.Format("{0}/{1}", host, directory));

            ftpRequest.Credentials = new NetworkCredential(user, pass);

            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;

            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            ftpRequest.Timeout = TIMEOUT;

            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

            ftpStream = ftpResponse.GetResponseStream();

            StreamReader ftpReader = new StreamReader(ftpStream);

            string directoryRaw = null;

            try
            {
                while (ftpReader.Peek() != -1)
                {
                    //TODO: выдергиваем имя файла, доделать...
                    directoryRaw += ftpReader.ReadLine().Split(' ').Last() + "|";
                }
            }
            catch
            {
            }

            ftpReader.Close();
            ftpStream.Close();
            ftpResponse.Close();
            ftpRequest = null;

            try
            {
                string[] directoryList = directoryRaw.Split("|".ToCharArray());
                return directoryList;
            }
            catch
            {
            }

            return new string[] { };
        }

        public void Dispose()
        {
            ftpRequest = null;

            if (ftpStream != null)
            {
                ftpStream.Close();
                ftpStream = null;
            }

            if (ftpResponse != null)
            {
                ftpResponse.Close();
                ftpResponse = null;
            }
        }
    }
}
