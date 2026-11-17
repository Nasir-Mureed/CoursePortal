using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoursePortal.Helpers;

public static class RoleHomeRedirect
{
    public const string Student = "Student";
    public const string Mentor = "Mentor";

    // Send each role to its own home after login/register
    public static string GetHomeUrl(IUrlHelper url, IList<string> roles)
    {
        if (roles.Contains(Mentor))
            return url.Action("Home", "Mentor")!;

        if (roles.Contains(Student))
            return url.Action("Home", "Student")!;

        return url.Action("Index", "Home")!;
    }

    public static async Task<string> GetHomeUrlAsync(
        IUrlHelper url,
        UserManager<IdentityUser> userManager,
        IdentityUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        return GetHomeUrl(url, roles);
    }
}
