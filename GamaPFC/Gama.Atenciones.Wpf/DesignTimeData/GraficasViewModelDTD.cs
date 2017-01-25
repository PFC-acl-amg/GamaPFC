﻿using Gama.Atenciones.Business;
using Gama.Atenciones.DataAccess;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Services;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.DesignTimeData
{
    public class GraficasViewModelDTD
    {
        public GraficasViewModelDTD()
        {
            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            var sessionFactory = new NHibernateSessionFactory();
            var rep = new PersonaRepository();
            rep.Session = sessionFactory.OpenSession();

            if (false)
            {
                var personasFake = new FakePersonaRepository().GetAll(); //personaRepository.GetAll();
                var citas = new FakeCitaRepository().GetAll();
                var atenciones = new FakeAtencionRepository().GetAll();

                personasFake.ForEach(p => p.Id = 0);
                citas.ForEach(c => c.Id = 0);
                atenciones.ForEach(a => a.Id = 0);

                var random = new Random();
                var opciones = new bool[] { true, false, true, false, true, true, false, true, false };

                for (int i = 0; i < personasFake.Count; i++)
                {
                    var persona = personasFake[i];
                    var cita = citas[i];
                    var atencion = atenciones[i];
                    var derivacion = new Derivacion
                    {
                        Id = 0,
                        Atencion = atencion,
                        EsDeFormacion = opciones[random.Next(0, 8)],
                        EsDeFormacion_Realizada = opciones[random.Next(0, 8)],
                        EsDeOrientacionLaboral = opciones[random.Next(0, 8)],
                        EsDeOrientacionLaboral_Realizada = opciones[random.Next(0, 8)],
                        EsExterna = opciones[random.Next(0, 8)],
                        EsExterna_Realizada = opciones[random.Next(0, 8)],
                        EsJuridica = opciones[random.Next(0, 8)],
                        EsJuridica_Realizada = opciones[random.Next(0, 8)],
                        EsPsicologica = opciones[random.Next(0, 8)],
                        EsPsicologica_Realizada = opciones[random.Next(0, 8)],
                        EsSocial = opciones[random.Next(0, 8)],
                        EsSocial_Realizada = opciones[random.Next(0, 8)],
                        Externa = "Externa",
                        Externa_Realizada = "Externa realizada",
                        Tipo = "",
                    };

                    atencion.Derivacion = derivacion;

                    cita.SetAtencion(atencion);
                    persona.AddCita(citas[i]);

                    //rep.Create(persona);
                }
            }

            var personas = rep.GetAll();

            HombreCisexualCount = personas.Count(p => p.IdentidadSexual == IdentidadSexual.HombreCisexual);
            HombreTransexualCount = personas.Count(p => p.IdentidadSexual == IdentidadSexual.HombreTransexual);
            MujerCisexualCount = personas.Count(p => p.IdentidadSexual == IdentidadSexual.MujerCisexual);
            MujerTransexualCount = personas.Count(p => p.IdentidadSexual == IdentidadSexual.MujerTransexual);
            NoProporcionadoCount = personas.Count(p => p.IdentidadSexual == IdentidadSexual.NoProporcionado);
            OtraIdentidadCount = personas.Count(p => p.IdentidadSexual == IdentidadSexual.Otra);

        }

        public int HombreCisexualCount { get; set; } = 3;
        public int HombreTransexualCount { get; set; } = 4;
        public int MujerCisexualCount { get; set; } = 5;
        public int MujerTransexualCount { get; set; } = 6;
        public int NoProporcionadoCount { get; set; } = 7;
        public int OtraIdentidadCount { get; set; } = 8;
        public Func<ChartPoint, string> PointLabel { get; set; }
    }
}