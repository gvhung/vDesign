using Base.Attributes;
using Base.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Base.BusinessProcesses.Entities
{
    [Table("BPWorkflows")]
    public class Workflow : BaseObject, ICategorizedItem
    {
        public Workflow() { }

        public Workflow(Workflow src)
        {
            CategoryID = src.CategoryID;
            CuratorsCategoryID = src.CuratorsCategoryID;
            CreatorID = src.CreatorID;
            CuratorID = src.CuratorID;
            Description = src.Description;
            ObjectType = src.ObjectType;
            PerformancePeriod = src.PerformancePeriod;
            Scheme = src.Scheme;
            SystemName = src.SystemName;
            Title = src.Title;
            LoadSteps(src.Steps);
        }

        private void LoadSteps(IEnumerable<Step> srcSteps)
        {
            Steps = new List<Step>();

            foreach (var step in srcSteps.Where(x => !x.Hidden))
            {
                var stepType = step.StepType;
                Step dest;

                switch (stepType)
                {
                    case FlowStepType.BranchingStep:
                        dest = copyBranchingStep(step as BranchingStep);
                        break;

                    case FlowStepType.CreateObjectTask:
                        dest = copyCreateObjectStep(step as CreateObjectStep);
                        break;

                    case FlowStepType.EndStep:
                        dest = new EndStep(step as EndStep);
                        break;

                    case FlowStepType.GotoStep:
                        dest = new GotoStep(step as GotoStep);
                        break;

                    case FlowStepType.ParalleizationStep:
                        dest = new ParallelizationStep(step as ParallelizationStep);
                        break;

                    case FlowStepType.ParallelEndStep:
                        dest = new ParallelEndStep(step as ParallelEndStep);
                        break;

                    case FlowStepType.RestoreStep:
                        dest = new RestoreStep(step as RestoreStep);
                        break;

                    case FlowStepType.Stage:
                        dest = copyStage(step as Stage);
                        break;

                    case FlowStepType.TaskStep:
                        dest = copyTaskStep(step as TaskStep);
                        break;

                    case FlowStepType.WorkflowOwnerStep:
                        dest = copyWorkflowOwnerStep(step as WorkflowOwnerStep);
                        break;

                    default:
                        throw new Exception("Не удалось определить тип шага");
                }
                dest.LoadOutputs(step.BaseOutputs);
                Steps.Add(dest);
            }

            foreach (var result in Steps.Where(x => x.BaseOutputs != null).SelectMany(x => x.BaseOutputs)
            .Join(Steps, o => o.NextStepViewID, i => i.ViewID, (o, i) => new { Output = o, NextStep = i }))
            {
                result.Output.NextStep = result.NextStep;
            }
        }

        
        private BranchingStep copyBranchingStep(BranchingStep src)
        {
            BranchingStep bs = new BranchingStep(src);
            bs.LoadOutputs(src.Outputs);
            return bs;
        }
        private CreateObjectStep copyCreateObjectStep(CreateObjectStep src)
        {
            var cos = new CreateObjectStep(src);
            cos.LoadInitItems(src.InitItems);
            return cos;
        }
        private Stage copyStage(Stage src)
        {
            var s = new Stage(src);
            if (src.AssignedToUsers != null) s.LoadAssignedUsers(src.AssignedToUsers);
            if (src.Outputs != null) s.LoadOutputs(src.Outputs);
            return s;
        }
        private TaskStep copyTaskStep(TaskStep src)
        {
            var ts = new TaskStep(src);
            ts.LoadAssignedToUsers(src.AssignedToUsers);
            return ts;
        }
        private WorkflowOwnerStep copyWorkflowOwnerStep(WorkflowOwnerStep src)
        {
            var wfows = new WorkflowOwnerStep(src);
            wfows.LoadChildWF(src.ChildWorkflow);
            return wfows;
        }


        private const int DefaultID = -1;

        public int CategoryID { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryID")]
        // ReSharper disable once InconsistentNaming
        public virtual WorkflowCategory Category_ { get; set; }

        #region ICategorizedItem
        HCategory ICategorizedItem.Category
        {
            get { return Category_; }
        }
        #endregion

        /// <summary>
        /// Represents default workflow id 
        /// </summary>
        public static int Default
        {
            get { return DefaultID; }
        }

        [ListView]
        [MaxLength(255)]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }

        [ListView]
        [DetailView(Name = "Описание")]
        public string Description { get; set; }
        public string Scheme { get; set; }

        [DetailView(Name = "Объект", Required = true)]
        [ListView]
        [PropertyDataType("ListWFObjects")]
        public string ObjectType { get; set; }

        public int? CuratorID { get; set; }

        [DetailView(Name = "Куратор", Required = true)]
        [ListView]
        public virtual User Curator { get; set; }


        public int? CuratorsCategoryID { get; set; }

        [DetailView(Name = "Категория кураторов")]
        [ListView]
        public virtual UserCategory CuratorsCategory { get; set; }


        [PropertyDataType("Duration"), DetailView(Name = "Срок исполнения", Order = 7)]
        public int PerformancePeriod { get; set; }

        [DetailView(Name = "Этапы", HideLabel = true, TabName = "[1]Схема бизнес-процесса")]
        [PropertyDataType("BPWorkflow")]
        public virtual ICollection<Step> Steps { get; set; }

        public int? CreatorID { get; set; }
        public virtual User Creator { get; set; }

        public DateTime? CreatedDate { get; set; }

        [ListView("Шаблон", Visible = false)]
        public bool IsTemplate { get; set; }

        public bool CreateTemplate { get; set; }

        [ListView]
        [MaxLength(255)]
        [DetailView(Name = "Системное имя")]
        public string SystemName { get; set; }
    }
}