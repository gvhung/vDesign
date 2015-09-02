using Framework;
using System;
using System.Collections.Generic;
using System.Web;
using Framework.Wrappers;
using System.Web.Mvc;
using Base;
using WebUI.Helpers;
using Base.Service;
using Base.FileStorage;
using System.IO;
using System.Threading.Tasks;
using Data.Service.Abstract;
using DocumentFormat.OpenXml.Packaging;
using WebUI.Authorize;

namespace WebUI.Controllers
{
    public class FileDataController : BaseController
    {
        private readonly IFileStorageItemService _fileStorageItemService;
        private readonly IWordService _wordService;

        public FileDataController(IBaseControllerServiceFacade baseServiceFacade, IFileStorageItemService fileStorageItemService, IWordService wordService)
            : base(baseServiceFacade)
        {
            _fileStorageItemService = fileStorageItemService;
            _wordService = wordService;
        }

        public JsonNetResult SaveFile()
        {
            HttpPostedFileBase file = null;

            if (Request.Files.Count > 0)
            {
                file = Request.Files[0];
            }

            var wrapp = DependencyResolver.Current.GetService<IPostedFileWrapper>();
            wrapp.SetItem(file);

            return new JsonNetResult(FileSystemService.SaveFile(wrapp));
        }

        public JsonNetResult SaveFiles()
        {
            var res = new List<FileData>();

            if (Request.Files.Count == 0) return new JsonNetResult(res);

            var wrapp = DependencyResolver.Current.GetService<IPostedFileWrapper>();

            foreach (string key in Request.Files)
            {
                wrapp.SetItem(Request.Files[key]);

                var fd = FileSystemService.SaveFile(wrapp);

                if (fd != null)
                {
                    res.Add(fd);
                }
            }

            return new JsonNetResult(res);
        }

        public JsonNetResult DeleteFiles(string[] fileNames, bool isNewObject)
        {
            return new JsonNetResult(null);
        }

        [DeleteTempFileFilter]
        public ActionResult GetTempFile(string id, string fileName)
        {
            var pathHelper = DependencyResolver.Current.GetService<IPathHelper>();

            string filePath = Path.Combine(pathHelper.GetTempDirectory(), id);

            return File(filePath, MimeMapping.GetMimeMapping(filePath), fileName);
        }

        public JsonNetResult GetWidget(int id)
        {
            FileStorageItem fileStorageItem;

            using(var uow = CreateUnitOfWork())
            {
                fileStorageItem = _fileStorageItemService.GetFileStorageItem(uow, id);
            }

            string extension = "";

            if (fileStorageItem != null && fileStorageItem.File != null)
            {
                string ext = Path.GetExtension(fileStorageItem.File.FileName);

                if (ext != null)
                    extension = ext.Replace(".", "").ToLower();
            }

            string view = "Unknown";

            var vd = new ViewDataDictionary();

            switch (extension)
            {
                case "gif":
                case "jpeg":
                case "jpg":
                case "png":
                case "tif":
                case "tiff":
                    view = "Image";
                    break;
                //case "pdf":
                //    view = "Book";
                //    break;

                case "wmv":
                case "mp4":
                case "avi":
                    view = "Video";
                    break;

                //case "wma":
                //case "mp3":
                //    view = "Audio";
                //    break;
            }

            return new JsonNetResult(new
            {
                html = RenderPartialViewToString(view, fileStorageItem, vd)
            });
        }

        private static readonly object ShowDocLocker = new object { };

        [AllowGuest]
        public ActionResult ShowDoc(Guid id)
        {
            string key = String.Format("F15FACBA-840A-44BE-A272-38C181560855:{0}", id);

            if (this.CacheWrapper[key] != null)
                return Content(CacheWrapper[key].ToString());

            lock (ShowDocLocker)
            {
                if (this.CacheWrapper[key] == null)
                {
                    ViewBag.FileID = id;

                    CacheWrapper[key] = RenderPartialViewToString("_Word");
                }
            }

            return Content(CacheWrapper[key].ToString());
        }

        private static readonly object GetDocContentLocker = new object { };

        [AllowGuest]
        public ContentResult GetDocContent(Guid id)
        {
            string key = String.Format("C6C1860A-0E49-4D17-AF41-57BAEDA2AA95:{0}", id);

            if (this.CacheWrapper[key] != null)
                return Content(CacheWrapper[key].ToString());

            lock (GetDocContentLocker)
            {
                if (this.CacheWrapper[key] == null)
                {
                    try
                    {
                        CacheWrapper[key] = _wordService.ConvertToHtml(id);

                    }
                    catch (Exception)
                    {
                        CacheWrapper[key] = FormatError("Некорректный формат файла");

                    }
                }
            }

            return Content(CacheWrapper[key].ToString());
        }

        private static string FormatError(string message)
        {
            return String.Format("<div class='error'>{0}</div>", message);
        }
    }
}