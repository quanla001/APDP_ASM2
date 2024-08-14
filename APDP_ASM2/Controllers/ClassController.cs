using APDP_ASM2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using APDP_ASM2.Models;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace APDP_ASM2.Controllers
{
    [Authorize]
    public class ClassController : Controller
    {
        static List<Class> classes = new List<Class>();
        private List<Teacher> teachers = new List<Teacher>();
        public List<Class>? LoadClassFromFile(string fileName)
        {
            string readText = System.IO.File.ReadAllText("class.json");
            return JsonSerializer.Deserialize<List<Class>>(readText);
        }
        public IActionResult ViewClass()
        {

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            // Read a file
            classes = LoadClassFromFile("class.json");
            return View(classes);
            // Trả về view Managestudent.cshtml
            // return View("Managestudent");//
        }
        public IActionResult ManageClass()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            // Read a file
            classes = LoadClassFromFile("class.json");
            return View(classes);
            // Trả về view Managestudent.cshtml
            // return View("Managestudent");//
        }
        public IActionResult Delete(int Id)
        {
            var classes = LoadClassFromFile("class.json");

            //find teacher in an array
            var searchClass = classes.FirstOrDefault(t => t.Id == Id);
            classes.Remove(searchClass);

            //save to file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(classes, options);
            //Save file
            using (StreamWriter writer = new StreamWriter("class.json"))
            {
                writer.Write(jsonString);
            }
            return RedirectToAction("ManageClass");
        }
        [HttpPost] // Submit new Teacher
        public IActionResult NewClass(Class @class)
        {
            if (ModelState.IsValid)
            {
                // Add the new class to the list
                classes.Add(@class);
                // Serialize the classes list to JSON
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(classes, options);
                // Save the JSON data to a file
                System.IO.File.WriteAllText("class.json", jsonString);
                // Redirect to the action to manage classes
                return RedirectToAction("ManageClass", new { classes = jsonString });
            }
            else
            {
                // If model state is not valid, return to the view with validation errors
                List<Teacher> teachers = LoadTeacherFromFile("teacher.json");
                ViewBag.SelectTeacher = teachers;
                return View();
            }
        }

        [HttpGet] // Click hyperlink
        public IActionResult NewClass()
        {
            List<Teacher> teachers = LoadTeacherFromFile("teacher.json");
            ViewBag.SelectTeacher = teachers;
            return View();
        }


        [HttpGet] // Click hyperlink
        public IActionResult EditClass(int id)
        {

            var @class = classes.FirstOrDefault(s => s.Id == id);
            if (@class == null)
            {
                return NotFound(); // Return a 404 error if the class is not found
            }

            List<Teacher> teachers = LoadTeacherFromFile("teacher.json");

            // Pass the list of lecturers to the view using ViewBag
            ViewBag.SelectTeacher = teachers;

            return View(@class);
        }
        public List<Teacher>? LoadTeacherFromFile(string fileName)
        {
            string readText = System.IO.File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<List<Teacher>>(readText);
        }
        [HttpPost]
        public IActionResult Edit(Class @class, List<Teacher> teachers)
        {
            var existingClass = classes.FirstOrDefault(t => t.Id == @class.Id);
            if (existingClass == null)
            {
                return NotFound(); // Return a 404 error if the class is not found
            }

            if (ModelState.IsValid)
            {
                existingClass.ClassName = @class.ClassName;
                existingClass.Major = @class.Major;
                existingClass.Lecturer = @class.Lecturer;

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(classes, options);
                ViewBag.SelectTeacher = teachers;

                // Save the updated class information to the file
                System.IO.File.WriteAllText("class.json", jsonString);

                // Redirect to the page for managing classes
                return RedirectToAction("ManageClass");
            }
            else
            {
                // If model state is not valid, return to the edit page with validation errors
                // Also pass the list of lecturers to the view
                ViewBag.SelectTeachers = teachers;
                return View("EditClass", @class);
            }
        }

        [HttpPost]
        public IActionResult Save(Class @class)
        {
            var existingClass = classes.FirstOrDefault(t => t.Id == @class.Id);
            if (existingClass == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy giáo viên
            }

            // Cập nhật thông tin giáo viên
            existingClass.ClassName = @class.ClassName;
            existingClass.Major = @class.Major;
            existingClass.Lecturer = @class.Lecturer;

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(classes, options);

            // Lưu thông tin mới vào file
            System.IO.File.WriteAllText("class.json", jsonString);

            // Chuyển hướng về trang quản lý giáo viên
            return RedirectToAction("ManageClass");
        }
        [HttpGet] //click hyperlink
        public IActionResult Save()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Cancel(string returnUrl)
        {
            // Redirect to the ManageCourse action method
            return RedirectToAction("ManageClass");
        }

    }
}
