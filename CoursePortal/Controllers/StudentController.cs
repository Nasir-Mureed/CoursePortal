using CoursePortal.Helpers;
using CoursePortal.Models;
using CoursePortal.Models.Interfaces;
using CoursePortal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoursePortal.Controllers;

[Authorize(Roles = RoleHomeRedirect.Student)]
public class StudentController : Controller
{
    private readonly ICourseRepository _courses;
    private readonly IStudentRepository _students;
    private readonly IEnrollmentRepository _enrollments;
    private readonly UserManager<IdentityUser> _userManager;

    public StudentController(
        ICourseRepository courses,
        IStudentRepository students,
        IEnrollmentRepository enrollments,
        UserManager<IdentityUser> userManager)
    {
        _courses = courses;
        _students = students;
        _enrollments = enrollments;
        _userManager = userManager;
    }

    // Student dashboard: available courses + my enrollments
    public async Task<IActionResult> Home()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var student = await _students.GetStudentByUserId(user.Id);
        var enrollments = student == null
            ? Enumerable.Empty<EnrollmentDetails>()
            : await _enrollments.GetByStudentId(student.StudentId);

        var enrolledIds = enrollments.Select(e => e.CourseId).ToHashSet();
        var available = (await _courses.GetActiveCourses())
            .Where(c => !enrolledIds.Contains(c.CourseId));

        return View(new StudentHomeViewModel
        {
            Student = student,
            AvailableCourses = available,
            MyEnrollments = enrollments
        });
    }

    // Enroll current student in a course
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var student = await _students.GetStudentByUserId(user.Id);
        if (student == null)
        {
            TempData["Error"] = "Student profile not found.";
            return RedirectToAction(nameof(Home));
        }

        if (await _enrollments.IsEnrolled(student.StudentId, courseId))
        {
            TempData["Error"] = "Already enrolled in this course.";
            return RedirectToAction(nameof(Home));
        }

        await _enrollments.AddEnrollment(new Enrollment
        {
            StudentId = student.StudentId,
            CourseId = courseId,
            EnrollmentDate = DateTime.UtcNow,
            Status = "Enrolled",
            Grade = string.Empty
        });

        TempData["Success"] = "Enrolled successfully.";
        return RedirectToAction(nameof(Home));
    }
}
