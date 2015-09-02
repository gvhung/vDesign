using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.DAL;
using Base.Service;
using Base.Security;

namespace Base.FileStorage
{
    public interface IFileStorageItemService : IBaseCategorizedItemService<FileStorageItem>
    {
        FileStorageItem GetFileStorageItem(IUnitOfWork unitOfWork, int id);
        FileStorageItem GetFileStorageItem(IUnitOfWork unitOfWork, Guid fileID);
    }
}
