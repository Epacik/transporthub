namespace TransportHub.Api.Dtos;

public class UserDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }
    public DateTime? PasswordExpirationDate { get; set; }
    public UserType UserType { get; set; }
    public bool MultiLogin { get; set; }
    public bool Disabled { get; set; }
}

public class UserAddDto
{
    public string? Name { get; set; }
    public string? Picture { get; set; }
    public string? Password { get; set; }
    public DateTime? PasswordExpirationDate { get; set; }
    public UserType UserType { get; set; }
    public bool MultiLogin { get; set; }
}

public class UserAdminUpdateDto
{
    public string? Name { get; set; }
    public string? Picture { get; set; }
    public DateTime? PasswordExpirationDate { get; set; }
    public UserType UserType { get; set; }
    public bool MultiLogin { get; set; }
    public bool Disabled { get; set; }
}

public class UserUpdateDto
{
    public string? Name { get; set; }
    public string? Picture { get; set; }
}

public class UserUpdatePasswordDto
{
    public UserUpdatePasswordDto(string password)
    {
        Password = password;
    }

    public string? Password { get; set; }
}
