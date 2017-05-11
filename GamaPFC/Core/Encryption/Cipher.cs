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
    public static class Cipher
    {
        static Cipher()
        {
            _PassPhrase = @"560A18CD-GAMA-4CF0-2017-671F9B6B";
            _IV = @"741952hheeyy66#cs!9hjv887mxx7@8y";
        }

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
            set { _PassPhrase = value; }
        }

        private static string _IV = "";
        public static string IV
        {
            get
            {
                if (string.IsNullOrEmpty(_IV))
                {
                    var _myResourceDictionary = new ResourceDictionary();
                    _myResourceDictionary.Source = new Uri("/Core;component/Encryption/Keys.xaml",
                            UriKind.RelativeOrAbsolute);
                    _IV = _myResourceDictionary["IV"] as string;

                }

                return _IV;
            }
            set { _IV = value; }
        }

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase = null, string iv = null)
        {
            if (passPhrase != null && iv != null)
            {
                _PassPhrase = passPhrase;
                IV = iv;
            }

            try
            {
                string cipherText;
                var rijndael = new RijndaelManaged()
                {
                    Key = Encoding.UTF8.GetBytes(PassPhrase),
                    Mode = CipherMode.CBC,
                    BlockSize = 256,
                    Padding = PaddingMode.PKCS7,
                    IV = Encoding.UTF8.GetBytes(IV)
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
                        //cryptoStream.FlushFinalBlock(); Dará error de que se llama dos veces. 
                    }
                }
                return cipherText;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string Decrypt(string cipherText, string passPhrase = null, string iv = null)
        {
            if (passPhrase != null && iv != null)
            {
                _PassPhrase = passPhrase;
                _IV = iv;
            }

            try
            {
                string plainText;
                byte[] cipherArray = Convert.FromBase64String(cipherText);
                var rijndael = new RijndaelManaged()
                {
                    Key = Encoding.UTF8.GetBytes(PassPhrase),
                    Mode = CipherMode.CBC,
                    BlockSize = 256,
                    Padding = PaddingMode.PKCS7,
                    IV = Encoding.UTF8.GetBytes(IV)
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

        public static byte[] Encrypt(byte[] plainText, string passPhrase = null, string iv = null)
        {
            if (passPhrase != null && iv != null)
            {
                _PassPhrase = passPhrase;
                IV = iv;
            }

            try
            {
                using (var des = new RijndaelManaged())
                {
                    des.Mode = CipherMode.CBC;
                    des.Padding = PaddingMode.PKCS7;
                    des.BlockSize = 256;

                    des.Key = Encoding.UTF8.GetBytes(PassPhrase);
                    des.IV = Encoding.UTF8.GetBytes(IV);

                    using (var memoryStream = new MemoryStream())
                    {
                        var cryptoStream = new CryptoStream(memoryStream,
                            des.CreateEncryptor(), CryptoStreamMode.Write);

                        cryptoStream.Write(plainText, 0, plainText.Length);
                        cryptoStream.FlushFinalBlock();

                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static byte[] Decrypt(byte[] cipherText, string passPhrase = null, string iv = null)
        {
            if (passPhrase != null && iv != null)
            {
                _PassPhrase = passPhrase;
                IV = iv;
            }

            try
            {
                using (var des = new RijndaelManaged())
                {
                    des.Mode = CipherMode.CBC;
                    des.Padding = PaddingMode.PKCS7;
                    des.BlockSize = 256;

                    des.Key = Encoding.UTF8.GetBytes(PassPhrase);
                    des.IV = Encoding.UTF8.GetBytes(IV);

                    using (var memoryStream = new MemoryStream())
                    {
                        var cryptoStream = new CryptoStream(memoryStream,
                            des.CreateDecryptor(), CryptoStreamMode.Write);

                        cryptoStream.Write(cipherText, 0, cipherText.Length);
                        cryptoStream.FlushFinalBlock();

                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
