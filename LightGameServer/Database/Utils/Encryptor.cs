using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LightGameServer.Database.Utils
{
    class Encryptor
    {
        private const int KEYSIZE = 256;
        private const int DERIVATION_ITERATIONS = 1000;
        private const string ENCRYPT_PASSWORD = "a6wP6V-JXct%9@uKWMjP8jVZLS_c+VJY*F4TUuWkRKRAJ6m9##yAc!5v8fRpdj+ErN8%UHr@Pp^JGf=P8+k9p^*SDg$D$fW-PUzN@XyA$zVUt=JY%ysBnXMt7CLb?xrZr6^T#*5FQRwX2VP^bPN5zQ+R*u=ek^bppB@aFdQntQuWZXYnM2q!Y!mN855ACs#p!zyq6e8hxM9yd_^3YQ5Q3M@YRGFUb6vD?W*jC3qyY99NV7_DZ_mZY5Jf^Bvwz47Q";

        public static string EncryptDeviceId(uint deviceId)
        {
            return Encrypt(deviceId.ToString(), ENCRYPT_PASSWORD);
        }

        public static uint DecryptDeviceId(string deviceId)
        {
            return uint.Parse(Decrypt(deviceId, ENCRYPT_PASSWORD));
        }

        private static string Encrypt(string plainText, string passPhrase)
        {
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DERIVATION_ITERATIONS))
            {
                var keyBytes = password.GetBytes(KEYSIZE / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        private static string Decrypt(string cipherText, string passPhrase)
        {
            try
            {
                var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KEYSIZE / 8).ToArray();
                var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KEYSIZE / 8).Take(KEYSIZE / 8).ToArray();
                var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((KEYSIZE / 8) * 2)
                    .Take(cipherTextBytesWithSaltAndIv.Length - ((KEYSIZE / 8) * 2)).ToArray();

                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DERIVATION_ITERATIONS))
                {
                    var keyBytes = password.GetBytes(KEYSIZE / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (var cryptoStream =
                                    new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    var plainTextBytes = new byte[cipherTextBytes.Length];
                                    var decryptedByteCount =
                                        cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }
            catch (FormatException)
            {
                //TODO THE USER MODIFIED HIS/HER DEVICE_ID, PUNISH HIM
            }
            catch (CryptographicException)
            {
                //TODO THE USER MODIFIED HIS/HER DEVICE_ID, PUNISH HIM
            }

            return "";
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
