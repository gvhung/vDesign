using Base.Task.Entities;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Base.BusinessProcesses.Entities
{
    public class RelatedTaskCollection : BaseObject, IEnumerable<RelatedTask>
    {
        public virtual ICollection<RelatedTask> RelatedTasks { get; set; }

        public int TaskStepID { get; set; }
        public virtual TaskStep TaskStep { get; set; }

        private bool? _isComplited;
        [NotMapped]
        public bool IsComplited
        {
            get
            {
                if (_isComplited == null)
                {
                    switch (TaskStep.ConditionalOperator)
                    {
                        case ConditionalOperator.One:
                            _isComplited = RelatedTasks.Any(x => x.Status == TaskStatus.Complete);
                            break;
                        case ConditionalOperator.All:
                            _isComplited = RelatedTasks.All(x => x.Status == TaskStatus.Complete);
                            break;
                        default:
                            _isComplited = false;
                            break;
                    }
                }

                return _isComplited.Value;
            }
        }

        public IEnumerator<RelatedTask> GetEnumerator()
        {
            return RelatedTasks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}