using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Model
{
    public class Lesson
    {
        public Lesson(DateTime date, int lessonNumber, string schoolClass, string teacherId)
        {
            Date = date;
            LessonNumber = lessonNumber;
            SchoolClass = schoolClass;
            TeacherId = teacherId;
        }

#pragma warning disable CS8618
        protected Lesson() { }
#pragma warning restore CS8618 

        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public DateTime Date { get; set; }
        public int LessonNumber { get; set; }
        public string SchoolClass { get; set; }
        public string TeacherId { get; set; }
    }
}
