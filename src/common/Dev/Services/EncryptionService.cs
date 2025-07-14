using Dev.Common;

using System.Security.Cryptography;
using System.Text;

namespace Dev.Services;

public interface IEncryptionService
{
    string CreateSaltKey(int size);
    string CreatePasswordHash(string password, string saltKey, string passwordFormat);
    string EncryptText(string plainText, string encryptionPrivateKey = "");
    string DecryptText(string cipherText, string encryptionPrivateKey = "");
}


public class EncryptionService : IEncryptionService
{
    private readonly bool _useAesEncryptionAlgorithm;
    private readonly string _encryptionKey;

    public EncryptionService()
    {
        _useAesEncryptionAlgorithm = true;
        _encryptionKey = "1234567890123456";
    }

    protected static byte[] EncryptTextToMemory(string data, SymmetricAlgorithm provider)
    {
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write))
        {
            var toEncrypt = Encoding.Unicode.GetBytes(data);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
        }

        return ms.ToArray();
    }
    protected static string DecryptTextFromMemory(byte[] data, SymmetricAlgorithm provider)
    {
        using var ms = new MemoryStream(data);
        using var cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.Unicode);

        return sr.ReadToEnd();
    }
    protected virtual SymmetricAlgorithm GetEncryptionAlgorithm(string encryptionKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(encryptionKey);

        SymmetricAlgorithm provider = _useAesEncryptionAlgorithm ? Aes.Create() : TripleDES.Create();

        var vectorBlockSize = provider.BlockSize / 8;

        provider.Key = Encoding.ASCII.GetBytes(encryptionKey[0..16]);
        provider.IV = Encoding.ASCII.GetBytes(encryptionKey[^vectorBlockSize..]);

        return provider;
    }

    public virtual string CreateSaltKey(int size)
    {
        //generate a cryptographic random number
        using var provider = RandomNumberGenerator.Create();
        var buff = new byte[size];
        provider.GetBytes(buff);

        // Return a Base64 string representation of the random number
        return Convert.ToBase64String(buff);
    }

    public virtual string CreatePasswordHash(string password, string saltkey, string passwordFormat)
    {
        return HashHelper.CreateHash(Encoding.UTF8.GetBytes(string.Concat(password, saltkey)), passwordFormat);
    }
    public virtual string EncryptText(string plainText, string encryptionPrivateKey = "")
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        if (string.IsNullOrEmpty(encryptionPrivateKey))
            encryptionPrivateKey = _encryptionKey;

        using var provider = GetEncryptionAlgorithm(encryptionPrivateKey);
        var encryptedBinary = EncryptTextToMemory(plainText, provider);

        return Convert.ToBase64String(encryptedBinary);
    }

    public virtual string DecryptText(string cipherText, string encryptionPrivateKey = "")
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        if (string.IsNullOrEmpty(encryptionPrivateKey))
            encryptionPrivateKey = _encryptionKey;

        using var provider = GetEncryptionAlgorithm(encryptionPrivateKey);

        var buffer = Convert.FromBase64String(cipherText);
        return DecryptTextFromMemory(buffer, provider);
    }

}
