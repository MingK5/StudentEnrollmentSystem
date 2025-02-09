namespace StudentEnrollmentSystem.Models
{
    public class CourseSelectionViewModel
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public bool IsSelected { get; set; } // Checkbox state
    }
}
