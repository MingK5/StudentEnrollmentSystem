namespace StudentEnrollmentSystem.Models
{
    public class TimetableViewModel
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Lecturer { get; set; } = string.Empty;
        public string Day { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
