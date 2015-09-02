using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base;
using Base.DAL;
using Base.Security.Service.Abstract;
using Base.Service;
using Base.Security;
using System.IO;

namespace Base.FileStorage
{
    public class FileStorageItemService : BaseCategorizedItemService<FileStorageItem>, IFileStorageItemService
    {
        public FileStorageItemService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override IQueryable<FileStorageItem> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);

            return GetAll(unitOfWork, hidden).Cast<FileStorageItem>().Where(a => (a.Category_.sys_all_parents != null && a.Category_.sys_all_parents.Contains(strID)) || a.Category_.ID == categoryID);
        }

        protected override IObjectSaver<FileStorageItem> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<FileStorageItem> objectSaver)
        {
            if (String.IsNullOrEmpty(objectSaver.Dest.Title) && objectSaver.Dest.File != null)
            {
                objectSaver.Dest.Title = Path.GetFileNameWithoutExtension(objectSaver.Dest.File.FileName);
            }

            if (objectSaver.Dest.CategoryID == 0)
            {
                var defaultCategory =
                    unitOfWork.GetRepository<FileStorageCategory>().All().Where(x => !x.Hidden).FirstOrDefault();
                if (defaultCategory != null)
                {
                    objectSaver.Dest.CategoryID = defaultCategory.ID;
                }
            }

            return base.GetForSave(unitOfWork, objectSaver).SaveOneObject(x => x.File);
        }

        public FileStorageItem GetFileStorageItem(IUnitOfWork unitOfWork, int id)
        {
            return this.GetAll(unitOfWork).FirstOrDefault(x => x.File != null && x.FileID == id);
        }

        public FileStorageItem GetFileStorageItem(IUnitOfWork unitOfWork, Guid fileID)
        {
            return this.GetAll(unitOfWork).FirstOrDefault(x => x.File != null && x.File.FileID == fileID);
        }

    }
}
