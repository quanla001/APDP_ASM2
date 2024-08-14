using APDP_ASM2.Controllers;
using APDP_ASM2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SIMS_Test
{
    public class CourseControllerTests
    {
        [Fact]
        public void TestLoadCourseFromFile()
        {
            // Arrange
            var controller = new CourseController();
            var fileName = "course.json";
            var coursesJson = "[{\"Id\": 1,\"Name\": \"Design\",\"Class\": \"SE06205\",\"Major\": \"IT\",\"Lecturer\": \"test\",\"Status\": \"Active\",\"Teacher\": null},{\"Id\": 2,\"Name\": \"Computing\",\"Class\": \"SE06204\",\"Major\": \"IT\",\"Lecturer\": \"Nguyen Thanh Trieu\",\"Status\": \"Active\",\"Teacher\": null}]";
     
            System.IO.File.WriteAllText(fileName, coursesJson);

            // Act
            var result = controller.LoadCourseFromFile(fileName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            // Check first course
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Design", result[0].Name);
            Assert.Equal("SE06205", result[0].Class);
            Assert.Equal("IT", result[0].Major);
            Assert.Equal("test", result[0].Lecturer);
            Assert.Equal("Active", result[0].Status);
            Assert.Null(result[0].Teacher);

            // Check second course
            Assert.Equal(2, result[1].Id);
            Assert.Equal("Computing", result[1].Name);
            Assert.Equal("SE06204", result[1].Class);
            Assert.Equal("IT", result[1].Major);
            Assert.Equal("Nguyen Thanh Trieu", result[1].Lecturer);
            Assert.Equal("Active", result[1].Status);
            Assert.Null(result[1].Teacher);
        }
        [Fact]
        public void TestCreateCourse()
        {
            // Arrange
            var controller = new CourseController();
            var courseToAdd = new Course
            {
                Id = 1,
                Name = "New Course",
                Class = "SE1234",
                Major = "IT",
                Lecturer = "John Doe",
                Status = "Active"
            };
            var classes = new List<Class> { new Class { ClassName = "SE1234" } };

            // Act
            var result = controller.CreateCourse(courseToAdd, classes) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ManageCourse", result.ActionName);
        }
       
        [Fact]
        public void TestDeleteCourse()
        {
            // Arrange
            var controller = new CourseController();
            var idToDelete = 1;
            var fileName = "course.json";

            // Create initial course data
            var initialCourses = new List<Course>
            {
                new Course { Id = 1, Name = "Design", Class = "SE06205", Major = "IT", Lecturer = "test", Status = "Active" },
                new Course { Id = 2, Name = "Computing", Class = "SE06204", Major = "IT", Lecturer = "Nguyen Thanh Trieu", Status = "Active" }
            };
            var initialJson = JsonSerializer.Serialize(initialCourses);
            System.IO.File.WriteAllText(fileName, initialJson);

            // Act
            var result = controller.Delete(idToDelete) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ManageCourse", result.ActionName);

            // Verify the course was actually deleted
            var remainingCoursesJson = System.IO.File.ReadAllText(fileName);
            var remainingCourses = JsonSerializer.Deserialize<List<Course>>(remainingCoursesJson);

            Assert.Single(remainingCourses);
            Assert.DoesNotContain(remainingCourses, c => c.Id == idToDelete);
            Assert.Contains(remainingCourses, c => c.Id == 2);
        }
    }
}