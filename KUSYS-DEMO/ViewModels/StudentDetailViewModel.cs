using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KUSYS_DEMO.ViewModels
{
    public class StudentDetailViewModel
    {
        public string StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int NumberOfCourses { get; set; }
    }
}
