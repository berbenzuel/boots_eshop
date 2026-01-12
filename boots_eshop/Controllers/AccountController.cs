using System.Diagnostics;
using BootEshop.ViewModels;
using Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BootEshop.Features.Login;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AccountController(
        SignInManager<User> signInManager,
        UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Try to find the user by email
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // PasswordSignInAsync requires the username internally
        var result = await _signInManager.PasswordSignInAsync(
            user.UserName,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Successful login → redirect
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            return View("Lockout");
        }

        if (result.RequiresTwoFactor)
        {
            // Optional: redirect to 2FA page if implemented
            return RedirectToAction("LoginWith2fa");
        }

        // If we reach here → failed login
        ModelState.AddModelError("", "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
