using System.ComponentModel.DataAnnotations;

namespace APDP_ASM2.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Class is required")]
        public string Class { get; set; }

        [Required(ErrorMessage = "Major is required")]
        public string Major { get; set; }

        [Required(ErrorMessage = "Lecturer is required")]
        public string Lecturer { get; set; }

        public string Status { get; set; }

        public Course()
        {

        }
    }
}