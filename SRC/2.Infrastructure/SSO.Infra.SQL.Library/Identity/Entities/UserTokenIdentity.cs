namespace SSO.Infra.SQL.Library.Identity.Entities;

[Table("UserTokens", Schema = "Identity")]
public class UserTokenIdentity : IdentityUserToken<long>, IAuditableEntity<long>
{
    public long Id { get; private set; }
    public EntityId EntityId { get; private set; } = Guid.NewGuid();

    public bool IsDeleted { get; private set; }
    public bool IsActive { get; private set; }
}
