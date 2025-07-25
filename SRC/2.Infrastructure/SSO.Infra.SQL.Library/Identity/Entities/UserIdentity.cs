using SSO.Core.Domain.Library.Aggregates.Identity.UserAggregate;
using SSO.Infra.SQL.Library.Identity.Entities.Parameters;

namespace SSO.Infra.SQL.Library.Identity.Entities;

[Table("Users", Schema = "Identity")]
public class UserIdentity : IdentityUser<long>, IAuditableEntity<long>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string NationalCode { get; private set; }
    public bool IsDeleted { get; private set; }
    public bool IsActive { get; private set; }
    public EntityId EntityId { get; private set; } = Guid.NewGuid();

    public UserIdentity()
    {

    }
    public UserIdentity(UserCreateParameters parameters)
    {
        UserName = parameters.UserName;
        Email = parameters.Email;
        FirstName = parameters.FirstName;
        LastName = parameters.LastName;
        PhoneNumber = parameters.PhoneNumber;
        NationalCode = parameters.NationalCode;
    }

    public static UserEntity Map(UserIdentity entity)
    {
        return UserEntity.MapModel(new(
            entity.Id,
            entity.EntityId.Value,
            entity.FirstName,
            entity.LastName,
            entity.NationalCode,
            entity.UserName,
            entity.NormalizedUserName,
            entity.Email,
            entity.NormalizedEmail,
            entity.EmailConfirmed,
            entity.PasswordHash,
            entity.SecurityStamp,
            entity.ConcurrencyStamp,
            entity.PhoneNumber,
            entity.PhoneNumberConfirmed,
            entity.TwoFactorEnabled,
            entity.LockoutEnd,
            entity.LockoutEnabled,
            entity.AccessFailedCount
            ));
    }

    public static UserIdentity Map(UserEntity entity)
    {
        return new UserIdentity()
        {
            Id = entity.Id,
            EntityId = entity.EntityId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NationalCode = entity.NationalCode,

            PhoneNumber = entity.PhoneNumber,
            PhoneNumberConfirmed = entity.PhoneNumberConfirmed,

            UserName = entity.UserName,
            NormalizedUserName = entity.NormalizedUserName,

            Email = entity.Email,
            NormalizedEmail = entity.NormalizedEmail,
            EmailConfirmed = entity.EmailConfirmed,

            TwoFactorEnabled = entity.TwoFactorEnabled,
            LockoutEnd = entity.LockoutEnd,
            AccessFailedCount = entity.AccessFailedCount,
            ConcurrencyStamp = entity.ConcurrencyStamp,
            PasswordHash = entity.PasswordHash,
            SecurityStamp = entity.SecurityStamp,
            LockoutEnabled = entity.LockoutEnabled,
            IsActive = entity.IsActive,
            IsDeleted = entity.IsDeleted,
        };
    }

}
