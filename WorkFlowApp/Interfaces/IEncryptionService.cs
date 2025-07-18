namespace WorkFlowApp.Interfaces;

public interface IEncryptionService
{
	string? Decrypt(string encryptedText);
	string? Encrypt(string plainText);
}