using Base.Content.Entities;
using Base.DAL;
using Base.Service;

namespace Base.Content.Service.Abstract
{
    public interface IQuestionService : IBaseCategorizedItemService<Question>
    {
        void AddQuestion(IUnitOfWork unitOfWork, int categoryID, string question);
    }
}
