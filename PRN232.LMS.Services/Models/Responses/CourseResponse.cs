using System;

namespace PRN232.LMS.Services.DTOs
{
    public class CourseResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }

        // Optional expansion
        public SemesterResponse? Semester { get; set; }
    }
}
