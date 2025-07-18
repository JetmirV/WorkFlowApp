namespace WorkFlowApp.DTOs.Requests;

public class LoginRequestDto
{
	public required string Username { get; set; } //Email
	public required string Password { get; set; }
}
