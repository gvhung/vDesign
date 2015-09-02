using Base.Ambient;
using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Security;
using Base.Service;
using System;
using System.Linq;

namespace Base.Content.Service.Concrete
{
    public class AnswerService : BaseCategorizedItemService<Answer>, IAnswerService
    {
        public AnswerService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override Answer CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            var dtm = DateTime.Now;
            dtm = new DateTime(dtm.Year, dtm.Month, dtm.Day, dtm.Hour, dtm.Minute, 0);

            return new Answer()
            {
                Author = unitOfWork.GetRepository<User>().Find(u => u.ID == AppContext.SecurityUser.ID),
                Date = dtm
            };
        }

        public override IQueryable<Answer> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);

            return this.GetAll(unitOfWork, hidden).Where(a => (a.ContentCategory.sys_all_parents != null && a.ContentCategory.sys_all_parents.Contains(strID)) || a.ContentCategory.ID == categoryID);
        }

        protected override IObjectSaver<Answer> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Answer> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.Author);
        }
    }
}
