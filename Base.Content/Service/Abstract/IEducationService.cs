using Base.Content.Entities;
using Base.DAL;
using Base.Service;
using System;
using System.Collections.Generic;

namespace Base.Content.Service.Abstract
{
    public interface IEducationService : IService
    {
        List<JournalEntry> GetJournal();
        List<JournalEntry> GetUserJournal();
        List<Course> GetCourses(IUnitOfWork unitOfWork);
        Course GetCourse(IUnitOfWork unitOfWork, int courseID);
        Lesson GetLesson(IUnitOfWork unitOfWork, int lessonID);
        void StartLesson(IUnitOfWork unitOfWork, int lessonID);
        void CompleteLesson(IUnitOfWork unitOfWork, int lessonID);
        void StartOverLesson(IUnitOfWork unitOfWork, int lessonID);
        ExerciseUI StartExercise(IUnitOfWork unitOfWork, int lessonID, int exerciseID);
        ExerciseCheckResult CheckExercise(IUnitOfWork unitOfWork, ExerciseCheckInput input, Func<IEnumerable<ContentCheckResult>, bool> resultStrategy = null, Func<ContentResult, ContentResult, bool> contentResultStrategy = null);
    }
}