namespace TransportHub.Api.Dtos;

public class LoginRequestDto
{
    public required string User { get; set; }
    public required string Password { get; set; }
    public bool? DisconnectOtherSessions { get; set; }
}

public class LoginResponseDto
{
    public string? User { get; set; }
    public string? Key { get; set; }
}

