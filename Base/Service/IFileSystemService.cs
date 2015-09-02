using Base.DAL;
using Framework.Wrappers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Base.Service
{
    public interface IFileSystemService : IService
    {
        string FilesDirectory { get; }
        string ContentDirectory { get; }
        FileData SaveFile(IPostedFileWrapper file);
        FileData SaveFile(Uri address, out Exception error);
        FileData SaveFile(Stream stream, out Exception error);
        FileData SaveFile(Stream stream, string extension, out Exception error);
        string GetFilePath(Guid fileid);
        Guid GetFileID(string filename);
        string GetFilePath(Guid fileid, bool checkTemp);
        string GetFilePathFromContent(string subFolder, string fileName, string extension);
        FileData GetFileData(IUnitOfWork unitOfWork, int id);
        FileData GetFileData(IUnitOfWork unitOfWork, Guid fileid);
        Task<FileData> GetFileDataAsync(IUnitOfWork unitOfWork, int id);
        Task<FileData> GetFileDataAsync(IUnitOfWork unitOfWork, Guid fileid);

    }

    public interface IFileManager
    {
        int DeleteFiles();
        int DeleteFileData();
    }
}