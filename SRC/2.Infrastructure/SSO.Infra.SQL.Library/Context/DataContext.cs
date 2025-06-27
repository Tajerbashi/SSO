namespace SSO.Infra.SQL.Library.Context;

public class DataContext : BaseDataContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
}
