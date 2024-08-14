using APDP_ASM2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace APDP_ASM2.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        static List<Teacher> teachers = new List<Teacher>();

        // Delete method
        public IActionResult Delete(int id)
        {
            var teachers = LoadTeachersFromFile("teacher.json");

            var searchTeacher = teachers.FirstOrDefault(t => t.Id == id);
            if (searchTeacher == null)
            {
                return NotFound(); // Return a 404 error if the teacher is not found
            }

            teachers.Remove(searchTeacher);

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(teachers, options);
            System.IO.File.WriteAllText("teacher.json", jsonString);

            return RedirectToAction("ManageTeacher");
        }

        [HttpPost]
        public IActionResult Save(int id, Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                var teachers = LoadTeachersFromFile("teacher.json");
                var existingTeacher = teachers.FirstOrDefault(t => t.Id == id);
                if (existingTeacher == null)
                {
                    return NotFound(); // Return a 404 error if the teacher is not found
                }

                existingTeacher.Name = teacher.Name;
                existingTeacher.DoB = teacher.DoB;
                existingTeacher.Course = teacher.Course;

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(teachers, options);
                System.IO.File.WriteAllText("teacher.json", jsonString);

                return RedirectToAction("ManageTeacher");
            }
            else
            {
                List<Course> courses = LoadCourseFromFile("course.json");
                ViewBag.SelectCourse = courses;
                return View("EditTeacher", teacher);
            }
        }

        [HttpPost]
        public IActionResult NewTeacher(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                var teachers = LoadTeachersFromFile("teacher.json");
                teachers.Add(teacher);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(teachers, options);
                System.IO.File.WriteAllText("teacher.json", jsonString);

                return RedirectToAction("ManageTeacher");
            }
            else
            {
                List<Course> courses = LoadCourseFromFile("course.json");
                ViewBag.SelectCourse = courses;
                return View();
            }
        }

        [HttpGet]
        public IActionResult NewTeacher()
        {
            List<Course> courses = LoadCourseFromFile("course.json");
            ViewBag.SelectCourse = courses;
            return View();
        }

        [HttpGet]
        public IActionResult EditTeacher(int id)
        {
            var teacher = LoadTeachersFromFile("teacher.json").FirstOrDefault(s => s.Id == id);
            if (teacher == null)
            {
                return NotFound(); // Return a 404 error if the teacher is not found
            }

            List<Course> courses = LoadCourseFromFile("course.json");
            ViewBag.SelectCourse = courses;
            return View(teacher);
        }

        [HttpPost]
        public IActionResult Edit(int id, Teacher teacher, List<Course> courses)
        {
            var teachers = LoadTeachersFromFile("teacher.json");
            var existingTeacher = teachers.FirstOrDefault(t => t.Id == id);
            if (existingTeacher == null)
            {
                return NotFound(); // Return a 404 error if the teacher is not found
            }

            existingTeacher.Name = teacher.Name;
            existingTeacher.DoB = teacher.DoB;
            existingTeacher.Course = teacher.Course;

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(teachers, options);
            System.IO.File.WriteAllText("teacher.json", jsonString);

            return RedirectToAction("ManageTeacher");
        }

        public List<Teacher> LoadTeachersFromFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                string readText = System.IO.File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<List<Teacher>>(readText) ?? new List<Teacher>();
            }
            return new List<Teacher>();
        }

        public List<Course> LoadCourseFromFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                string readText = System.IO.File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<List<Course>>(readText) ?? new List<Course>();
            }
            return new List<Course>();
        }

        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            teachers = LoadTeachersFromFile("teacher.json");
            return View(teachers);
        }

        public IActionResult ManageTeacher()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            teachers = LoadTeachersFromFile("teacher.json");
            return View(teachers);
        }

        public IActionResult ViewTeacher()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            teachers = LoadTeachersFromFile("teacher.json");
            return View(teachers);
        }

        public IActionResult ViewClass()
        {
            // Logic to display class information
            return View();
        }

        [HttpPost]
        public IActionResult Cancel(string returnUrl)
        {
            return RedirectToAction("ManageTeacher");
        }

        public IActionResult ViewCourse()
        {
            // Logic to display course information
            return View();
        }
    }
}
