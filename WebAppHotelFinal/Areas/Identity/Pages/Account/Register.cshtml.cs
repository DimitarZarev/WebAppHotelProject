// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using WebAppHotelFinal.Data.Domain;
using WebAppHotelFinal.Models;
using WebAppHotelFinal.Data;

namespace WebAppHotelFinal.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
     UserManager<AppUser> userManager,
     IUserStore<AppUser> userStore,
     SignInManager<AppUser> signInManager,
     ILogger<RegisterModel> logger,
     IEmailSender emailSender,
     ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            [StringLength(10)]
            public string PhoneNumber { get; set; }

            public bool IsAdult { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
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

            _logger.LogInformation("Reached start of OnPostAsync");
            _logger.LogInformation("ModelState valid: {Valid}", ModelState.IsValid);

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning("ModelState error in {Key}: {ErrorMessage}", state.Key, error.ErrorMessage);
                    }
                }
                return Page();
            }

            _logger.LogInformation("Creating user {Email}", Input.Email);

            var user = CreateUser();
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Assigning role");

                bool isFirstUser = !_userManager.Users.Any(u => u.Id != user.Id);

                if (isFirstUser)
                    await _userManager.AddToRoleAsync(user, "Admin");
                else
                    await _userManager.AddToRoleAsync(user, "Employee");

                if (!isFirstUser) // Only create Client for non-Admin
                {
                    _logger.LogInformation("Creating client");

                    var client = new Client
                    {
                        FullName = $"{user.FirstName} {user.LastName}",
                        PhoneNumber = Input.PhoneNumber,
                        IsAdult = Input.IsAdult,
                        AppUserId = user.Id
                    };

                    try
                    {
                        _context.Clients.Add(client);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Client saved successfully for {Email}", Input.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating Client for {Email}", Input.Email);
                        ModelState.AddModelError(string.Empty, "Error creating client. Check logs.");
                        return Page();
                    }
                }

                _logger.LogInformation("Signing in user {Email}", Input.Email);

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    _logger.LogInformation("Redirecting to RegisterConfirmation for {Email}", Input.Email);
                    return RedirectToPage("RegisterConfirmation",
                        new { email = Input.Email, returnUrl = returnUrl });
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                _logger.LogInformation("Redirecting to {ReturnUrl}", returnUrl);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                _logger.LogWarning("UserManager error: {Error}", error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }


        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}
