namespace StudentEnrollmentSystem.Models
{
    public class EnrolledCourseViewModel
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Lecturer { get; set; } = string.Empty;
        public int Credit { get; set; } 
        public TimeSpan? StartTime { get; set; } 
        public TimeSpan? EndTime { get; set; } 
        public string Day { get; set; } = string.Empty; 
        public string Action { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime? DatePerformed { get; set; }
    }
}
