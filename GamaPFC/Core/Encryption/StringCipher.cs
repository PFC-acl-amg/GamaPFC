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
                    IV = Encoding.ASCII.GetBytes(IV)
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

        public static byte[] EncryptImage2(byte[] plainText)
        {
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
                //byte[] cipherText;
                //var rijndael = new RijndaelManaged()
                //{
                //    Key = Encoding.ASCII.GetBytes(PassPhrase),
                //    Mode = CipherMode.CBC,
                //    BlockSize = 256,
                //    Padding = PaddingMode.PKCS7,
                //    IV = Encoding.ASCII.GetBytes(IV)
                //};

                //ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

                //using (var memoryStream = new MemoryStream())
                //{
                //    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                //    {
                //        using (var streamWriter = new StreamWriter(cryptoStream))
                //        {
                //            streamWriter.Write(plainText);
                //            streamWriter.Flush();
                //        }
                //        cipherText = memoryStream.ToArray();
                //        //cryptoStream.FlushFinalBlock();
                //    }
                //}
                //return cipherText;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static byte[] DecryptImage2(byte[] cipherText)
        {
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
                //byte[] plainText;
                //byte[] cipherArray = cipherText;
                //var rijndael = new RijndaelManaged()
                //{
                //    Key = Encoding.ASCII.GetBytes(PassPhrase),
                //    Mode = CipherMode.CBC,
                //    BlockSize = 256,
                //    Padding = PaddingMode.PKCS7,
                //    IV = Encoding.ASCII.GetBytes(IV)
                //};
                //ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

                //using (var memoryStream = new MemoryStream(cipherArray))
                //{
                //    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                //    {
                //        using (var streamReader = new StreamReader(cryptoStream))
                //        {
                //            plainText = Encoding.UTF8.GetBytes(streamReader.ReadToEnd());
                //        }
                //    }
                //}
                //return plainText;
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
                    IV = Encoding.ASCII.GetBytes(IV)
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

        public static byte[] EncryptImage(byte[] clearData)
        {
            // Create a MemoryStream to accept the encrypted bytes 
            MemoryStream ms = new MemoryStream();

            // Create a symmetric algorithm. 
            // We are going to use Rijndael because it is strong and
            // available on all platforms. 
            // You can use other algorithms, to do so substitute the
            // next line with something like 
            //      TripleDES alg = TripleDES.Create(); 
            //Rijndael alg = Rijndael.Create();

            // Now set the key and the IV. 
            // We need the IV (Initialization Vector) because
            // the algorithm is operating in its default 
            // mode called CBC (Cipher Block Chaining).
            // The IV is XORed with the first block (8 byte) 
            // of the data before it is encrypted, and then each
            // encrypted block is XORed with the 
            // following block of plaintext.
            // This is done to make encryption more secure. 

            // There is also a mode called ECB which does not need an IV,
            // but it is much less secure. 
            var rijndael = new RijndaelManaged()
            {
                Key = Encoding.ASCII.GetBytes(PassPhrase),
                Mode = CipherMode.ECB,
                BlockSize = 256,
                Padding = PaddingMode.PKCS7,
            };

            // Create a CryptoStream through which we are going to be
            // pumping our data. 
            // CryptoStreamMode.Write means that we are going to be
            // writing data to the stream and the output will be written
            // in the MemoryStream we have provided. 
            CryptoStream cs = new CryptoStream(ms, rijndael.CreateEncryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the encryption 
            cs.Write(clearData, 0, clearData.Length);

            // Close the crypto stream (or do FlushFinalBlock). 
            // This will tell it that we have done our encryption and
            // there is no more data coming in, 
            // and it is now a good time to apply the padding and
            // finalize the encryption process. 
            cs.Close();

            // Now get the encrypted data from the MemoryStream.
            // Some people make a mistake of using GetBuffer() here,
            // which is not the right way. 
            byte[] encryptedData = ms.ToArray();

            return encryptedData;
        }

        public static byte[] DecryptImage(byte[] cipherData)
        {
            MemoryStream ms = new MemoryStream();

            var rijndael = new RijndaelManaged()
            {
                Key = Encoding.ASCII.GetBytes(PassPhrase),
                Mode = CipherMode.ECB,
                BlockSize = 256,
                Padding = PaddingMode.PKCS7,
            };

            CryptoStream cs = new CryptoStream(ms, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();

            byte[] decryptedData = ms.ToArray();

            return decryptedData;
        }
    }
}
