using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Concrete;
using Base.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WebUI.Models.BusinessProcess
{
    public class StageVM
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public IEnumerable<StageAction> Actions { get; set; }
        public User Performer { get; set; }
        public User FromUser { get; set; }
        public ISecurityUser CurrentUser { get; set; }
        public IEnumerable<User> PermittedUsers { get; set; }
        public PerformerType PerformerType { get; set; }
        public int ObjectID { get; set; }
        public string ObjectType { get; set; }
        public string Color { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDateExpected { get; set; }
        public TimeSpan TimeLeft { get; set; }
        public string ElapsedString { get; set; }
        public double ElapsedPercentage { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        private ElapsedStatus? _status;
        public ElapsedStatus? Status
        {
            get
            {
                return _status ??
                       (_status =
                           (ElapsedStatus)
                               Enumerable.Range(0, _ranges.Length).FirstOrDefault(x => _ranges[x] > ElapsedPercentage));
            }
        }



        private static readonly int[] _ranges = { 50, 75, 100, int.MaxValue };


        public StageVM(StagePerform stage)
        {
            Actions = stage.Stage.Outputs.Where(x=>!x.Hidden);
            ID = stage.StageId;
            Name = stage.Stage.Title;
            Color = stage.Stage.Color;
            BeginDate = stage.BeginDate;
            EndDateExpected = stage.BeginDate + TimeSpan.FromMinutes(stage.Stage.PerformancePeriod);
            ElapsedTime = DateTime.Now - stage.BeginDate;
            TimeLeft = TimeSpan.FromMinutes(stage.Stage.PerformancePeriod) - (DateTime.Now - stage.BeginDate);
            ElapsedPercentage = 100 * (DateTime.Now - stage.BeginDate).TotalSeconds / TimeSpan.FromMinutes(stage.Stage.PerformancePeriod).TotalSeconds;
            ElapsedString = ElapsedPercentage.ToString(CultureInfo.GetCultureInfo("en-US"));
            Performer = stage.PerformUser;
            FromUser = stage.FromUser;
        }

    }

    public enum ElapsedStatus
    {
        Good = 0, Info, Warning, Danger
    }
}