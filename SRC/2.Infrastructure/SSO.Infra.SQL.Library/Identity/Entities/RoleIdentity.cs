﻿namespace SSO.Infra.SQL.Library.Identity.Entities;

[Table("Roles", Schema = "Identity")]
public class RoleIdentity : IdentityRole<long>, IAuditableEntity<long>
{
    public string Title { get; private set; }
    public EntityId EntityId { get; private set; } = Guid.NewGuid();
    public bool IsDeleted { get; private set; }
    public bool IsActive { get; private set; }
    public RoleIdentity() : base()
    {
    }
    public RoleIdentity(string roleName) : base(roleName)
    {
    }
    public RoleIdentity(string roleName, string title) : base(roleName)
    {
        Title = title;
        Description = $"Create New Role With {roleName} ({title})";
    }
    public RoleIdentity(string roleName, string title, string description) : base(roleName)
    {
        Title = title;
        Description = description;
    }
    public string Description { get; private set; } = string.Empty;
    public override string ToString()
    {
        return $"{Name} ({Description})";
    }
}
