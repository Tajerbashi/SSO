namespace SSO.Infra.SQL.Library.Identity.Entities;

[Table("RoleClaims", Schema = "Identity")]
public class RoleClaimIdentity : IdentityRoleClaim<long>, IAuditableEntity<int>
{
    public EntityId EntityId { get; private set; } = Guid.NewGuid();
    public bool IsDeleted { get; private set; }
    public bool IsActive { get; private set; }
}
