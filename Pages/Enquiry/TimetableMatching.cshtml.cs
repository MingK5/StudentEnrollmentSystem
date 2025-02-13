using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using Microsoft.Extensions.Configuration;

namespace StudentEnrollmentSystem.Pages.Enquiry
{
    public class TimetableMatchingModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public TimetableMatchingModel(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public Student Student { get; set; } = new();
        public List<Course> AvailableCourses { get; set; } = new();
        public List<StudentUnavailability> StudentUnavailability { get; set; } = new();
        public List<TimetableViewModel> AllCourses { get; set; } = new();
        public List<TimetableViewModel> MatchingSchedule { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            var studentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var Session = _configuration["Session"] ?? string.Empty;

            Student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId) ?? new Student();
            AvailableCourses = await _context.Courses.ToListAsync();
            StudentUnavailability = await _context.StudentUnavailability
                .Where(su => su.StudentId == studentId)
                .ToListAsync();

            var lastCourses = await _context.Enrolments
                .Where(e => e.StudentId == studentId && e.Session == Session)
                .Include(e => e.Course) 
                .GroupBy(e => e.CourseId)
                .Select(g => g.OrderByDescending(e => e.DatePerformed).First())
                .ToListAsync();

            AllCourses = lastCourses
                .Where(e => e.StudentId == studentId && (e.Action == "Enrolled" || e.Action == "Add"))
                .Select(e => new TimetableViewModel
                {
                    Day = e.Course.Day,
                    StartTime = e.Course.StartTime,
                    EndTime = e.Course.EndTime,
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    Credit = e.Course.Credit
                }).ToList();

            return Page();
        }

        private int GetNextStudentUnavailabilityId()
        {
            var maxId = _context.StudentUnavailability.Max(su => (int?)su.StudentUnavailabilityId) ?? 0;
            return maxId + 1;
        }

        private void LoadStudentData(string studentId)
        {
            Student = _context.Students.FirstOrDefault(s => s.StudentId == studentId) ?? new Student();
            AvailableCourses = _context.Courses.ToList();
            StudentUnavailability = _context.StudentUnavailability
                .Where(su => su.StudentId == studentId)
                .ToList();

            AllCourses = _context.Enrolments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .Select(e => new TimetableViewModel
                {
                    Day = e.Course.Day,
                    StartTime = e.Course.StartTime,
                    EndTime = e.Course.EndTime,
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    Credit = e.Course.Credit
                }).ToList();
        }



        public IActionResult OnPostAddUnavailability(string Day, TimeSpan StartTime, TimeSpan EndTime)
        {
            var studentId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToPage("/Login");
            }

            // Reload student data and courses to avoid disappearing data
            LoadStudentData(studentId);

            // Normalize input for case-insensitive comparison
            string normalizedDay = Day.ToLower();

            // Check if there is an existing unavailability with the same day and overlapping time
            bool isDuplicate = _context.StudentUnavailability
                .Any(su =>
                    su.StudentId == studentId &&
                    su.Day.ToLower() == normalizedDay &&
                    !(EndTime <= su.StartTime || StartTime >= su.EndTime)
                );

            if (isDuplicate)
            {
                ModelState.AddModelError(string.Empty, "This unavailability period overlaps with an existing one.");
                return Page();
            }

            // If no duplicate, add the new unavailability entry
            var newUnavailability = new StudentUnavailability
            {
                StudentUnavailabilityId = GetNextStudentUnavailabilityId(),
                StudentId = studentId,
                Day = Day,
                StartTime = StartTime,
                EndTime = EndTime
            };

            _context.StudentUnavailability.Add(newUnavailability);
            _context.SaveChanges();

            return RedirectToPage();
        }



        public IActionResult OnPostRemoveUnavailability(int id)
        {
            var unavailability = _context.StudentUnavailability.Find(id);
            if (unavailability != null)
            {
                _context.StudentUnavailability.Remove(unavailability);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostStartMatching()
        {
            var studentId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToPage("/Login");
            }

            // Get student details again
            Student = _context.Students.FirstOrDefault(s => s.StudentId == studentId) ?? new Student();

            // Get student unavailability again
            StudentUnavailability = _context.StudentUnavailability
                .Where(su => su.StudentId == studentId)
                .ToList();

            // Get all courses the student is enrolled in
            AllCourses = _context.Enrolments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .Select(e => new TimetableViewModel
                {
                    Day = e.Course.Day,
                    StartTime = e.Course.StartTime,
                    EndTime = e.Course.EndTime,
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    Credit = e.Course.Credit
                }).ToList();

            // Filter out courses that overlap with unavailability times
            MatchingSchedule = AllCourses
                .Where(course => !StudentUnavailability.Any(unavailable =>
                    unavailable.Day.Equals(course.Day, StringComparison.OrdinalIgnoreCase) && // Match day (case insensitive)
                    !(course.EndTime <= unavailable.StartTime || course.StartTime >= unavailable.EndTime) // Overlapping condition
                ))
                .ToList();

            return Page();
        }



    }
}


