using WorkFlowApp.Data.Entities;
using WorkFlowApp.DTOs;
using WorkFlowApp.DTOs.Requests;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Services;

public class UserService : IUserService
{
	private readonly IDataRepo _dataRepo;
	private readonly IEncryptionService _encryptionService;

	public UserService(IDataRepo dataRepo, IEncryptionService encryptionService)
	{
		this._dataRepo = dataRepo;
		this._encryptionService = encryptionService;
	}

	public UserDto? DoesUserHaveValidCredentials(string email, string password)
	{
		var user = this._dataRepo.GetUserByEmail(email);

		if (user is null) return null;

		var role = this._dataRepo.GetRoleById(user.RoleId);

		if (password is null || user.Password != password) return null;

		return new UserDto
		{
			Email = user.Email,
			Name = user.Name,
			Role = role!.Name,
			Scope = role.Scope,
			Password = user.Password,
		};
	}

	public void CreateUser(SigninRequestDto request)
	{
		var existingUser = this._dataRepo.GetUserByEmail(request.Username);

		if (existingUser != null) return;

		var user = new User
		{
			Name = request.Name,
			Email = request.Username,
			Password = this._encryptionService.Encrypt(request.Password)!,
			RoleId = request.RoleId,
		};

		this._dataRepo.CreateUser(user);
	}


}
