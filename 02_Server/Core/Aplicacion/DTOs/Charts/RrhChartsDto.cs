using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Wrappers;
using Aplicacion.DTOs.Persona;
using System.Net.NetworkInformation;

namespace Aplicacion.DTOs.Charts
{
    public class RrhChartsDto
    {
        public string Category { get; set; }
        public string FullName { get; set; }
        public double Value    { get; set; }
        public string Color    { get; set; }
    }
}

