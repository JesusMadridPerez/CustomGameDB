using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CustomGameDB.encriptacion
{
    public static class Encriptacion
    {
        private static readonly string HexKey = "3f3d728b319dc614921e573c93c4f4a3693e3efa6dbd317982202253a947fe84";
        private const int IvSize = 16;
        private static byte[] GetKeyBytes(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKeyBytes(HexKey);
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using (var ms = new MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length);
                    using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return null;

            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] iv = new byte[IvSize];
            byte[] cipher = new byte[fullCipher.Length - IvSize];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, IvSize);
            Buffer.BlockCopy(fullCipher, IvSize, cipher, 0, cipher.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKeyBytes(HexKey);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipher))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }


    }


}

