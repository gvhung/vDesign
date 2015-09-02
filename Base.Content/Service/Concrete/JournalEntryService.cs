using Base.Ambient;
using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Content.Service.Concrete
{
    public class JournalEntryService : BaseCategorizedItemService<JournalEntry>, IJournalEntryService
    {
        public JournalEntryService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        public override IQueryable<JournalEntry> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);
            return this.GetAll(unitOfWork, hidden).Where(a => (a.CourseCategory.sys_all_parents != null && a.CourseCategory.sys_all_parents.Contains(strID)) || a.CourseCategory.ID == categoryID);
        }

        public JournalEntry StartLesson(IUnitOfWork unitOfWork, Lesson lesson)
        {
            var currentItem = this.GetLastJournalEntry(unitOfWork, lesson.ID, complete: null);

            if (currentItem != null) return currentItem;

            var ex = lesson.Exercises.FirstOrDefault();

            if (ex != null)
                currentItem = new JournalEntry()
                {
                    UserID = AppContext.SecurityUser.ID,
                    Start = DateTime.Now,
                    End = null,
                    ExerciseResults = new List<ExerciseResult>()
                    {
                        new ExerciseResult(){
                            ExerciseID = ex.ID,
                            Start = DateTime.Now,
                        }
                    },
                    CategoryID = lesson.ID,
                };


            return this.Create(unitOfWork, currentItem);
        }

        public void StartOverLesson(IUnitOfWork unitOfWork, Lesson lesson)
        {
            var currentItem = this.GetLastJournalEntry(unitOfWork, lesson.ID, complete: null);

            if (currentItem == null) return;

            var repository = unitOfWork.GetRepository<ExerciseResult>();

            currentItem.ExerciseResults.ToList().ForEach(x => repository.Delete(x));

            unitOfWork.GetRepository<JournalEntry>().Delete(currentItem);

            unitOfWork.SaveChanges();
        }

        public JournalEntry CompleteLesson(IUnitOfWork unitOfWork, Lesson lesson)
        {
            var currentItem = this.GetLastJournalEntry(unitOfWork, lesson.ID);

            if (currentItem == null) return null;

            if (currentItem.ExerciseResults.Any(x => x.End == null && x.Exercise.Content.HasInteractive))
            {
                throw new Exception("Не все задания решены");
            }

            currentItem.ExerciseResults.Where(x => x.End == null).ToList().ForEach(x =>
            {
                x.Points = 1;
                x.End = DateTime.Now;
            });

            currentItem.End = DateTime.Now;

            return this.Update(unitOfWork, currentItem);
        }

        public JournalEntry GetLastJournalEntry(IUnitOfWork unitOfWork, int lessonID, bool? complete = false)
        {
            var q = this.GetAll(unitOfWork)
                  .Where(x => x.UserID == AppContext.SecurityUser.ID && x.CategoryID == lessonID);

            if (complete != null)
            {
                if ((bool)complete)
                    q = q.Where(x => x.End != null);
                else
                    q = q.Where(x => x.End == null);
            }

            return q.OrderByDescending(x => x.ID).FirstOrDefault();
        }

        public int? GetLastExerciseID(IUnitOfWork unitOfWork, int lessonID)
        {
            var currentItem = this.GetLastJournalEntry(unitOfWork, lessonID);

            return currentItem != null ? currentItem.ExerciseResults.OrderByDescending(x => x.ID).Select(x => x.ExerciseID ?? null).FirstOrDefault() : null;
        }

        public ExerciseResult StartExercise(IUnitOfWork unitOfWork, Lesson lesson, int exerciseID)
        {
            var journalEntry = this.GetLastJournalEntry(unitOfWork, lesson.ID) ?? this.StartLesson(unitOfWork, lesson);

            if (journalEntry.ExerciseResults == null)
            {
                journalEntry.ExerciseResults = new List<ExerciseResult>();
            }

            var exercise = lesson.Exercises.FirstOrDefault(x => x.ID == exerciseID);

            if (journalEntry.ExerciseResults.Any())
            {
                var results = journalEntry.ExerciseResults
                    .Where(x =>
                        x.ExerciseID != exercise.ID &&
                        x.End == null &&
                        x.Exercise != null &&
                        x.Exercise.SortOrder < exercise.SortOrder &&
                        !x.Exercise.Content.HasInteractive);

                foreach (var source in results)
                {
                    source.Points = 1;
                    source.End = DateTime.Now;
                }
            }

            var exerciseResult = journalEntry.ExerciseResults.FirstOrDefault(x => x.ExerciseID == exercise.ID);

            if (exerciseResult == null)
            {
                exerciseResult = new ExerciseResult()
                {
                    ExerciseID = exercise.ID,
                    Start = DateTime.Now,
                };

                journalEntry.ExerciseResults.Add(exerciseResult);
            }

            var updateJournalEntry = this.Update(unitOfWork, journalEntry);

            return updateJournalEntry.ExerciseResults.FirstOrDefault(x => x.ExerciseID == exercise.ID);
        }

        public ExerciseResult CompleteExercise(IUnitOfWork unitOfWork, int lessonID, ExerciseResult exercise)
        {
            var journalEntry = this.GetLastJournalEntry(unitOfWork, lessonID);

            if (journalEntry == null) return null;

            var exerciseResult = journalEntry.ExerciseResults.FirstOrDefault(x => x.ExerciseID == exercise.ExerciseID);

            if (exerciseResult == null) return null;

            exerciseResult.Points = exercise.Points;
            exerciseResult.Html = exercise.Html;
            exerciseResult.End = DateTime.Now;

            unitOfWork.GetRepository<ExerciseResult>().Update(exerciseResult);
            unitOfWork.SaveChanges();

            return null;
        }
    }
}
