using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Maybe;

namespace Base.Content.Entities
{
    public class Course
    {
        public Course(CourseCategory source)
        {
            this.ID = source.ID;
            this.ImageID = source.Image.WithStruct(x => x.FileID, Guid.Empty);
            this.ImageKey = source.Image.WithStruct(x => x.Key, Guid.Empty);
            this.Title = source.Name;
            this.Description = source.Description;
        }

        public int ID { get; set; }
        public Guid ImageID { get; set; }
        public Guid ImageKey { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Lesson> Lessons { get; set; }
        public int Pct { get; set; }
    }
}
