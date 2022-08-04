using FacturaWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacturaWeb.Controllers
{
    [Authorize]
    public class InicioController : Controller
    {
        public ActionResult Index()
        {
            Usuario sessionUser = (Usuario)Session["Usuario"];
            ViewBag.UsuarioAdm = sessionUser.idRol == Rol.administrador ? "0" : "1";
      
            return View();
        }

        public string ListarFacturas() {

            Respuesta respuesta = new Respuesta();
            List<Factura> facturas = new List<Factura>();
            Usuario sessionUser = (Usuario)Session["Usuario"];
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);
                SqlCommand command = new SqlCommand("SP_ObtenerFacturasbyUser", connection);
                command.CommandType = CommandType.StoredProcedure;

                int i = sessionUser.idRol == Rol.administrador ? 1 : sessionUser.id;

                command.Parameters.AddWithValue("@idUsuario", i);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Factura factura = new Factura();
                        factura.Id_Venta = reader.GetInt32(0);
                        factura.Fecha = reader.GetDateTime(1).ToShortDateString();
                        factura.Cedula = reader.GetString(2);
                        factura.Cliente = reader.GetString(3);
                        factura.Subtotal = reader.GetDecimal(4).ToString();
                        factura.Iva = reader.GetDecimal(5).ToString();
                        factura.Total = reader.GetDecimal(6).ToString();
                        facturas.Add(factura);
                    }
                }
                connection.Close();
                respuesta.Datos = facturas;
                return JsonConvert.SerializeObject(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Error = ex.Message;
                return JsonConvert.SerializeObject(respuesta);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }  
        public ActionResult NoAuthorized()
        {

            return View();
        }
    }
}