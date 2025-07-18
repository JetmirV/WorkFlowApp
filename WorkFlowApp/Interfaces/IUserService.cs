using WorkFlowApp.DTOs;
using WorkFlowApp.DTOs.Requests;

namespace WorkFlowApp.Interfaces;
public interface IUserService
{
	void CreateUser(SigninRequestDto request);
	UserDto? DoesUserHaveValidCredentials(string email, string password);
}