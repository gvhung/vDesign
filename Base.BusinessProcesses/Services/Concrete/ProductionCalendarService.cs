using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class ProductionCalendarService : IProductionCalendarService
    {
        private readonly IWeekendService _weekendService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public ProductionCalendarService(IWeekendService weekendService, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _weekendService = weekendService;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public void CreateCalendar(int year)
        {   
            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                if (_weekendService.GetAll(uofw).Any(x => x.Start.Year == year)) return;

                var date = new DateTime(year, 1, 1);

                var weekends = new List<Weekend>();

                while (date.Year == year)
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        weekends.Add(new Weekend()
                        {
                            Start = date
                        });
                    }

                    date = date.AddDays(1);
                }

                if (weekends.Count > 0)
                    _weekendService.CreateCollection(uofw, weekends);
            }
        }

        public DateTime GetEndDate(DateTime start, int period, PerfomancePeriodType periodType)
        {   
            if (periodType == PerfomancePeriodType.Calendar) return start.AddDays(period);

            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                var dtmEnd = start;

                var weekends = uofw.GetRepository<Weekend>().All().Where(x => !x.Hidden).Select(x => x.Start).ToList();
                
                while (period > 0)
                {
                    dtmEnd = dtmEnd.AddDays(1);

                    if (weekends.All(x => x.Date != dtmEnd.Date))
                    {
                        period--;
                    }
                }

                return dtmEnd;
            }
        }

        public int GetPeriod(DateTime start, DateTime end, PerfomancePeriodType periodType)
        {
            throw new NotImplementedException();
        }
    }
}
