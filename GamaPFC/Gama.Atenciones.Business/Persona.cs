﻿using Core.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Business
{
    public class Persona
    {
        public virtual ComoConocioAGama ComoConocioAGama { get; set; }
        public virtual string DireccionPostal { get; set; }
        public virtual string Email { get; set; }
        public virtual EstadoCivil EstadoCivil { get; set; }
        public virtual DateTime? FechaDeNacimiento { get; set; }
        public virtual string Facebook { get; set; }
        public virtual int Id { get; set; }
        public virtual IdentidadSexual IdentidadSexual { get; set; }
        public virtual string LinkedIn { get; set; }
        public virtual string Nacionalidad { get; set; }
        public virtual string Nif { get; set; }
        public virtual NivelAcademico NivelAcademico { get; set; }
        public virtual string Nombre { get; set; }
        public virtual int NumeroDeAtendido { get; set; }
        public virtual string Ocupacion { get; set; }
        public virtual OrientacionSexual OrientacionSexual { get; set; }
        public virtual string Telefono { get; set; }
        public virtual bool TieneTrabajo { get; set; }
        public virtual string Twitter { get; set; }
        public virtual ViaDeAccesoAGama ViaDeAccesoAGama { get; set; }
        public virtual IList<Cita> Citas { get; set; }

        public Persona()
        {
            this.Citas = new List<Cita>();
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
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ComoConocioAGama
    {
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
        [Description("Personal")]
        Personal,
        [Description("Telefónica")]
        Telefonica,
        [Description("Email")]
        Email
    }
}
