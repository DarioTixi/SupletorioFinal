using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturaWeb.Models
{
    public class Respuesta
    {
        public string Error { get; set; }
        public Object Datos { get; set; }
        public Respuesta()
        {

        }
    }
}