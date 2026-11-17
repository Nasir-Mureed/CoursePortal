#nullable disable

using System.ComponentModel.DataAnnotations;
using CoursePortal.Helpers;
using CoursePortal.Models;
using CoursePortal.Models.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoursePortal.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IStudentRepository _students;

    public RegisterModel(
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        SignInManager<IdentityUser> signInManager,
        ILogger<RegisterModel> logger,
        IStudentRepository students)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _students = students;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // Student or Mentor
        [Required]
        [Display(Name = "Register as")]
        public string Role { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        // Only allow the two app roles
        if (Input.Role is not (RoleHomeRedirect.Student or RoleHomeRedirect.Mentor))
            ModelState.AddModelError("Input.Role", "Please select Student or Mentor.");

        if (!ModelState.IsValid)
            return Page();

        var user = CreateUser();
        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");

            await _userManager.AddToRoleAsync(user, Input.Role);

            // Students also get a profile row for enrollments
            if (Input.Role == RoleHomeRedirect.Student)
            {
                var name = Input.Email.Split('@')[0];
                await _students.AddStudent(new Student
                {
                    UserId = user.Id,
                    Email = Input.Email,
                    FirstName = name,
                    LastName = string.Empty,
                    DateOfBirth = DateTime.UtcNow.Date,
                    Gender = string.Empty,
                    PhoneNumber = string.Empty,
                    Address = string.Empty,
                    ProfilePicture = string.Empty,
                    RegistrationDate = DateTime.UtcNow
                });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            var homeUrl = await RoleHomeRedirect.GetHomeUrlAsync(Url, _userManager, user);
            return LocalRedirect(homeUrl);
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }

    private IdentityUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<IdentityUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'.");
        }
    }

    private IUserEmailStore<IdentityUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
            throw new NotSupportedException("The default UI requires a user store with email support.");

        return (IUserEmailStore<IdentityUser>)_userStore;
    }
}
