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
            var propertyValue = cleanText;

            string value = "";
            for (int i = 0; i < propertyValue.ToString().Length; i++)
            {
                var theChar = (char)((int)propertyValue.ToString()[i] + 1);
                value += theChar;
            }

            return value;
        }

        public static string Decrypt(string cleanText)
        {
            var propertyValue = cleanText;

            string value = "";
            for (int i = 0; i < propertyValue.ToString().Length; i++)
            {
                var theChar = (char)((int)propertyValue.ToString()[i] - 1);
                value += theChar;
            }

            return value;
        }
    }
}
