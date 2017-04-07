using Core;
using Core.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Business
{
    public class Asistente : TimestampedModel, IEncryptable
    {
        public virtual int Id { get; set; }
        public virtual string Nif { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Email { get; set; }
        public virtual string Telefono { get; set; } = "";
        public virtual byte[] Imagen { get; set; }

        public virtual IList<Cita> Citas { get; set; }

        public virtual List<string> EncryptedFields { get; set; }
        public virtual bool IsEncrypted { get; set; }

        public Asistente()
        {
            EncryptedFields = new List<string>();

            //EncryptedFields.AddRange(new[] {
            //    nameof(Nombre),
            //    nameof(Nif),
            //    nameof(Telefono),
            //    nameof(Email),
            //    nameof(Imagen)
            //});

            IsEncrypted = true;
        }

        public virtual void Encrypt()
        {
            if (IsEncrypted)
                return;

            foreach (var propertyName in EncryptedFields)
            {
                var propertyInfo = this.GetType().GetProperty(propertyName);
                var propertyValue = propertyInfo.GetValue(this, null);

                if (propertyValue != null)
                {
                    if (propertyName == nameof(Imagen))
                    {
                        propertyInfo.SetValue(this, Cipher.Encrypt((byte[])propertyValue));
                    }
                    else
                    {
                        var value = propertyValue.ToString();
                        if (!String.IsNullOrWhiteSpace(value))
                        {
                            propertyInfo.SetValue(this, Cipher.Encrypt(value));
                        }
                    }
                }
            }

            IsEncrypted = true;
        }

        public virtual Asistente DecryptFluent()
        {
            Decrypt();
            return this;
        }

        public virtual void Decrypt()
        {
            try
            {
                if (!IsEncrypted)
                    return;

                foreach (var propertyName in EncryptedFields)
                {
                    var propertyInfo = this.GetType().GetProperty(propertyName);
                    var propertyValue = propertyInfo.GetValue(this, null);

                    if (propertyValue != null)
                    {
                        if (propertyName == nameof(Imagen))
                        {
                            try
                            {
                                propertyInfo.SetValue(this, Cipher.Decrypt((byte[])propertyValue));
                            }
                            catch (Exception ex)
                            {
                                var ok = ex.Message;
                                throw;
                            }
                        }
                        else
                        {
                            var value = propertyValue.ToString();
                            if (!String.IsNullOrWhiteSpace(value))
                            {
                                propertyInfo.SetValue(this, Cipher.Decrypt(value));
                            }
                        }
                    }
                }

                IsEncrypted = false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
