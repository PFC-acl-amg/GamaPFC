using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IEncryptable
    {
        List<string> EncryptedFields { get; set; }
        void Encrypt();
        void Decrypt();
        bool IsEncrypted { get; set; }
    }
}
