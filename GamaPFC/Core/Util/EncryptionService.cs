using Core.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Util
{
    public static class EncryptionService
    {
        public static string Encrypt(string cleanText)
        {
            return StringCipher.Encrypt(cleanText);
        }

        public static string Decrypt(string encryptedText)
        {
            return StringCipher.Decrypt(encryptedText);
        }
    }
}
