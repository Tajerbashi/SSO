﻿namespace SSO.Infra.SQL.Library.Identity.Entities;

[Table("UserClaims", Schema = "Identity")]
public class UserClaimIdentity : IdentityUserClaim<long>, IAuditableEntity<int>
{
    public EntityId EntityId { get; private set; } = Guid.NewGuid();
    public bool IsDeleted { get; private set; }
    public bool IsActive { get; private set; }
}
