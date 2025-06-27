using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SSO.Infra.SQL.Library.Common.DataContext;

public abstract class BaseDataContext : IdentityDbContext
{
}


public class DataContext : BaseDataContext
{

}