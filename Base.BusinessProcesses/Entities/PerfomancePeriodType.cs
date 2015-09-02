using System.ComponentModel;

namespace Base.BusinessProcesses.Entities
{
    public enum PerfomancePeriodType
    {
        [Description("В календарных днях")]
        Calendar = 0,
        [Description("В рабочих днях")]
        Workday = 1,
    }
}