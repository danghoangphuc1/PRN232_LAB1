using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.DTOs
{
    public class CreateCourseRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "CourseName is required and cannot be empty.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "CourseName must be between 3 and 100 characters.")]
        public string CourseName { get; set; } = null!;

        [Required(ErrorMessage = "SemesterId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SemesterId must be greater than 0.")]
        public int SemesterId { get; set; }
    }
}
