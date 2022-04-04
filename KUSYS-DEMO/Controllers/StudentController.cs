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
        private readonly MyContext _dbContext; //MyContext classını inject ettim.
        public StudentController(MyContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetStudents()//öğrenci listesini getiren action
        {

            var model = await _dbContext.Students.Include(x => x.Courses).OrderBy(x => x.StudentId).ToListAsync(); //veritabaından Students tablosunu çektim ve StudentId'ye göre sıraladım ve model değişkenine atıp View'a gönderdim


            return View(model);
        }

        [HttpGet]
        public IActionResult Create()//öğrenci oluşturan action
        {
            return View();//sadece view'ı var çünkü sadece veri girmek için kullandım.
        }

        [HttpPost]
        public IActionResult Create(StudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //model başarılı değilse aynı modeli geri döndürsün
                return View(model);
            }

            var student = new Student() //Student nesnesi oluşturuğ modelden gelen verileri student propertylerine atadım.
            {
                StudentId = model.StudentId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                BirthDate = model.BirthDate,
            };

            try
            {
                _dbContext.Students.Add(student);//veri tabanına ekleme işlemi
                _dbContext.SaveChanges();//kaydetme işlem,
                return RedirectToAction(nameof(GetStudents));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);//hatalı ise hata mesajı döndürür.
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)//sil butonuna bastıktan sonra id paramatresi gönderdim
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.StudentId == id);//gelen id parametresi ile veritabında studentId ile eşleşirme işlemi
            if (student == null)
            {
                return RedirectToAction(nameof(GetStudents));
            }

            try
            {
                _dbContext.Students.Remove(student);//silme işlemi
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
        public async Task<IActionResult> Update(string? id)//update de id parametresi alıyor çünkü belli bir kişinin verisini güncelleyeceğiz.
        {
            var data = await _dbContext.Students.FirstOrDefaultAsync(x => x.StudentId == id); //veritabanındaki studentId ile eşleşen kaydı getirir.
            if (data == null)
            {
                return RedirectToAction(nameof(GetStudents));
            }

            var model = new StudentViewModel() //oluşturduğum view model'e inputları otomatik olarak dolu getirmek için model nesnesi oluşturdum ve view'a yolladım.
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

            var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.StudentId == model.StudentId);//modelden gelen studentId veri tabanında eşleşen kaydı getirir.
            if (student == null)
            {
                return RedirectToAction(nameof(GetStudents));
            }

            student.StudentId = model.StudentId; //veritabanına modelden gelen verileri atama işlemi
            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.BirthDate = model.BirthDate;
            try
            {

                _dbContext.Students.Update(student);//güncelleme işlemi
                _dbContext.SaveChanges();
                TempData["Message"] = $"{student.StudentId} numaralı öğrenci başarılı bir şekilde güncellendi"; //tempdata ile view'a güncelleme işlemi başarılı mesajı gönderdim.
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
            var data = _dbContext.Students
                .Include(x => x.Courses)
                .Include(x => x.StudentCourses)
                .FirstOrDefault(x => x.StudentId == id); //Detay sayfasıda aynı şekilde gelen id paramteresi veritabında eşleşen kaydı getirir.
            if (data == null)
            {
                return RedirectToAction(nameof(GetStudents));
            }

            var model = new StudentDetailViewModel() //Detayları göstermek için StudentViewModel nesnes, oluşturup veri eşleşen kaydın proplarını atadım
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
