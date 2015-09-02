using Base.Content.Entities;
using Base.DAL;
using Base.Service;

namespace Base.Content.Service.Abstract
{
    public interface IJournalEntryService : IBaseCategorizedItemService<JournalEntry>
    {
        JournalEntry StartLesson(IUnitOfWork unitOfWork, Lesson lesson);
        void StartOverLesson(IUnitOfWork unitOfWork, Lesson lesson);
        JournalEntry CompleteLesson(IUnitOfWork unitOfWork, Lesson lesson);
        JournalEntry GetLastJournalEntry(IUnitOfWork unitOfWork, int lessonID, bool? complete = false);
        int? GetLastExerciseID(IUnitOfWork unitOfWork, int lessonID);
        ExerciseResult StartExercise(IUnitOfWork unitOfWork, Lesson lesson, int exerciseID);
        ExerciseResult CompleteExercise(IUnitOfWork unitOfWork, int lessonID, ExerciseResult exercise);
    }
}
