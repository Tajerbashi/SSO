namespace SSO.SharedKernel.Utilities.Library.DapperProvider.QueryModels;

public class ResultQuery<TModel> : IQueryModel
{
    public IEnumerable<TModel> Data { get; set; }
    public int Count { get; set; }
    public bool ExistData { get; set; }
}

