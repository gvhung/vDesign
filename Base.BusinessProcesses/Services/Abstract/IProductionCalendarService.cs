using Base.BusinessProcesses.Entities;
using System;

namespace Base.BusinessProcesses.Services.Abstract
{
    public interface IProductionCalendarService
    {
        void CreateCalendar(int year);
        DateTime GetEndDate(DateTime start, int period, PerfomancePeriodType periodType);
        int GetPeriod(DateTime start, DateTime end, PerfomancePeriodType periodType);
    }
}
