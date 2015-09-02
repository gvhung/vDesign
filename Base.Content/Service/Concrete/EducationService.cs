using Base.Ambient;
using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Framework.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Content.Service.Concrete
{
    public class EducationService : IEducationService
    {
        private readonly IExerciseService _exerciseService;
        private readonly ICourseCategoryService _courseService;
        private readonly IJournalEntryService _journalEntryService;
        private readonly ISystemUnitOfWork _systemUnitOfWork;

        public EducationService(IExerciseService exerciseService, ICourseCategoryService courseService, IJournalEntryService journalEntryService, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _exerciseService = exerciseService;
            _courseService = courseService;
            _journalEntryService = journalEntryService;
            _systemUnitOfWork = unitOfWorkFactory.CreateSystem();
        }

        public List<JournalEntry> GetJournal()
        {
            var courses = _courseService.GetAll(_systemUnitOfWork)
                .Where(c => c.ParentID == null)
                .ToList()
                .Select(x => new Course(x))
                .ToList();

            var courseIDs = courses.Select(x => x.ID).ToArray();

            var lessons = _courseService.GetAll(_systemUnitOfWork)
                .Where(c => courseIDs.Contains(c.ParentID ?? 0))
                .ToList()
                .Select(x => new Lesson(x))
                .ToList();

            var lessonIDs = lessons.Select(x => x.ID).ToArray();

            return
                _journalEntryService.GetAll(_systemUnitOfWork)
                    .Where(x => lessonIDs.Contains(x.CategoryID))
                    .ToList();

        }

        public List<JournalEntry> GetUserJournal()
        {
            return GetJournal().Where(x => x.UserID == AppContext.SecurityUser.ID).ToList();
        }

        public List<Course> GetCourses(IUnitOfWork unitOfWork)
        {
            var courses = _courseService.GetAll(unitOfWork).Where(x => x.ParentID == null).ToList().Select(x => new Course(x)).ToList();

            var courseIDs = courses.Select(x => x.ID).ToArray();

            var lessons = _courseService.GetAll(unitOfWork)
                    .Where(c => courseIDs.Contains(c.ParentID ?? 0)).ToList()
                    .Select(c => new Lesson(c)).ToList();

            var lessonIDs = lessons.Select(x => x.ID).ToArray();

            var journal = _journalEntryService.GetAll(_systemUnitOfWork).Where(x => x.UserID == AppContext.SecurityUser.ID && lessonIDs.Contains(x.CategoryID)).ToList();

            lessons.ForEach(lesson =>
            {
                var jentry = journal.FirstOrDefault(j => j.CategoryID == lesson.ID);

                if (jentry != null)
                {
                    lesson.Complete = jentry.End != null;
                }
            });

            courses.ForEach(course =>
            {
                var list = lessons.Where(l => l.CourseID == course.ID).ToList();

                int count = list.Count();

                if (count <= 0) return;

                int complete = list.Count(x => x.Complete);

                course.Pct = complete * 100 / count;
            });

            return courses;
        }

        public Course GetCourse(IUnitOfWork unitOfWork, int courseID)
        {
            var category = _courseService.Get(unitOfWork, courseID);

            if (category != null)
            {
                var course = new Course(category)
                {
                    Lessons =
                        _courseService.GetAll(unitOfWork)
                            .Where(c => c.ParentID == courseID).ToList()
                            .Select(c => new Lesson(c)).ToList()
                };

                var ids = course.Lessons.Select(x => x.ID).ToArray();

                var exercises = _exerciseService.GetAll(_systemUnitOfWork).Where(x => ids.Contains(x.CategoryID)).ToList();

                var journal = _journalEntryService.GetAll(_systemUnitOfWork).Where(x => x.UserID == AppContext.SecurityUser.ID && ids.Contains(x.CategoryID)).ToList();

                course.Lessons.ToList().ForEach(lesson =>
                {
                    var jentry = journal.FirstOrDefault(j => j.CategoryID == lesson.ID);

                    if (jentry != null)
                    {
                        lesson.Complete = jentry.End != null;

                        if (!lesson.Complete)
                        {
                            var exe = exercises.Where(x => x.CategoryID == lesson.ID);

                            if (exe != null)
                            {
                                int count = exe.Count();

                                if (count > 0)
                                {
                                    int complete = jentry.ExerciseResults.Count(x => x.End != null);

                                    lesson.Pct = complete * 100 / count;
                                }
                            }

                            lesson.LastExerciseID = jentry.ExerciseResults.Where(r => r.End == null).Select(r => r.ExerciseID).FirstOrDefault();
                        }
                    }
                });

                return course;
            }

            return null;
        }
        public Lesson GetLesson(IUnitOfWork unitOfWork, int lessonID)
        {
            var cat = _courseService.Get(unitOfWork, lessonID);

            if (cat == null) return null;

            var journalEntry = _journalEntryService.GetLastJournalEntry(unitOfWork, lessonID, complete: null);

            return new Lesson(cat)
            {
                LastExerciseID = _journalEntryService.GetLastExerciseID(unitOfWork, lessonID),
                Complete = journalEntry.With(x => x.End) != null,
                Exercises =
                    _exerciseService.GetAll(_systemUnitOfWork)
                        .Where(x => x.CategoryID == lessonID)
                        .OrderBy(x => x.SortOrder).ToList()
                        .Select(x => new ExerciseUI(x, journalEntry.With(y => y.ExerciseResults.FirstOrDefault(r => r.ExerciseID == x.ID)))).ToList(),
            };
        }

        public void StartLesson(IUnitOfWork unitOfWork, int lessonID)
        {
            _journalEntryService.StartLesson(unitOfWork, this.GetLesson(unitOfWork, lessonID));
        }

        public void StartOverLesson(IUnitOfWork unitOfWork, int lessonID)
        {
            _journalEntryService.StartOverLesson(unitOfWork, this.GetLesson(unitOfWork, lessonID));
        }

        public void CompleteLesson(IUnitOfWork unitOfWork, int lessonID)
        {
            _journalEntryService.CompleteLesson(unitOfWork, this.GetLesson(unitOfWork, lessonID));
        }

        public ExerciseUI StartExercise(IUnitOfWork unitOfWork, int lessonID, int exerciseID)
        {
            var lesson = this.GetLesson(unitOfWork, lessonID);

            if (lesson != null)
            {
                _journalEntryService.StartExercise(unitOfWork, lesson, exerciseID);

                return lesson.Exercises.FirstOrDefault(x => x.ID == exerciseID);
            }

            return null;
        }

        public ExerciseCheckResult CheckExercise(IUnitOfWork unitOfWork, ExerciseCheckInput input,
           Func<IEnumerable<ContentCheckResult>, bool> resultStrategy = null,
           Func<ContentResult, ContentResult, bool> contentResultStrategy = null)
        {
            if (input == null) throw new ArgumentNullException("input");

            if (resultStrategy == null)
                resultStrategy = results => results.All(x => x.Passed);

            if (contentResultStrategy == null)
                contentResultStrategy = (o, i) => i.Value.ToUpper() == o.Value.ToUpper();

            var exercise = _exerciseService.Get(unitOfWork, input.ID);

            if (exercise != null && exercise.Content != null)
            {
                var results = exercise.Content.Results.GroupJoin(
                    input.Input,
                    result => result.UID,
                    result => result.UID,
                    (o, r) => new { Origin = o, Results = r })
                    .Select(x => new { x.Origin, Result = x.Results.FirstOrDefault() })
                    .Select(x => CheckContentResult(x.Origin, x.Result, contentResultStrategy));

                var checkResult = new ExerciseCheckResult(resultStrategy, results)
                {
                    Input = input
                };

                _journalEntryService.CompleteExercise(unitOfWork, exercise.CategoryID, new ExerciseResult()
                {
                    ExerciseID = exercise.ID,
                    Exercise = exercise,
                    Points = checkResult.Passed ? 1 : 0,
                    Html = input.Html,
                });

                return checkResult;
            }

            throw new Exception("Exercise not found exception");
        }

        private ContentCheckResult CheckContentResult(ContentResult origin, ContentResult input, Func<ContentResult, ContentResult, bool> contentResultStrategy)
        {
            return new ContentCheckResult
            {
                Input = input,
                Origin = origin,
                Passed = contentResultStrategy(origin, input),
                UID = input.UID
            };
        }
    }
}