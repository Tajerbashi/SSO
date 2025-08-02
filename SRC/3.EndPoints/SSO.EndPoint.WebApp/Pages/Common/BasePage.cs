namespace SSO.EndPoint.WebApp.Pages.Common;

public abstract class BasePage : PageModel
{
}
[Authorize]
public abstract class AuthPage : BasePage
{
    protected AuthPage()
    {
        if (!User.Identity.IsAuthenticated)
        {
            Redirect("/Account/Login");
        }
    }
}