using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Base;
using Base.DAL;
using Base.Security.Service;
using Base.Service;
using Data.Entities;
using Data.Service.Abstract;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using DocumentFormat.OpenXml.Wordprocessing;
using Framework;
using Framework.Morphology;
using Framework.Wrappers;
using OpenXmlPowerTools;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;

namespace Data.Service.Concrete
{
    public class WordService : IWordService
    {
        private const string AnswersTemplateName = "npa_answers.docx";
        private const string AnswersCacheKey = "{5AF87F19-EAAF-4C21-AE24-75BF7225F8B0}";

        //private const string SummaryTemplateName = "npa_summary.docx";
        //private static readonly string SummaryCacheKey = Guid.NewGuid().ToString("N");

        private static readonly object _cacheLocker = new object();

        private readonly ISecurityUserService _securityUserService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IPathHelper _pathHelper;
        private readonly ICacheWrapper _cache;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public WordService(ISecurityUserService securityUserService,
            IFileSystemService fileSystemService,
            IPathHelper pathHelper,
            ICacheWrapper cache,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _securityUserService = securityUserService;
            _fileSystemService = fileSystemService;
            _pathHelper = pathHelper;
            _cache = cache;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public string GetContent(FileData fileData, bool compressForSearch = false)
        {
            string path = _fileSystemService.GetFilePath(fileData.FileID, true);
            if (path == null) return null;

            using (WordprocessingDocument wdDoc = WordprocessingDocument.Open(path, false))
            {
                MainDocumentPart mainPart = wdDoc.MainDocumentPart;

                var body = mainPart.Document.Body.Descendants<Paragraph>().Select(x => x.InnerText).Aggregate("", (x, y) => x + " " + y);

                if (compressForSearch)
                {
                    return MorphologyHelper.SearchString(body).Distinct().Aggregate("", (x, y) => x + " " + y);
                }
                else
                {
                    return body;
                }
            }
        }

        private async Task<FileData> _SaveReport(MemoryStream sourceStream, string fileName)
        {
            Guid fileid = Guid.NewGuid();
            string targetPath = _fileSystemService.GetFilePath(fileid);

            FileInfo fi = new FileInfo(targetPath);
            if (fi.Exists) fi.Delete();
            if (!fi.Directory.Exists) fi.Directory.Create();

            sourceStream.Position = 0;
            FileStream destinationStream = File.Create(targetPath);
            await sourceStream.CopyToAsync(destinationStream).ContinueWith(_ => destinationStream.Dispose());

            fi.Refresh();
            if (!fi.Exists) throw new Exception("Ошибка создания файла");

            FileData fileData = new FileData
            {
                FileID = fileid,
                FileName = (fileName.Length > 240 ? fileName.Substring(0, 240) : fileName) + ".docx",
                Size = fi.Length,
                CreationDate = DateTime.Now,
                ChangeDate = DateTime.Now,
            };

            return fileData;
        }

        public string ConvertToHtml(Guid fileid)
        {
            var path = _fileSystemService.GetFilePath(fileid);

            if (!File.Exists(path)) throw new InvalidOperationException("Файл не найден");

            using (var ms = new MemoryStream())
            {
                using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.CopyTo(ms);
                }
                return ConvertToHtml(ms);
            }
        }

        public async Task<string> ConvertToHtmlAsync(Guid fileid)
        {
            string path = _fileSystemService.GetFilePath(fileid);

            if (!File.Exists(path)) throw new InvalidOperationException("Файл не найден");

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await fs.CopyToAsync(ms);
                }
                return ConvertToHtml(ms);
            }
        }

        private static readonly object _convertLocker = new object();

        public string ConvertToHtml(MemoryStream docStream)
        {
            lock (_convertLocker)
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Open(docStream, true, new OpenSettings { AutoSave = false }))
                {
                    return _ConvertToHtml(doc);
                }
            }
        }

        private string _ConvertToHtml(WordprocessingDocument doc)
        {
            lock(_convertLocker)
            {
                HtmlConverterSettings settings = new HtmlConverterSettings
                {
                    #region import images
                    //                        ImageHandler = imageInfo =>
                    //                        {
                    //                            DirectoryInfo localDirInfo = new DirectoryInfo(imageDirectoryName);
                    //                            if (!localDirInfo.Exists)
                    //                                localDirInfo.Create();
                    //                            ++imageCounter;
                    //                            string extension = imageInfo.ContentType.Split('/')[1].ToLower();
                    //
                    //                            ImageFormat imageFormat = null;
                    //                            switch (extension)
                    //                            {
                    //                                case "jpeg":
                    //                                    // Convert the .jpeg file to a .png file.
                    //                                    extension = "png";
                    //                                    imageFormat = ImageFormat.Png;
                    //                                    break;
                    //                                case "bmp":
                    //                                    imageFormat = ImageFormat.Bmp;
                    //                                    break;
                    //                                case "png":
                    //                                    imageFormat = ImageFormat.Png;
                    //                                    break;
                    //                                case "tiff":
                    //                                    imageFormat = ImageFormat.Tiff;
                    //                                    break;
                    //                            }
                    //
                    //                            if (imageFormat == null) return null;
                    //
                    //                            string imageFileName = String.Format("{0}{1}/img.{2}", imageDirectoryName, imageCounter, extension);
                    //                            try
                    //                            {
                    //                                imageInfo.Bitmap.Save(imageFileName, imageFormat);
                    //                            }
                    //                            catch (ExternalException)
                    //                            {
                    //                                return null;
                    //                            }
                    //
                    //                            XElement img = new XElement(Xhtml.img,
                    //                                new XAttribute(NoNamespace.src, imageFileName),
                    //                                imageInfo.ImgStyleAttribute,
                    //                                imageInfo.AltText != null ?
                    //                                    new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                    //
                    //                            return img;
                    //                        }
                    #endregion
                };

                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();

                CultureInfo ci2 = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci2.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = ci2;

                XElement html = HtmlConverter.ConvertToHtml(doc, settings);

                Thread.CurrentThread.CurrentCulture = ci;

                return html.ToStringNewLineOnAttributes();
            }
        }
    }
}
