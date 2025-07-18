namespace WorkFlowApp.Data.Entities;

#nullable disable
public class User
{
	public string Name { get; set; }
	public string Email { get; set; }
	public string Password { get; set; }
	public int RoleId { get; set; }
}
