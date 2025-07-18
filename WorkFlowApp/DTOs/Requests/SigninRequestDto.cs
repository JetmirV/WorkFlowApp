namespace WorkFlowApp.DTOs.Requests;

public class SigninRequestDto
{
	public required string Name { get; set; }
	public required string Username { get; set; } //Email
	public required string Password { get; set; }
	public required int RoleId { get; set; }
}
