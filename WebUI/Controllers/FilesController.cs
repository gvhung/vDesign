using System.Xml.Linq;
using Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Base.DAL;
using Base.Service;
using Base.Service.Log;
using Framework.Wrappers;
using WebUI.Extensions;

namespace WebUI.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogService _logService;
        private readonly ICacheWrapper _cacheWrapper;

        public FilesController(IFileSystemService fileSystemService, IUnitOfWorkFactory unitOfWorkFactory, ILogService logService, ICacheWrapper cacheWrapper)
        {
            _fileSystemService = fileSystemService;
            _unitOfWorkFactory = unitOfWorkFactory;
            _logService = logService;
            _cacheWrapper = cacheWrapper;
        }

        private static readonly object GetFileLocker = new object { };

        //[OutputCache(Duration = 3600, VaryByParam = "fileid")]
        public FileResult GetFile(Guid fileid)
        {
            string keyCache = String.Format("7079EFEF-6B84-4863-8E5A-A85B75FC0610:{0}", fileid);

            if (_cacheWrapper[keyCache] != null)
                return _cacheWrapper[keyCache] as FileResult;

            lock (GetFileLocker)
            {
                if (_cacheWrapper[keyCache] == null)
                {
                    using (var uofw = _unitOfWorkFactory.CreateSystem())
                    {
                        var fileData = _fileSystemService.GetFileData(uofw, fileid);

                        if (fileData == null) return null;

                        string path = _fileSystemService.GetFilePath(fileid);

                        byte[] result;

                        if (!System.IO.File.Exists(path)) return null;

                        using (var sourceStream = System.IO.File.Open(path, FileMode.Open))
                        {
                            result = new byte[sourceStream.Length];
                            sourceStream.Read(result, 0, (int)sourceStream.Length);
                        }

                        _cacheWrapper[keyCache] = File(result, MimeMapping.GetMimeMapping(fileData.FileName),
                            fileData.FileName);
                    }
                }
            }

            return (FileResult)_cacheWrapper[keyCache];
        }

        private static readonly object GetImageLocker = new object { };

        [HttpGet]
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "*")]
        public FileResult GetImage(Guid? id, int? width, int? height, string defImage = "", string type = "crop")
        {
            string key = String.Format("394A2D54D6684366918C9D37DE27FD1E-[{0}][{1}][{2}][{3}][{4}]", id, width ?? -1, height ?? -1, defImage, type);

            if (_cacheWrapper[key] != null)
                return _cacheWrapper[key] as FileResult;

            lock (GetImageLocker)
            {
                if (_cacheWrapper[key] == null)
                {
                    //_logService.Log(String.Format("GetImage: {0}", id));

                    var path = "";

                    if (id != null && id != Guid.Empty)
                        path = _fileSystemService.GetFilePath((Guid)id);

                    if (type != null)
                        type = type.ToLower();

                    var converType = type == "crop" ? CrazyImage.ConvertType.Crop : CrazyImage.ConvertType.Frame;

                    var bytes = CrazyImage.GetThumbImage(path, width, height, converType);

                    bytes = bytes != null && bytes.Length > 0
                        ? bytes
                        : CrazyImage.GetThumbImage(
                            defImage == "NoPhoto" ? Properties.Resources.NoPhoto : Properties.Resources.NoImage, width,
                            height, converType);

                    _cacheWrapper[key] = File(bytes, "image/png");
                }
            }

            return (FileResult)_cacheWrapper[key];
        }

        private static readonly object GetSubImageLocker = new object { };

        public FileResult GetEmbeddedImage(string filename)
        {
            Regex regex = new Regex(@"([0-9a-f]{32,38})_(\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var match = regex.Match(filename);
            if (!match.Success) return null;

            string key = String.Format("7D702104-5647-4850-AE1E-DB97167F232D-{0}", filename);
            if (_cacheWrapper[key] != null)
                return _cacheWrapper[key] as FileResult;

            lock (GetSubImageLocker)
            {
                var path = _fileSystemService.GetFilePath(Guid.Parse(match.Groups[1].Value));
                var bytes = System.IO.File.ReadAllBytes(path + "_" + match.Groups[2].Value);

                _cacheWrapper[key] = File(bytes, "image/png");
            }

            return (FileResult)_cacheWrapper[key];
        }
        
        private static readonly object GetXmlLocker = new object();
        public FileResult GetXml(string folder, string fileName)
        {
            string cacheKey = String.Format("38F3B534-EA57-4E59-8448-46C6C1E0E563:{0}:{1}", folder, fileName);

            if (_cacheWrapper[cacheKey] != null)
                return _cacheWrapper[cacheKey] as FileResult;

            lock (GetXmlLocker)
            {
                if (_cacheWrapper[cacheKey] == null)
                {
                    var fileBytes =
                        System.IO.File.ReadAllBytes(_fileSystemService.GetFilePathFromContent(folder, fileName, "xml"));

                    _cacheWrapper[cacheKey] = new FileContentResult(fileBytes, "text/xml");
                }
            }

            return (FileContentResult)_cacheWrapper[cacheKey];
        }
    }
}