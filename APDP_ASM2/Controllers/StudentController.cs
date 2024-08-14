﻿using APDP_ASM2.Interface;
using APDP_ASM2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace APDP_ASM2.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IEmailValidator _emailValidator;

        public StudentController(IStudentService studentService, IEmailValidator emailValidator)
        {
            _studentService = studentService;
            _emailValidator = emailValidator;
        }

        public IActionResult Delete(int id)
        {
            _studentService.DeleteStudent(id);
            return RedirectToAction("ManageStudent");
        }

        public IActionResult ManageStudent()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            var students = _studentService.GetAllStudents();
            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            if (student == null)
            {
                return BadRequest("Student object cannot be null.");
            }

            if (ModelState.IsValid && _emailValidator.IsValid(student.Email))
            {
                _studentService.UpdateStudent(student);
                return RedirectToAction("ManageStudent");
            }

            return View(student);
        }

        [HttpGet]
        public IActionResult EditStudent(int id)
        {
            var student = _studentService.GetStudentById(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost]
        public IActionResult Save(Student student)
        {
            if (ModelState.IsValid && _emailValidator.IsValid(student.Email))
            {
                _studentService.UpdateStudent(student);
                return RedirectToAction("ManageStudent");
            }
            return View("EditStudent", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewStudent(Student student)
        {
            if (ModelState.IsValid && _emailValidator.IsValid(student.Email))
            {
                _studentService.AddStudent(student);
                return RedirectToAction("ManageStudent");
            }
            return View(student);
        }

        [HttpGet]
        public IActionResult NewStudent()
        {
            return View();
        }

        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            var students = _studentService.GetAllStudents();
            return View(students);
        }
    }
}
