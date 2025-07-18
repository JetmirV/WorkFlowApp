namespace WorkFlowApp.Models;

#nullable disable
public class JwtTokenConfig
{
	public string Secret { get; set; }
	public string Issuer { get; set; }
	public int AccessTokenExpiration { get; set; }
	public int RefreshTokenExpiration { get; set; }
}
