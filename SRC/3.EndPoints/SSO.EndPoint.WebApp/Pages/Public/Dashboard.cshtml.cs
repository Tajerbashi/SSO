namespace SSO.EndPoint.WebApp.Pages.Public;

public class DashboardModel : AuthPage
{
    private readonly ILogger<DashboardModel> _logger;

    public DashboardModel(ILogger<DashboardModel> logger)
    {
        _logger = logger;
    }


    public IActionResult OnGet()
    {
        return Page();
    }


}
