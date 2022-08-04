using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacturaWeb.Models
{
    public class Cliente
    {
        public int Num { get; set; }
        public int ClienteID { get; set; }
        public string Cedula { get; set; }
        public string NomCliente { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public int Estado { get; set; }
        public Cliente()
        {

        }
    }

    public class DetalleFactura {
        public int ID_DetFac { get; set; }
        public int Factura { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }

    }

    public class Factura {
        public int Id_Venta { get; set; }
        public string Fecha { get; set; }
        public string Cedula { get; set; }
        public string Cliente { get; set; }
        public string Subtotal { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }

        public List<Producto> Productos { get; set; }
        
        public Factura()
        {
            Productos = new List<Producto>();
        }
    }

    public class Producto 
    {
        public int Num { get; set; }
        public int ProductoID { get; set; }
        public int Estado { get; set; }
        public string NomProducto { get; set; }
        public string  Stock { get; set; }
        public string Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public Producto()
        {

        }
    }
}