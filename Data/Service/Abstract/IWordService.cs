using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Base;
using Base.Service;
using Data.Entities;

namespace Data.Service.Abstract
{
    public interface IWordService : IService
    {
        string ConvertToHtml(MemoryStream docStream);
        string ConvertToHtml(Guid fileid);
        Task<string> ConvertToHtmlAsync(Guid fileid);
        string GetContent(FileData fileData, bool compressForSearch = false);
    }
}