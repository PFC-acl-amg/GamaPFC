using Core.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ModelBase : IEncryptable
    {
        public virtual List<string> EncryptedFields { get; set; }

        public virtual bool IsEncrypted { get; set; }

        public virtual void Encrypt()
        {
            if (IsEncrypted || EncryptedFields == null)
                return;

            foreach (var propertyName in EncryptedFields)
            {
                var propertyInfo = this.GetType().GetProperty(propertyName);
                var propertyValue = propertyInfo.GetValue(this, null);

                if (propertyValue != null)
                {
                    if (propertyValue is byte[])
                    {
                        propertyInfo.SetValue(this, Cipher.Encrypt((byte[])propertyValue));
                    }
                    else if (propertyValue is string)
                    {
                        string value = propertyValue.ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            propertyInfo.SetValue(this, Cipher.Encrypt(value));
                        }
                    }
                }
            }

            IsEncrypted = true;
        }

        public virtual void Decrypt()
        {
            if (!IsEncrypted || EncryptedFields == null)
                return;

            foreach (var propertyName in EncryptedFields)
            {
                var propertyInfo = this.GetType().GetProperty(propertyName);
                var propertyValue = propertyInfo.GetValue(this, null);

                if (propertyValue != null)
                {
                    if (propertyValue is byte[])
                    {
                        propertyInfo.SetValue(this, Cipher.Decrypt((byte[])propertyValue));
                    }
                    else if (propertyValue is string)
                    {
                        string value = propertyValue.ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            propertyInfo.SetValue(this, Cipher.Decrypt(value));
                        }
                    }
                }
            }

            IsEncrypted = false;
        }

        public virtual object DecryptFluent()
        {
            Decrypt();
            return this;
        }
    }
}
