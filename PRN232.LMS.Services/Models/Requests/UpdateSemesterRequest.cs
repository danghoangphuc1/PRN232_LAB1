using System;

namespace PRN232.LMS.Services.DTOs
{
    public class UpdateSemesterRequest
    {
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
