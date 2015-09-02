using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Service;
using System;
using System.Linq;

namespace Base.Content.Service.Concrete
{
    public class ExerciseService : BaseCategorizedItemService<Exercise>, IExerciseService
    {
        public ExerciseService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override IQueryable<Exercise> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);

            return this.GetAll(unitOfWork, hidden).Where(a => (a.CourseCategory.sys_all_parents != null && a.CourseCategory.sys_all_parents.Contains(strID)) || a.CourseCategory.ID == categoryID);
        }

        protected override IObjectSaver<Exercise> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Exercise> objectSaver)
        {
            var current = objectSaver.Dest;

            if (unitOfWork.GetRepository<CourseCategory>().All().Any(x => x.ID == current.CategoryID && x.IsRoot))
            {
                throw new Exception("Учебное задание должно быть в составе занятия, а не курса");
            }

            return base.GetForSave(unitOfWork, objectSaver);
        }
    }
}
