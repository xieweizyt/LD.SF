using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LdSf;

public static class RsaEnvelopeService
{
    public static EncryptedEnvelopeDto? TryEncrypt(string? publicKeyPem, AuthorizationPayloadDto payload)
    {
        if (string.IsNullOrWhiteSpace(publicKeyPem))
        {
            return null;
        }

        try
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var cipher = rsa.Encrypt(Encoding.UTF8.GetBytes(json), RSAEncryptionPadding.OaepSHA256);
            return new EncryptedEnvelopeDto("RSA-OAEP-SHA256", Convert.ToBase64String(cipher));
        }
        catch
        {
            return null;
        }
    }
}
