using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//GXSF2017DX7HGFA2728AKGLODFC285

namespace Core.Encryption
{
    public static class StringCipher
    {
        private static string _PassPhrase = "";
        public static string PassPhrase
        {
            get
            {
                if (string.IsNullOrEmpty(_PassPhrase))
                {
                    var _myResourceDictionary = new ResourceDictionary();
                    _myResourceDictionary.Source = new Uri("/Core;component/Encryption/Keys.xaml",
                            UriKind.RelativeOrAbsolute);
                    _PassPhrase = _myResourceDictionary["PassPhrase"] as string;

                }

                return _PassPhrase;
            }
        }

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText)
        {
            try
            {
                string cipherText;
                var rijndael = new RijndaelManaged()
                {
                    Key = Encoding.ASCII.GetBytes(PassPhrase),
                    Mode = CipherMode.CBC,
                    BlockSize = 256,
                    Padding = PaddingMode.PKCS7,
                    IV = Encoding.ASCII.GetBytes("741952hheeyy66#cs!9hjv887mxx7@8y")
                };
                ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                            streamWriter.Flush();
                        }
                        cipherText = Convert.ToBase64String(memoryStream.ToArray());
                        //cryptoStream.FlushFinalBlock();
                    }
                }
                return cipherText;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                string plainText;
                byte[] cipherArray = Convert.FromBase64String(cipherText);
                var rijndael = new RijndaelManaged()
                {
                    Key = Encoding.ASCII.GetBytes(PassPhrase),
                    Mode = CipherMode.CBC,
                    BlockSize = 256,
                    Padding = PaddingMode.PKCS7,
                    IV = Encoding.ASCII.GetBytes("741952hheeyy66#cs!9hjv887mxx7@8y")
                };
                ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

                using (var memoryStream = new MemoryStream(cipherArray))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream))
                        {
                            plainText = streamReader.ReadToEnd();
                        }
                    }
                }
                return plainText;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
