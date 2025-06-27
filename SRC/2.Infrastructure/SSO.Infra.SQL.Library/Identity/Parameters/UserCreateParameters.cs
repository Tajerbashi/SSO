namespace SSO.Infra.SQL.Library.Identity.Parameters;


public record UserCreateParameters(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string NationalCode
    );

