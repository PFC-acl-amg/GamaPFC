using Core;
using Gama.Cooperacion.Business;
using System.ComponentModel.DataAnnotations;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class EmailWrapper : ModelWrapper<Email>
    {
        public EmailWrapper(Email model) : base(model)
        {
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        [Required(ErrorMessage = "El campo de dirección es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe introducir una direccción válida")]
        public string Direccion
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DireccionOriginalValue => GetOriginalValue<string>(nameof(Direccion));

        public bool DireccionIsChanged => GetIsChanged(nameof(Direccion));

        public override string ToString()
        {
            return Direccion;
        }
    }
}
