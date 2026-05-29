namespace PRN232.LMS.Services.BusinessModels
{
    public class CourseModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
    }
}
