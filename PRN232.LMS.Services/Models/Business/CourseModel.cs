using System;

namespace PRN232.LMS.Services.Models.Business
{
    public class CourseModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
    }
}
