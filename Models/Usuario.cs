using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturaWeb.Models
{
    public class Usuario
    {
       
          
            public int id { get; set; } 
            public int estado { get; set; } 
            public  Rol idRol { get; set; }
            public string email { get; set; }
            public string password{ get; set; }
           
            
        
    }
}