namespace Base.Content.Entities
{
    public class ExerciseUI 
    {
        public ExerciseUI(Exercise exercise, ExerciseResult res)
        {
            this.ID = exercise.ID;
            this.SortOrder = exercise.SortOrder;
            this.Title = exercise.Title;
            this.Description = exercise.Description;
            this.HasInteractive = exercise.Content.HasInteractive;
            this.Html = exercise.Content.Html;

            if (res != null)
            {
                this.Html = res.Html ?? this.Html;
                this.Complete = res.End != null;
                this.Points = res.Points;
                this.HasInteractive = this.HasInteractive && !this.Complete;
            }

            if (this.Complete)
            {
                this.Passed = true;

                if (exercise.Content.HasInteractive)
                {
                    this.Passed = this.Points > 0;
                }
            }
        }

        public int ID { get; set; }
        public int SortOrder { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Html { get; set; }
        public bool Complete { get; set; }
        public int Points { get; set; }
        public bool HasInteractive { get; set; }
        public bool Passed { get; set; }
    }
}
