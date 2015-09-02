using Framework.Maybe;
using System;
using System.Collections.Generic;

namespace Base.Content.Entities
{
    public class Course
    {
        public Course(CourseCategory source)
        {
            this.ID = source.ID;
            this.ImageID = source.Image.WithStruct(x => x.FileID, Guid.Empty);
            this.Title = source.Name;
            this.Description = source.Description;
        }

        public int ID { get; set; }
        public Guid ImageID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Lesson> Lessons { get; set; }
        public int Pct { get; set; }
    }
}
