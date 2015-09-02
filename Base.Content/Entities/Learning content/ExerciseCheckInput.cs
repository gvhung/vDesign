using System;
using System.Collections.Generic;

namespace Base.Content.Entities
{
    public class ExerciseCheckInput
    {
        public int ID { get; set; }
        public string Html { get; set; }
        public IEnumerable<ContentResult> Input { get; set; }
        public ExerciseCheckInput() { }
        public ExerciseCheckInput(int id, IEnumerable<ContentResult> input)
        {
            this.ID = id;
            this.Input = input;
        }
    }

    public class ExerciseCheckResult
    {
        private readonly Func<IEnumerable<ContentCheckResult>, bool> _resultStrategy;

        public ExerciseCheckResult(Func<IEnumerable<ContentCheckResult>, bool> resultStrategy, IEnumerable<ContentCheckResult> results)
        {
            _resultStrategy = resultStrategy;

            Results = results;
        }

        public string Message { get; set; }
        public IEnumerable<ContentCheckResult> Results { get; set; }
        public ExerciseCheckInput Input { get; set; }

        private bool? _passed;
        public bool Passed
        {
            get { return _passed ?? (bool)(_passed = _resultStrategy(this.Results)); }
        }
    }

    public class ContentCheckResult
    {
        public string UID { get; set; }
        public bool Passed { get; set; }
        public string Message { get; set; }

        public ContentResult Origin { get; set; }
        public ContentResult Input { get; set; }
    }
}