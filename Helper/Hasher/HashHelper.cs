using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Helper.Hasher;

public static class HashHelper
{
    public static HashedPassword Hash(string password)
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256/8
        ));
        return new HashedPassword() {Password = hashed, Salt = Convert.ToBase64String(salt)};
    }

    public static bool CheckHash(string attemptedPassword, string hash, string salt)
    {
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: attemptedPassword,
            salt: Convert.FromBase64String(salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested:256/8
            
        ));

        return hash == hashed;
    }

}