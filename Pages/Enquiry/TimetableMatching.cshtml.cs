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

        public Student Student { get; set; } = new();
        public List<Course> AvailableCourses { get; set; } = new();
        public List<StudentUnavailability> StudentUnavailability { get; set; } = new();
        public List<TimetableViewModel> AllCourses { get; set; } = new();
        public List<TimetableViewModel> MatchingSchedule { get; set; } = new();

        public IActionResult OnGet()
        {
            var user = HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            var studentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

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

            return Page();
        }

        private int GetNextStudentUnavailabilityId()
        {
            var maxId = _context.StudentUnavailability.Max(su => (int?)su.StudentUnavailabilityId) ?? 0;
            return maxId + 1;
        }

        public IActionResult OnPostAddUnavailability(string Day, TimeSpan StartTime, TimeSpan EndTime)
        {
            var studentId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToPage("/Login");
            }

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

            var unavailability = _context.StudentUnavailability
                .Where(su => su.StudentId == studentId)
                .ToList();

            var allSchedules = _context.Courses
                .Select(c => new TimetableViewModel
                {
                    Day = c.Day,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                }).ToList();

            MatchingSchedule = allSchedules
                .Where(schedule => !unavailability.Any(u =>
                    u.Day == schedule.Day &&
                    (schedule.StartTime < u.EndTime && schedule.EndTime > u.StartTime)))
                .ToList();

            return Page();
        }
    }
}
