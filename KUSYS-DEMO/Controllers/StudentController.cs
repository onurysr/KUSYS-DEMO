using KUSYS_DEMO.Data;
using KUSYS_DEMO.Models.Entities;
using KUSYS_DEMO.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KUSYS_DEMO.Controllers
{
    public class StudentController : Controller
    {
        private readonly MyContext _dbContext; 
        public StudentController(MyContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {

            var model = await _dbContext.Students.Include(x => x.Courses).OrderBy(x => x.StudentId).ToListAsync();


            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            //ViewBag.CourseList = GetCourseList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.CourseList = GetCourseList();
                return View(model);
            }

            var student = new Student()
            {
                StudentId = model.StudentId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                BirthDate = model.BirthDate,
            };

            try
            {
                _dbContext.Students.Add(student);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(GetStudents));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                //ViewBag.CourseList = GetCourseList();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.StudentId == id);
            if (student == null)
            {
                return RedirectToAction(nameof(GetStudents));
            }

            try
            {
                _dbContext.Students.Remove(student);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(GetStudents));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(GetStudents));
                TempData["Message"] = "Silme işlemi Başarısız";
            }
        }
        [HttpGet]
        public async Task<IActionResult> Update(string? id)
        {
            var data = await _dbContext.Students.FirstOrDefaultAsync(x => x.StudentId == id);
            if (data == null)
            {
                return RedirectToAction(nameof(GetStudents));
            }

            var model = new StudentViewModel()
            {
                StudentId = data.StudentId,
                BirthDate = data.BirthDate,
                FirstName = data.FirstName,
                LastName = data.LastName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(StudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.StudentId == model.StudentId);
            if (student == null)
            {
                return RedirectToAction(nameof(GetStudents));
            }

            student.StudentId = model.StudentId;
            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.BirthDate = model.BirthDate;
            try
            {

                _dbContext.Students.Update(student);
                _dbContext.SaveChanges();
                TempData["Message"] = $"{student.StudentId} numaralı öğrenci başarılı bir şekilde güncellendi";
                return RedirectToAction(nameof(GetStudents));
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Detail(string? id)
        {
            var data = _dbContext.Students.Include(x => x.Courses).Include(x => x.StudentCourses).FirstOrDefault(x => x.StudentId == id);
            if (data == null)
            {
                return RedirectToAction(nameof(GetStudents));
            }

            var model = new StudentDetailViewModel()
            {
                StudentId = data.StudentId,
                BirthDate = data.BirthDate,
                FirstName = data.FirstName,
                LastName = data.LastName,
                NumberOfCourses = data.StudentCourses.Count

            };

            return View(model);
        }
    }
}
