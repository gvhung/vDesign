using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Service;
using System.Collections.Generic;
using System.Linq;

namespace Base.Content.Service.Concrete
{
    public class QuestionContentCategoryService : BaseCategoryService<ContentCategory>, IQuestionContentCategoryService
    {
        private readonly IContentCategoryService _contentCategoryService;
        private readonly IQuestionService _questionService;

        public QuestionContentCategoryService(IBaseObjectServiceFacade facade, IQuestionService questionService, IContentCategoryService contentCategoryService)
            : base(facade)
        {
            _questionService = questionService;
            _contentCategoryService = contentCategoryService;
        }

        public override IQueryable<ContentCategory> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            var categories = _questionService.GetAll(unitOfWork, hidden: null).Select(x => x.ContentCategory).Distinct().ToList();

            var ids = new List<int>();

            foreach (var category in categories)
            {
                if (category.ParentID != null)
                {
                    ids.AddRange(category.sys_all_parents.Split(HCategory.Seperator).Select(x => HCategory.IdToInt(x)));
                }

                ids.Add(category.ID);
            }

            return _contentCategoryService.GetAll(unitOfWork, hidden).Where(x => ids.Contains(x.ID));
        }
    }
}
