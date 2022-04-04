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
            var data = _dbContext.Courses.ToList(); //derleri listeler
            return View(data);
        }

        public IActionResult GetCoursesWithStudents() //Tüm öğrenciler ve aldığı dersleri gösteren action
        {
            var data = _dbContext.StudentCourses.Include(x => x.Student).Include(x => x.Course)
                .OrderBy(x => x.StudentId)
                .ToList();
            return View(data);
        }

        [HttpGet]
        public IActionResult AddCourse(string? id) //Öğrenciye Ders ekleme işlemi yapan action
        {
            var student = _dbContext.Students.FirstOrDefault(x => x.StudentId == id); //gelen id ile veritabında studentId eşleme işlemi
            if (student == null)
            {
                return RedirectToAction(nameof(GetCoursesWithStudents));
            }

            var model = new CourseViewModel() //öğrenci numarası göstermek için kullanılan viewmodel
            {
                StudentId = student.StudentId
            };

            ViewBag.CourseList = GetCourseList();//viewBagle dersleri view'a göndermek için kullandım
            return View(model);
        }

        [HttpPost]
        public IActionResult Addcourse(CourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var course = _dbContext.StudentCourses
                .Where(x => x.StudentId == model.StudentId)
                .ToList();//modelden gelen studentId ile Veritabaındaki studentId eşleşen Öğreni-Dersleri Liste olarak atamasını yaptım

            foreach (var item in course) //tüm dersleri foreachle dönüp öğrencinin daha önce aldığı bir ders mi diye kontrol ettim 
            {
                if (item.CourseId == model.CourseId)
                {     
                    ViewBag.CourseList = GetCourseList();
                    TempData["Message"] = "Bu Ders Daha önce Eklenmiştir";
                    return View(model);
                }
            }
            var Student = new StudentCourse() //CourseId ve Student Id'yi yeni StudentCourse Nesnesine atadım
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

        private List<SelectListItem> GetCourseList() //birden çok veriyi getirmek için SelectListItem kullandı
        {
            var courses = _dbContext.Courses.OrderBy(x => x.CourseName).ToList(); //derlerin listesini döndürüp courses değişkenine atadım

            var courseList = new List<SelectListItem>()
            {
                new SelectListItem("Kategori yok",null)
            };
            foreach (var item in courses)
            {
                courseList.Add(new SelectListItem(item.CourseName, item.CourseId)); //foreacle tüm courses için CourseName ve idlerini attım
            }


            return courseList;
        }
    }
}
