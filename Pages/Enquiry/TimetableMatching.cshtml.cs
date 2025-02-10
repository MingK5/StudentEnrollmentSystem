using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Pages.Enquiry
{
    public class TimetableMatchingModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TimetableMatchingModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Student Student { get; set; } = new(); // Added Student property

        public List<Course> AvailableCourses { get; set; } = new();
        public List<StudentUnavailability> StudentUnavailability { get; set; } = new();
        public List<TimetableViewModel> EnrolledCourses { get; set; } = new();
        public List<TimetableViewModel> MatchingSchedule { get; set; } = new();

        public IActionResult OnGet()
        {
            var user = HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            Student = new Student
            {
                StudentName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",
                StudentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown",
                Program = user.FindFirst("Program")?.Value ?? "Unknown"
            };

            // Fetch enrolled courses
            EnrolledCourses = _context.Enrolments
                .Where(e => e.StudentId == Student.StudentId)
                .Join(
                    _context.Courses,
                    enrol => enrol.CourseId,
                    course => course.CourseId,
                    (enrol, course) => new TimetableViewModel
                    {
                        CourseId = course.CourseId,
                        CourseName = course.CourseName,
                        Lecturer = course.Lecturer,
                        StartTime = course.StartTime ?? TimeSpan.Zero, // Handle nullable TimeSpan
                        EndTime = course.EndTime ?? TimeSpan.Zero, // Handle nullable TimeSpan
                        Day = course.Day
                    })
                .ToList();

            return Page();
        }
    }

    public class Student
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
    }

    public class StudentUnavailability
    {
        public int Id { get; set; } // Added Id property
        public string Day { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
