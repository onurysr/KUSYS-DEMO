using KUSYS_DEMO.Data;
using KUSYS_DEMO.Models.Entities;
using KUSYS_DEMO.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KUSYS_DEMO.Controllers
{
    public class CourseController : Controller
    {
        private readonly MyContext _dbContext;

        public CourseController(MyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Get()
        {
            var data = _dbContext.Courses.ToList();
            return View(data);
        }

        public IActionResult GetCoursesWithStudents()
        {
            var data = _dbContext.StudentCourses.Include(x => x.Student).Include(x => x.Course)
                .OrderBy(x => x.StudentId)
                .ToList();
            return View(data);
        }

        [HttpGet]
        public IActionResult AddCourse(string? id)
        {
            var student = _dbContext.Students.FirstOrDefault(x => x.StudentId == id);
            if (student == null)
            {
                return RedirectToAction(nameof(GetCoursesWithStudents));
            }

            var model = new CourseViewModel()
            {
                StudentId = student.StudentId
            };

            ViewBag.CourseList = GetCourseList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Addcourse(CourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var course = _dbContext.StudentCourses.Where(x => x.StudentId == model.StudentId).ToList();

            foreach (var item in course)
            {
                if (item.CourseId == model.CourseId)
                {     
                    ViewBag.CourseList = GetCourseList();
                    TempData["Message"] = "Bu Ders Daha önce Eklenmiştir";
                    return View(model);
                }
            }
            var Student = new StudentCourse()
            {
                CourseId = model.CourseId,
                StudentId = model.StudentId
            };

            try
            {
                _dbContext.StudentCourses.Add(Student);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(GetCoursesWithStudents));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                //ViewBag.CourseList = GetCourseList();
                return View(model);
            }



        }

        private List<SelectListItem> GetCourseList()
        {
            var courses = _dbContext.Courses.OrderBy(x => x.CourseName).ToList();

            var courseList = new List<SelectListItem>()
            {
                new SelectListItem("Kategori yok",null)
            };
            foreach (var item in courses)
            {
                courseList.Add(new SelectListItem(item.CourseName, item.CourseId));
            }


            return courseList;
        }
    }
}
