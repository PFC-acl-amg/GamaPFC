using Core;
using Core.Encryption;
using Core.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Gama.Atenciones.Business
{
    public class Persona : TimestampedModel
    {
        public virtual string Nif { get; set; } = "";
        public virtual string _SavedNif { get; set; } = "";
        public virtual string ComoConocioAGama { get; set; }
        public virtual string DireccionPostal { get; set; } = "";
        public virtual string Email { get; set; }
        public virtual string EstadoCivil { get; set; }
        public virtual DateTime? FechaDeNacimiento { get; set; }
        public virtual string Facebook { get; set; } = "";
        public virtual int Id { get; set; }
        public virtual string IdentidadSexual { get; set; }
        public virtual string LinkedIn { get; set; } = "";
        public virtual string Nacionalidad { get; set; } = "";
        public virtual string NivelAcademico { get; set; }
        public virtual string Nombre { get; set; } = "";
        public virtual string Ocupacion { get; set; } = "";
        public virtual string OrientacionSexual { get; set; }
        public virtual string Telefono { get; set; } = "";
        public virtual bool TieneTrabajo { get; set; }
        public virtual string Twitter { get; set; } = "";
        public virtual byte[] Imagen { get; set; }
        public virtual DateTime? ImagenUpdatedAt {get; set;}
        public virtual string ViaDeAccesoAGama { get; set; }
        public virtual IList<Cita> Citas { get; set; }

        public Persona()
        {
            Citas = new List<Cita>();
            EncryptedFields = new List<string>();

            EncryptedFields.AddRange(new[] {
                nameof(Nombre),
                nameof(DireccionPostal),
                nameof(Nif),
                nameof(_SavedNif),
                nameof(Facebook),
                nameof(Nacionalidad),
                nameof(LinkedIn),
                nameof(Telefono),
                nameof(Twitter),
                nameof(Email),
                nameof(OrientacionSexual),
                nameof(EstadoCivil),
                nameof(IdentidadSexual),
                nameof(ComoConocioAGama),
                nameof(ViaDeAccesoAGama),
                nameof(NivelAcademico),
                nameof(Ocupacion),
                nameof(Imagen)
            });

            IsEncrypted = true;
        }

        public virtual string Edad
        {
            get
            {
                string result;

                if (FechaDeNacimiento != null)
                {
                    var difference = new DateTime(DateTime.Now.Ticks - FechaDeNacimiento.Value.Ticks);
                    result = difference.Year.ToString() + " años";
                }
                else
                {
                    result = "Edad no proporcionada";
                }

                return result;
            }
        }

        public virtual int? EdadNumerica
        {
            get
            {
                int? result;

                if (FechaDeNacimiento != null)
                {
                    try
                    {
                        var difference = new DateTime(DateTime.Now.Ticks - FechaDeNacimiento.Value.Ticks);

                        // Así prevenimos que se lance una excepción. Nadie debería tener una fecha de
                        // nacimiento mayor a la fecha actual, pero si el usuario lo introduce por error
                        // se controlorá
                        if (FechaDeNacimiento > DateTime.Now)
                            result = null;
                        else
                            result = difference.Year;
                    } catch (ArgumentOutOfRangeException)
                    {
                        result = null;
                    }
                }
                else
                {
                    result = null;
                }

                return result;
            }
        }

        public virtual void AddCita(Cita cita)
        {
            cita.Persona = this;
            this.Citas.Add(cita);
        }

        public virtual void CopyValuesFrom(Persona other)
        {
            Nif = other.Nif;
            _SavedNif = other._SavedNif;
            ComoConocioAGama = other.ComoConocioAGama;
            DireccionPostal = other.DireccionPostal;
            Email = other.Email;
            EstadoCivil = other.EstadoCivil;
            FechaDeNacimiento = other.FechaDeNacimiento;
            Facebook = other.Facebook;
            IdentidadSexual = other.IdentidadSexual;
            LinkedIn = other.LinkedIn;
            Nacionalidad = other.Nacionalidad;
            NivelAcademico = other.NivelAcademico;
            Nombre = other.Nombre;
            Ocupacion = other.Ocupacion;
            OrientacionSexual = other.OrientacionSexual;
            Telefono = other.Telefono;
            Twitter = other.Twitter;
            Imagen = other.Imagen;
            ViaDeAccesoAGama = other.ViaDeAccesoAGama;
        }
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ComoConocioAGama
    {
        [Description("No Proporcionado")]
        NoProporcionado,
        [Description("Red Informal")]
        RedInformal,
        [Description("Red Formal")]
        RedFormal,
        [Description("Difusion")]
        Difusion,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum EstadoCivil
    {
        [Description("No Proporcionado")]
        NoProporcionado,
        [Description("Soltera/o")]
        Soltera,
        [Description("Casada/o")]
        Casada,
        [Description("Separada/o")]
        Separada,
        [Description("Divorciada")]
        Divorciada
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum IdentidadSexual
    {
        [Description("No Proporcionado")]
        NoProporcionado,
        [Description("Hombre Cisexual")]
        HombreCisexual,
        [Description("Mujer Cisexual")]
        MujerCisexual,
        [Description("Hombre Transexual")]
        HombreTransexual,
        [Description("Mujer Transexual")]
        MujerTransexual,
        [Description("Otra")]
        Otra,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum NivelAcademico
    {
        [Description("No Proporcionado")]
        NoProporcionado,
        [Description("Sin Estudios")]
        SinEstudios,
        [Description("Estudios Primarios")]
        EstudiosPrimarios,
        [Description("Estudios Secundarios")]
        EstudiosSecundarios,
        [Description("Ciclo Formativo de Grado Medio")]
        CicloFormativoDeGradoMedio,
        [Description("Estudio Universitario Medio / Ciclo Formativo de Grado Superior")]
        EstudioUniversitarioMedioOCicloFormativoDeGradoSuperior,
        [Description("Estudios Universitarios (Licenciatura)")]
        EstudiosUniversitarios,
        [Description("Estudio de Postgrado (o Máster)")]
        EstudioDePostgradoOMaster
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OrientacionSexual
    {
        [Description("No Proporcionado")]
        NoProporcionado,
        [Description("Heterosexual")]
        Heterosexual,
        [Description("Bisexual")]
        Bisexual,
        [Description("Lesbiana")]
        Lesbiana,
        [Description("Gay")]
        Gay,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ViaDeAccesoAGama
    {
        [Description("No Proporcionado")]
        NoProporcionado,
        [Description("Personal")]
        Personal,
        [Description("Telefónica")]
        Telefonica,
        [Description("Email")]
        Email
    }
}
