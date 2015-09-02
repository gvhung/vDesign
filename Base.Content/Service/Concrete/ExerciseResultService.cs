using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Service;
using System;

namespace Base.Content.Service.Concrete
{
    public class ExerciseResultService : BaseObjectService<ExerciseResult>, IExerciseResultService
    {
        public ExerciseResultService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override ExerciseResult CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            var dtm = DateTime.Now;

            dtm = new DateTime(dtm.Year, dtm.Month, dtm.Day, dtm.Hour, dtm.Minute, 0);

            return new ExerciseResult()
            {
                Start = dtm,
            };
        }

        protected override IObjectSaver<ExerciseResult> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<ExerciseResult> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.Exercise);
        }
    }
}
