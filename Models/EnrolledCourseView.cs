namespace StudentEnrollmentSystem.Models
{
    public class EnrolledCourseViewModel
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Lecturer { get; set; } = string.Empty;
        public int Credit { get; set; } // Use "Credit" to match Course model
        public TimeSpan? StartTime { get; set; } // Add start time
        public TimeSpan? EndTime { get; set; } // Add end time
        public string Day { get; set; } = string.Empty; // Add Day property
    }
}
