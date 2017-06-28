using Core.Encryption;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class ModelBase : IEncryptable
    {
        public virtual List<string> EncryptedFields { get; set; }

        public virtual bool IsEncrypted { get; set; }

        public virtual void Encrypt()
        {
            if (IsEncrypted)
                return;

            IsEncrypted = true;

            if (EncryptedFields != null)
            {
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
                        else if (propertyValue is Enum)
                        {
                            string enumValue = propertyValue.ToString();
                            if (!string.IsNullOrWhiteSpace(enumValue))
                            {
                                propertyInfo.SetValue(this, Cipher.Encrypt(enumValue));
                            }
                        }
                        else if (propertyValue is IEncryptable)
                        {
                            var encryptableValue = propertyValue as IEncryptable;
                            encryptableValue.Encrypt();
                        }
                    }
                }
            }
        }

        public virtual void Decrypt()
        {
            if (!IsEncrypted)
                return;

            IsEncrypted = false;

            if (EncryptedFields != null)
            {
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
                            string stringValue = propertyValue.ToString();
                            if (!string.IsNullOrWhiteSpace(stringValue))
                            {
                                propertyInfo.SetValue(this, Cipher.Decrypt(stringValue));
                            }
                        }
                        else if (propertyValue is Enum)
                        {
                            string enumValue = propertyValue.ToString();
                            if (!string.IsNullOrWhiteSpace(enumValue))
                            {
                                propertyInfo.SetValue(this, Cipher.Decrypt(enumValue));
                            }
                        }
                        else if (propertyValue is IEncryptable)
                        {
                            var encryptableValue = propertyValue as IEncryptable;
                            encryptableValue.Decrypt();
                        }
                        else if (propertyValue is IEnumerable)
                        {
                            var list = propertyValue as IEnumerable;
                            foreach (var item in list)
                            {
                                if (item is IEncryptable)
                                    ((IEncryptable)item).Decrypt();
                                else
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public virtual object DecryptFluent()
        {
            Decrypt();
            return this;
        }

        public virtual void CopyValuesFrom() { }
    }
}
