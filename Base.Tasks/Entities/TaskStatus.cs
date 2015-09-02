using Base.Attributes;
using System.ComponentModel;

namespace Base.Task.Entities
{
    public enum TaskStatus
    {
        [Color("#5cb85c")]
        [Description("Новое")]
        New = 0,

        [Color("#5cb85c")]
        [Description("В работе")]
        InProcess = 1,

        [Color("#5bc0de")]
        [Description("Завершено")]
        Complete = 2,

        [Color("#f0ad4e")]
        [Description("Неактуально")]
        NotRelevant = 3,

        [Color("#5bc0de")]
        [Description("Просмотрено")]
        Viewed = 4,

        [Color("#5bc0de")]
        [Description("Уточнение")]
        Refinement = 5,

        [Color("#d9534f")]
        [Description("Проверка")]
        Revise = 6,

        [Color("#5bc0de")]
        [Description("Доработка")]
        Rework = 7,

        [Color("#f0ad4e")]
        [Description("Переадресация")]
        Redirection = 8,
    }
}
