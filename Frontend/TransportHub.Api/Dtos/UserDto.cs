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
