using System;

namespace Framework.Wrappers
{
    public interface IWebClientAdapter : IDisposable
    {
        void DownloadFile(Uri address, string fileName);
        void DownloadFileAsync(Uri address, string fileName);
    }
}
