using CoursePortal.Helpers;
using CoursePortal.Models;
using CoursePortal.Models.Interfaces;
using CoursePortal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePortal.Controllers;

[Authorize(Roles = RoleHomeRedirect.Mentor)]
public class MentorController : Controller
{
    private readonly ICourseRepository _courses;
    private readonly IStudentRepository _students;
    private readonly IEnrollmentRepository _enrollments;

    public MentorController(
        ICourseRepository courses,
        IStudentRepository students,
        IEnrollmentRepository enrollments)
    {
        _courses = courses;
        _students = students;
        _enrollments = enrollments;
    }

    // Mentor dashboard: courses, students, enrollments
    public async Task<IActionResult> Home()
    {
        return View(new MentorHomeViewModel
        {
            Courses = await _courses.GetAllCourses(),
            Students = await _students.GetAllStudents(),
            Enrollments = await _enrollments.GetAllEnrollments()
        });
    }

    [HttpGet]
    public IActionResult CreateCourse()
    {
        return View(new Course { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourse(Course course)
    {
        if (!ModelState.IsValid)
            return View(course);

        course.CreatedDate = DateTime.Now;
        await _courses.AddCourse(course);

        TempData["Success"] = "Course created successfully!";
        return RedirectToAction(nameof(Home));
    }
}
