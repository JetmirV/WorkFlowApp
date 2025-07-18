using System.Security.Cryptography;
using System.Text;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Services;

public class EncryptionService : IEncryptionService
{
	private readonly string _passwordHash;
	private readonly string _saltKey;
	private readonly string _vIKey;

	public EncryptionService(IConfiguration configuration)
	{
		this._passwordHash = configuration["Encryption:PasswordHash"]!;
		this._saltKey = configuration["Encryption:SaltKey"]!;
		this._vIKey = configuration["Encryption:VIKey"]!;
	}

	public string? Decrypt(string encryptedText)
	{
		try
		{
			// 1. Base64-decode the cipher text
			var cipherTextBytes = Convert.FromBase64String(encryptedText);

			// 2. Derive a 256-bit key from password and salt
			var saltBytes = Encoding.ASCII.GetBytes(this._saltKey);
			using var keyDerivation = new Rfc2898DeriveBytes(
				this._passwordHash,
				saltBytes,
				100000, // Recommended iteration count
				HashAlgorithmName.SHA256 // Secure hash algorithm
			);
			var keyBytes = keyDerivation.GetBytes(256 / 8);

			// 3. Prepare the IV (first 16 bytes of your provided IV key)
			var ivBytes = Encoding.ASCII.GetBytes(this._vIKey).Take(16).ToArray();

			// 4. Configure AES
			using var aes = Aes.Create();
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.None; // match original behavior
			aes.Key = keyBytes;
			aes.IV = ivBytes;

			// 5. Decrypt
			using var decryptor = aes.CreateDecryptor();
			using var ms = new MemoryStream(cipherTextBytes);
			using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

			var plainTextBytes = new byte[cipherTextBytes.Length];
			var decryptedByteCount = cs.Read(plainTextBytes, 0, plainTextBytes.Length);

			// 6. Convert back to string and trim any padding zeros
			return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd('\0');
		}
		catch (Exception)
		{
			return null;
		}
	}

	public string? Encrypt(string plainText)
	{
		try
		{
			// 1. Convert plaintext to bytes
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

			// 2. Derive a 256-bit key using PBKDF2
			var saltBytes = Encoding.ASCII.GetBytes(this._saltKey);
			using var keyDerivation = new Rfc2898DeriveBytes(
				this._passwordHash,
				saltBytes,
				100_000,
				HashAlgorithmName.SHA256
			);
			var keyBytes = keyDerivation.GetBytes(256 / 8);

			var ivBytes = Encoding.ASCII.GetBytes(this._vIKey).Take(16).ToArray();

			// 3. Create AES in CBC mode with zero padding
			using var aes = Aes.Create();
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.Zeros;
			aes.Key = keyBytes;
			aes.IV = ivBytes;

			// 4. Encrypt
			using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
			byte[] cipherBytes;
			using (var ms = new MemoryStream())
			using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
			{
				cs.Write(plainTextBytes, 0, plainTextBytes.Length);
				cs.FlushFinalBlock();
				cipherBytes = ms.ToArray();
			}

			// 5. Prepend IV and return Base64
			var result = aes.IV.Concat(cipherBytes).ToArray();
			return Convert.ToBase64String(result);
		}
		catch (Exception)
		{
			return null;
		}
	}
}
