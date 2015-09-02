
namespace Base.Service
{
    public interface IPathHelper : IService
    {
        string GetRootDirectory();
        string GetTempDirectory();
        string GetFilesDirectory();
        string GetLogDirectory();
        string GetVideoConvertDirectory();
        string GetAudioConvertDirectory();
        string GetContentDirectory();
        string GetViewsDirectory();
        string GetViewsSharedDirectory();
        string GetAppDataDirectory();
    }
}
