using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Service;
using System;
using System.Linq;

namespace Base.Content.Service.Concrete
{
    public class CourseCategoryService : BaseCategoryService<CourseCategory>, ICourseCategoryService
    {
        public CourseCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }

        public Exercise[] GetAllExercises(IUnitOfWork unitOfWork, int lessonID)
        {
            var q = this.GetAll(unitOfWork).Where(x => x.ID == lessonID);

            return q.Any()
                ? unitOfWork.GetRepository<Exercise>().Filter(x => x.CategoryID == lessonID).ToArray()
                : new Exercise[0];
        }

        public override void ChangePosition(IUnitOfWork unitOfWork, CourseCategory obj, int? posChangeID, string typePosChange)
        {
            CourseCategory target;

            if (typePosChange == "over")
            {
                if (obj.IsRoot)
                {
                    throw new Exception("Нельзя поместить курс в состав другого курса или в состав занятия");
                }

                target = this.GetAll(unitOfWork).First(x => x.ID == posChangeID);

                if (!target.IsRoot)
                {
                    throw new Exception("Нельзя поместить занятие в состав другого занятия");
                }
            }
            else
            {
                target = this.GetAll(unitOfWork).First(x => x.ID == posChangeID);

                if (obj.IsRoot && !target.IsRoot)
                {
                    throw new Exception("Нельзя поместить курс в состав другого курса или в состав занятия");
                }
                else if (!obj.IsRoot && target.IsRoot)
                {
                    throw new Exception("Занятие должно быть в составе учебного курса");
                }
            }

            base.ChangePosition(unitOfWork, obj, posChangeID, typePosChange);
        }

        public IQueryable<Exercise> GetExercises(IUnitOfWork unitOfWork, int couseID)
        {
            return unitOfWork.GetRepository<Exercise>().All().Where(x => x.CategoryID == couseID);
        }

        protected override IObjectSaver<CourseCategory> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<CourseCategory> objectSaver)
        {
            if (objectSaver.IsNew)
            {
                if (objectSaver.Dest.Level > 1)
                {
                    throw new Exception("Нельзя создавать занятие в составе другого занятия");
                }
            }

            return base.GetForSave(unitOfWork, objectSaver).SaveOneObject(x => x.Image);
        }
    }
}
