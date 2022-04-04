using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KUSYS_DEMO.Models.Entities
{
    public class Student
    {
        [Key]
        public string StudentId { get; set; }
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        public virtual List<Course> Courses { get; set; }
        public virtual List<StudentCourse> StudentCourses { get; set; }
    }
}
