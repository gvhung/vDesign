using Base.DAL;
using Framework.Wrappers;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Base.Service
{
    public class FileSystemService : IFileSystemService
    {
        private readonly IWebClientAdapter _webClientAdapter;

        private readonly string _filesDirectory;
        private readonly string _contentDirectory;

        public string FilesDirectory { get { return _filesDirectory; } }
        public string ContentDirectory { get { return _contentDirectory; } }

        public FileSystemService(IPathHelper pathHelper, IWebClientAdapter webClientAdapter)
        {
            _webClientAdapter = webClientAdapter;
            _filesDirectory = pathHelper.GetFilesDirectory();
            _contentDirectory = pathHelper.GetContentDirectory();
        }

        public FileData SaveFile(IPostedFileWrapper file)
        {
            if (file == null) return null;

            if (!Directory.Exists(FilesDirectory))
                Directory.CreateDirectory(FilesDirectory);

            var fileid = Guid.NewGuid();

            var result = new FileData()
            {
                FileID = (Guid)fileid,
                FileName = file.FileName,
                Size = file.ContentLength,
                CreationDate = DateTime.Now,
                ChangeDate = DateTime.Now,
            };

            string path = GetFilePath(result.FileID);

            string dir = Directory.GetParent(path).FullName;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            else if (File.Exists(path))
                File.Delete(path);

            file.SaveAs(path);

            return result;
        }

        public FileData SaveFile(Uri address, out Exception error)
        {
            if (!Directory.Exists(FilesDirectory))
                Directory.CreateDirectory(FilesDirectory);

            var fileid = Guid.NewGuid();

            string path = GetFilePath(fileid);

            string dir = Directory.GetParent(path).FullName;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            else if (File.Exists(path))
                File.Delete(path);

            FileInfo fi = null;

            try
            {
                using (var client = Activator.CreateInstance(_webClientAdapter.GetType()) as IWebClientAdapter)
                {
                    if (client != null) client.DownloadFile(address, path);
                }

                fi = new FileInfo(path);

                error = null;
            }
            catch (Exception e)
            {
                fileid = Guid.Empty;
                error = e;
            }

            return new FileData()
            {
                FileID = fileid,
                FileName = fi != null ? fi.Name : fileid.ToString(),
                Size = fi != null ? fi.Length : 0,
                CreationDate = DateTime.Now,
                ChangeDate = DateTime.Now,
            };
        }

        public FileData SaveFile(Stream stream, out Exception error)
        {
            if (!Directory.Exists(FilesDirectory))
                Directory.CreateDirectory(FilesDirectory);

            var fileid = Guid.NewGuid();

            string path = GetFilePath(fileid);

            string dir = Directory.GetParent(path).FullName;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            else if (File.Exists(path))
                File.Delete(path);

            FileInfo fi = null;

            try
            {
                using (var fileStream = File.Create(path))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }

                fi = new FileInfo(path);
                error = null;
            }
            catch (Exception e)
            {
                fileid = Guid.Empty;
                error = e;
            }

            return new FileData()
            {
                FileID = fileid,
                FileName = fi != null ? fi.Name : fileid.ToString(),
                Size = fi != null ? fi.Length : 0,
                CreationDate = DateTime.Now,
                ChangeDate = DateTime.Now,
            };
        }

        public FileData SaveFile(Stream stream, string extension, out Exception error)
        {
            if (!Directory.Exists(FilesDirectory))
                Directory.CreateDirectory(FilesDirectory);

            var fileid = Guid.NewGuid();

            string path = GetFilePath(fileid);

            string dir = Directory.GetParent(path).FullName;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            else if (File.Exists(path))
                File.Delete(path);

            FileInfo fi = null;

            try
            {
                using (var fileStream = File.Create(path))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }

                fi = new FileInfo(path);
                error = null;
            }
            catch (Exception e)
            {
                fileid = Guid.Empty;
                error = e;
            }

            return new FileData()
            {
                FileID = fileid,
                FileName = fi != null ? fi.Name + extension : fileid + extension,
                Size = fi != null ? fi.Length : 0,
                CreationDate = DateTime.Now,
                ChangeDate = DateTime.Now,
            };
        }

        public string GetFilePath(Guid fileid)
        {
            string guid = fileid.ToString("N");

            return Path.Combine(Path.Combine(FilesDirectory, guid.Substring(1, 4)), guid);
        }

        public string GetFilePathFromContent(string subFolder, string fileName, string extension)
        {
            return Path.Combine(Path.Combine(ContentDirectory, subFolder), fileName + "." + extension);
        }

        public Guid GetFileID(string filename)
        {
            return Guid.Parse(filename);
        }

        public string GetFilePath(Guid fileid, bool checkTemp)
        {
            string path = GetFilePath(fileid);

            var fi = new FileInfo(path);

            return fi.Exists ? path : null;
        }

        public FileData GetFileData(IUnitOfWork unitOfWork, int id)
        {
            return unitOfWork.GetRepository<FileData>().Find(f => f.ID == id);
        }

        public Task<FileData> GetFileDataAsync(IUnitOfWork unitOfWork, int id)
        {
            return unitOfWork.GetRepository<FileData>().All().FirstOrDefaultAsync(f => f.ID == id);
        }

        public FileData GetFileData(IUnitOfWork unitOfWork, Guid fileid)
        {
            return unitOfWork.GetRepository<FileData>().Find(f => f.FileID == fileid);
        }

        public Task<FileData> GetFileDataAsync(IUnitOfWork unitOfWork, Guid fileid)
        {
            return unitOfWork.GetRepository<FileData>().All().FirstOrDefaultAsync(f => f.FileID == fileid);
        }
    }

    public class DefaultFileManager : IFileManager
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IFileSystemService _fileSystemService;

        public DefaultFileManager(IUnitOfWorkFactory unitOfWorkFactory, IFileSystemService fileSystemService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _fileSystemService = fileSystemService;
        }

        public int DeleteFiles()
        {
            int i = 0;

            if (!Directory.Exists(_fileSystemService.FilesDirectory)) return 0;

            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                var files = Directory.GetFiles(_fileSystemService.FilesDirectory, "*", SearchOption.AllDirectories);

                var repository = uofw.GetRepository<FileData>();

                foreach (string file in files)
                {
                    var fi = new FileInfo(file);

                    if (fi.LastWriteTime >= DateTime.Now.AddHours(-1)) continue;

                    var guid = _fileSystemService.GetFileID(fi.Name);

                    if (!repository.All().Any(x => x.FileID == guid))
                    {
                        File.Delete(file);
                        i++;
                    }
                }

                var dirs = Directory.GetDirectories(_fileSystemService.FilesDirectory);

                foreach (string dir in dirs)
                {
                    var di = new DirectoryInfo(dir);

                    if (!di.GetFiles().Any())
                    {
                        try
                        {
                            Directory.Delete(dir);
                        }
                        catch (Exception e)
                        {
                            // ignored
                        }
                    }
                }
            }

            return i;
        }

        public int DeleteFileData()
        {
            int i = 0;

            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                var repository = uofw.GetRepository<FileData>();

                var items = repository.All().Select(x => new { ID = x.ID , RowVersion = x.RowVersion });

                foreach (var item in items)
                {
                    var file = new FileData() { ID = item.ID, RowVersion = item.RowVersion };

                    repository.Attach(file);
                    repository.Delete(file);

                    try
                    {
                        uofw.SaveChanges();
                        i++;
                    }
                    catch (Exception)
                    {
                        //Конфликт инструкции DELETE с ограничением REFERENCE. Выполнение данной инструкции было прервано.
                        repository.Detach(file);
                    }
                }
            }

            return i;
        }
    }
}
