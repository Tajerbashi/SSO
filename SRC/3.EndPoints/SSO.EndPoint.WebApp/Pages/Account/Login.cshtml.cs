using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace SSO.EndPoint.WebApp.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IIdentityService _identityService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        IIdentityService identityService,
        IWebHostEnvironment env,
        ILogger<LoginModel> logger)
    {
        _identityService = identityService;
        _env = env;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }
    public bool EnvironmentIsDevelopment => _env.IsDevelopment();
    [TempData]
    public string ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public IActionResult OnGet()
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(ReturnUrl);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _identityService.TokenService.LoginAsync(new()
        {
            Username = Input.Username,
            Password = Input.Password,
            IsRemember = Input.RememberMe,
            ReturnUrl = ReturnUrl
        });

        if (result.Succeeded)
        {
            return RedirectToLocal(ReturnUrl);
        }

        if (result.Message.Contains("locked", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(string.Empty, "Account locked. Please try again later.");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return Page();
    }

    public async Task<IActionResult> OnGetLoginAs()
    {
        // Only allow in development environment
        if (!_env.IsDevelopment())
        {
            _logger.LogWarning("LoginAs attempted in non-development environment");
            return Forbid();
        }

        try
        {
            var result = await _identityService.TokenService.LoginAsync(new()
            {
                Username = "tajerbashi",
                Password = "Admin123!",
                IsRemember = true,
                ReturnUrl = ReturnUrl
            });

            if (!result.Succeeded)
            {
                _logger.LogWarning("LoginAs failed: {Message}", result.Message);
                ErrorMessage = "Automatic login failed";
                return Page();
            }

            _logger.LogInformation("LoginAs succeeded for user tajerbashi");
            return RedirectToLocal(ReturnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LoginAs");
            ErrorMessage = "An error occurred during automatic login";
            return Page();
        }
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToPage("/Index");
    }
}