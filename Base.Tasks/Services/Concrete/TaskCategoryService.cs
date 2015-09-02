using Base.DAL;
using Base.Entities.Complex;
using Base.Service;
using Base.Task.Entities;
using System.Linq;


namespace Base.Task.Services
{
    public class TaskCategoryService : BaseCategoryService<TaskCategory>, ITaskCategoryService
    {
        public TaskCategoryService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }

        public override TaskCategory Create(IUnitOfWork unitOfWork, TaskCategory obj)
        {
            var category = base.Create(unitOfWork, obj);
            
            category.SetParent(category.Parent_);

            Update(unitOfWork, category);

            //NOTE: Check
            //this.UnitOfWork.SaveChanges();
            
            return category;
        }

        public void DuplicateCategory(IUnitOfWork unitOfWork, HCategory cat, string SysName)
        {
            var rootCategory = GetAll(unitOfWork).FirstOrDefault(x => x.SysName == SysName);
            var lbObject = new LinkBaseObject(cat);

            var rootIsNew = false;

            if (rootCategory == null)
            {
                rootCategory = new TaskCategory()
                {
                    Name = SysName,
                    SysName = SysName
                };

                Create(unitOfWork, rootCategory);

                rootIsNew = true;
            }

            var currentSysName = lbObject.ToString();

            var categoryForDuplicate = GetAll(unitOfWork).FirstOrDefault(x => x.SysName == currentSysName);

            if (categoryForDuplicate == null)
            {
                categoryForDuplicate = new TaskCategory()
                {
                    Name = cat.Name,
                    SysName = currentSysName
                };

                var currentParentSysName = "";
                TaskCategory parentCategory = null;

                if (cat.ParentID.HasValue)
                {
                    currentParentSysName = string.Format("{0}_{1}", cat.GetType(), cat.ParentID);
                    parentCategory = GetAll(unitOfWork).FirstOrDefault(x => x.SysName == currentParentSysName);
                }

                if (parentCategory != null)
                {
                    categoryForDuplicate.SetParent(parentCategory);
                    Create(unitOfWork, categoryForDuplicate);
                }
                else
                {
                    if (!rootIsNew)
                    {
                        categoryForDuplicate.SetParent(rootCategory);
                        Create(unitOfWork, categoryForDuplicate);
                    }
                    else
                    {
                        categoryForDuplicate.Parent_ = rootCategory;
                        Create(unitOfWork, categoryForDuplicate);
                    }
                }
            }
            else
            {
                categoryForDuplicate.Name = cat.Name;
                Update(unitOfWork, categoryForDuplicate);
            }

            //NOTE: Check
            //this.UnitOfWork.SaveChanges();
        }
    }
}
