using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base;
using Base.DAL;
using Base.Security.Service.Abstract;
using Base.Service;

namespace Base.FileStorage
{
    public class FileStorageCategoryService : BaseCategoryService<FileStorageCategory>, IFileStorageCategoryService
    {
        public FileStorageCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
