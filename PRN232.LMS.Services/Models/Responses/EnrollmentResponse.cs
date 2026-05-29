using System;

namespace PRN232.LMS.Services.DTOs
{
    public class EnrollmentResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;

        // Optional expansions
        public StudentResponse? Student { get; set; }
        public CourseResponse? Course { get; set; }
    }
}
